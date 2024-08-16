using DustyPig.OpenSubtitles.Models;
using DustyPig.OpenSubtitles.Utils;
using DustyPig.REST;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DustyPig.OpenSubtitles;

public class Client : IDisposable
{
    const string DEFAULT_HOST = "api.opensubtitles.com";
    const string URL_PREFIX = "api/v1";
    const string LANGUAGE_VALUE_ALL = "all";

    enum TokenRequirementLevels
    {
        None,
        Optional,
        Required
    }

    readonly REST.Client _client;





    #region Constructor / Dispose

    public Client(string appName = null, Version appVersion = null, string apiKey = null, HttpClient httpClient = null)
    {
        AppName = appName;
        AppVersion = appVersion;
        ApiKey = apiKey;

        _client = new(httpClient ?? new())
        {
            //Except for login, throttle is 5/sec
            Throttle = 200,
            IncludeRawContentInResponse = true,
        };
        _client.DefaultRequestHeaders.Add("Accept", "*/*");
        SetHost(DEFAULT_HOST);
    }


    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _client.Dispose();
    }

    #endregion





    #region Properties

    public bool AutoThrowIfError { get; set; }

    public bool IncludeRawContentInResponse { get; set; }
     
    public string AppName{ get; set; }

    public Version AppVersion { get; set; }

    public string ApiKey { get; set; }

    public string Token { get; set; }

    /// <summary>
    /// How many times to retry a call when the server returns a 5xx status code. Default = 2, and the client will wait 1 second between retries.
    /// </summary>
    public uint RetryCount { get; set; } = 2;

    #endregion





    #region Authentication Endpoints

    public async Task<Response<LoginResponse>> LoginAsync(Credentials credentials, CancellationToken cancellationToken = default)
    {
        var response = await PostAsync<LoginResponse>(TokenRequirementLevels.None, "login", credentials, cancellationToken, 1000).ConfigureAwait(false);
        if (response.Success)
        {
            response.Data.TokenExpiresUTC = DateTime.UtcNow.AddDays(1).AddMinutes(-1);
            RestoreSession(response.Data.GetSessionInfo());
        }
        else
        {
            try
            {
                var imr = JsonSerializer.Deserialize<InternalMessageResponse>(response.RawContent);
                if (!string.IsNullOrWhiteSpace(imr.Message))
                    response.Error = new Exception(imr.Message);
            }
            catch { }

        }

        FinalResponseHandler(response);

        return response;
    }


    public async Task<Response<LogoutResponse>> LogoutAsync(CancellationToken cancellationToken = default)
    {
        var response = await DeleteAsync<LogoutResponse>(TokenRequirementLevels.Required, "logout", cancellationToken).ConfigureAwait(false);
        if (!response.Success)
        {
            try
            {
                var imr = JsonSerializer.Deserialize<InternalMessageResponse>(response.RawContent);
                if (!string.IsNullOrWhiteSpace(imr.Message))
                    response.Error = new Exception(imr.Message);
                else if (imr.Errors?.Count > 0)
                    response.Error = new Exception("Errors: " + string.Join(", ", imr.Errors));
            }
            catch { }
        }

        FinalResponseHandler(response);

        return response;
    }

    /// <summary>
    /// NOT part of the api. This is here for convenience
    /// </summary>
    public void RestoreSession(SessionInfo sessionInfo)
    {
        if (DateTime.UtcNow > sessionInfo.ExpiresUTC)
            throw new Exception("Token is expired");

        Token = sessionInfo.Token;
        SetHost(sessionInfo.BaseUrl);
    }

    #endregion





    #region Discover Endpoints

    public async Task<Response<MovieFeaturesResponse>> GetPopularMovieFeaturesAsync(string language = LANGUAGE_VALUE_ALL, CancellationToken cancellationToken = default)
    {
        string queryParams = GetLanguageAndTypeQueryParams(language, SubtitleTypes.Movie);

        var response = await GetAsync<MovieFeaturesResponse>(TokenRequirementLevels.Optional, $"discover/popular?{queryParams}", cancellationToken).ConfigureAwait(false);
        FinalResponseHandler(response);
        return response;
    }

    public async Task<Response<TvShowFeaturesResponse>> GetPopularTvShowFeaturesAsync(string language = LANGUAGE_VALUE_ALL, CancellationToken cancellationToken = default)
    {
        string queryParams = GetLanguageAndTypeQueryParams(language, SubtitleTypes.TvShow);

        var response = await GetAsync<TvShowFeaturesResponse>(TokenRequirementLevels.Optional, $"discover/popular?{queryParams}", cancellationToken).ConfigureAwait(false);
        FinalResponseHandler(response);
        return response;
    }

    public async Task<Response<SubtitlesResponse>> GetLatestSubtitlesAsync(SubtitleTypes subtitleType = SubtitleTypes.Movie, string language = LANGUAGE_VALUE_ALL, CancellationToken cancellationToken = default)
    {
        string queryParams = GetLanguageAndTypeQueryParams(language, subtitleType);

        var response = await GetAsync<SubtitlesResponse>(TokenRequirementLevels.Optional, $"discover/latest?{queryParams}", cancellationToken).ConfigureAwait(false);
        FinalResponseHandler(response);
        return response;
    }

    public async Task<Response<SubtitlesResponse>> GetMostDownloadedSubtitlesAsync(SubtitleTypes subtitleType = SubtitleTypes.Movie, string language = LANGUAGE_VALUE_ALL, CancellationToken cancellationToken = default)
    {
        string queryParams = GetLanguageAndTypeQueryParams(language, subtitleType);

        var response = await GetAsync<SubtitlesResponse>(TokenRequirementLevels.Optional, $"discover/most_downloaded?{queryParams}", cancellationToken).ConfigureAwait(false);
        FinalResponseHandler(response);
        return response;
    }

    static string GetLanguageAndTypeQueryParams(string language, SubtitleTypes subtitleType)
    {
        string queryParams = string.Empty;
        if(string.IsNullOrWhiteSpace(language))
            language = LANGUAGE_VALUE_ALL;

        queryParams = QueryParamBuilder.AddQueryParam(queryParams, "language", language);
                
        string stype = subtitleType == SubtitleTypes.TvShow ? "tvshow" : "movie";
        queryParams = QueryParamBuilder.AddQueryParam(queryParams, "type", stype);

        return queryParams.Trim('&');
    }

    #endregion





    #region Download Endpoints

    /// <summary>
    /// The download count is calculated on this action, not the file download itself
    /// </summary>
    public async Task<Response<DownloadResponse>> GetDownloadAsync(DownloadRequest request, CancellationToken cancellationToken = default)
    {
        var response = await PostAsync<DownloadResponse>(TokenRequirementLevels.Required, "download", request, cancellationToken).ConfigureAwait(false);
        FinalResponseHandler(response);
        return response;
    }

    #endregion





    #region Feature Endpoints

    public async Task<Response<MovieFeaturesResponse>> SearchForMovieFeaturesAsync(SearchFeatureRequest request, CancellationToken cancellationToken = default)
    {
        string queryParams = request.ToQueryParams("movie");

        var response = await GetAsync<MovieFeaturesResponse>(TokenRequirementLevels.Optional, $"features?{queryParams}", cancellationToken).ConfigureAwait(false);
        FinalResponseHandler(response);
        return response;
    }

    public async Task<Response<TvShowFeaturesResponse>> SearchForTvShowFeaturesAsync(SearchFeatureRequest request, CancellationToken cancellationToken = default)
    {
        string queryParams = request.ToQueryParams("tvshow");

        var response = await GetAsync<TvShowFeaturesResponse>(TokenRequirementLevels.Optional, $"features?{queryParams}", cancellationToken).ConfigureAwait(false);
        FinalResponseHandler(response);
        return response;
    }

    public async Task<Response<EpisodeFeatureResponse>> SearchForEpisodeeaturesAsync(SearchFeatureRequest request, CancellationToken cancellationToken = default)
    {
        string queryParams = request.ToQueryParams("episode");

        var response = await GetAsync<EpisodeFeatureResponse>(TokenRequirementLevels.Optional, $"features?{queryParams}", cancellationToken).ConfigureAwait(false);
        FinalResponseHandler(response);
        return response;
    }

    #endregion





    #region Infos Endpoints

    public async Task<Response<SubtitleFormatsResponse>> GetSubtitleFormatsAsync(CancellationToken cancellationToken = default)
    {
        var response = await GetAsync<SubtitleFormatsResponse>(TokenRequirementLevels.Optional, $"infos/formats", cancellationToken).ConfigureAwait(false);
        FinalResponseHandler(response);
        return response;
    }

    public async Task<Response<LanguagesResponse>> GetLanguagesAsync(CancellationToken cancellationToken = default)
    {
        var response = await GetAsync<LanguagesResponse>(TokenRequirementLevels.Optional, $"infos/languages", cancellationToken).ConfigureAwait(false);
        FinalResponseHandler(response);
        return response;
    }

    public async Task<Response<UserInformationResponse>> GetUserInformationAsync(CancellationToken cancellationToken = default)
    {
        var response = await GetAsync<UserInformationResponse>(TokenRequirementLevels.Required, $"infos/user", cancellationToken).ConfigureAwait(false);
        FinalResponseHandler(response);
        return response;
    }

    #endregion





    #region Subtitle Endpoints

    public async Task<Response<SubtitlesResponse>> SearchAsync(SearchSubtitleRequest request, CancellationToken cancellationToken = default)
    {
        string queryParams = request.ToQueryParams();

        var response = await GetAsync<SubtitlesResponse>(TokenRequirementLevels.Optional, $"subtitles?{queryParams}", cancellationToken).ConfigureAwait(false);
        FinalResponseHandler(response);
        return response;
    }

    #endregion





    #region Guessit Endpoints

    public async Task<Response<GuessitResponse>> GuessitAsync(string filename, CancellationToken cancellationToken = default)
    {
        string queryParams = QueryParamBuilder.AddQueryParam(string.Empty, "filename", filename);

        var response = await GetAsync<GuessitResponse>(TokenRequirementLevels.Optional, $"utilities/guessit?{queryParams}", cancellationToken).ConfigureAwait(false);
        FinalResponseHandler(response);
        return response;
    }

    #endregion





    #region Internal Methods

    void SetHost(string host) => _client.BaseAddress = new($"https://{host}/api/v1/");
            
    void FinalResponseHandler(Response response)
    {
        if (!IncludeRawContentInResponse)
            response.RawContent = null;

        if(AutoThrowIfError)
            response.ThrowIfError();
    }
    
    Dictionary<string, string> CreateHeaders(TokenRequirementLevels tokenRequirementLevel)
    {
        if (string.IsNullOrWhiteSpace(ApiKey))
            throw new Exception("Invalid " + nameof(ApiKey));

        if(string.IsNullOrWhiteSpace(AppName))
            throw new Exception("Invalid " + nameof(AppName));

        if (AppVersion == null || AppVersion.Equals(new Version()))
            throw new Exception("Invalid " + nameof(AppVersion));

        if (tokenRequirementLevel == TokenRequirementLevels.Required && string.IsNullOrWhiteSpace(Token))
            throw new Exception("Invalid " + nameof(Token));

        var ret = new Dictionary<string, string>
        {
            { "User-Agent", $"{AppName} v{AppVersion}" },
            { "Api-Key", ApiKey }
        };

        if (tokenRequirementLevel != TokenRequirementLevels.None && !string.IsNullOrWhiteSpace(Token))
            ret.Add("Authorization", "Bearer " + Token);

        return ret;
    }

    

    async Task<Response<T>> PostAsync<T>(TokenRequirementLevels tokenRequirementLevel, string url, object data, CancellationToken cancellationToken, int throttle = 250)
    {
        Dictionary<string, string> headers;
        try { headers = CreateHeaders(tokenRequirementLevel); }
        catch (Exception ex)
        {
            var ret = new Response<T> { Error = ex };
            if (AutoThrowIfError)
                ret.ThrowIfError();
            return ret;
        }

        _client.Throttle = throttle;
        var response = new Response<T> { StatusCode = HttpStatusCode.InternalServerError };
        for (int i = 0; i < RetryCount + 1; i++)
        {
            if (!response.Success && (int)(response.StatusCode) >= 500)
            {
                if(i > 0)
                    await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
                response = await _client.PostAsync<T>(url, data, headers, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                break;
            }
        }

        return response;

    }
    

    async Task<Response<T>> GetAsync<T>(TokenRequirementLevels tokenRequirementLevel, string url, CancellationToken cancellationToken, int throttle = 250)
    {
        Dictionary<string, string> headers;
        try { headers = CreateHeaders(tokenRequirementLevel); }
        catch (Exception ex)
        {
            var ret = new Response<T> { Error = ex };
            if (AutoThrowIfError)
                ret.ThrowIfError();
            return ret;
        }

        _client.Throttle = throttle;
        var response = new Response<T> { StatusCode = HttpStatusCode.InternalServerError };
        for (int i = 0; i < RetryCount + 1; i++)
        {
            if (!response.Success && (int)(response.StatusCode) >= 500)
            {
                if (i > 0)
                    await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
                response = await _client.GetAsync<T>(url, headers, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                break;
            }
        }

        return response;
    }


    async Task<Response<T>> DeleteAsync<T>(TokenRequirementLevels tokenRequirementLevel, string url, CancellationToken cancellationToken, int throttle = 250)
    {
        Dictionary<string, string> headers;
        try { headers = CreateHeaders(tokenRequirementLevel); }
        catch (Exception ex)
        {
            var ret = new Response<T> { Error = ex };
            if (AutoThrowIfError)
                ret.ThrowIfError();
            return ret;
        }

        _client.Throttle = throttle;
        var response = new Response<T> { StatusCode = HttpStatusCode.InternalServerError };
        for (int i = 0; i < RetryCount + 1; i++)
        {
            if (!response.Success && (int)(response.StatusCode) >= 500)
            {
                if (i > 0)
                    await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
                response = await _client.DeleteAsync<T>(url, headers, null, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                break;
            }
        }

        return response;
    }

    #endregion
}
