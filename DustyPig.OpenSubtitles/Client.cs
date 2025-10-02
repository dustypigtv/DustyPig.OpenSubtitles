using DustyPig.OpenSubtitles.Models;
using DustyPig.OpenSubtitles.Utils;
using DustyPig.REST;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DustyPig.OpenSubtitles;

public class Client
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

    private static readonly HttpClient _internalHttpClient = new(new EndpointSpecificThrottle());

    private readonly REST.Client _client;

    #region Constructor

    public Client() : this(null, null) { }

    public Client(HttpClient httpClient) : this(httpClient, null) { }

    public Client(ILogger<Client> logger) : this(null, logger) { }

    public Client(HttpClient httpClient, ILogger<Client> logger)
    {
        _client = new(httpClient ?? _internalHttpClient, logger)
        {
            IncludeRawContentInResponse = true,
        };
        _client.DefaultRequestHeaders.Add("Accept", "*/*");
        SetHost(DEFAULT_HOST);
    }

    #endregion





    #region Properties

    public bool AutoThrowIfError { get; set; }

    public bool IncludeRawContentInResponse { get; set; }
     
    public string AppName{ get; set; }

    public Version AppVersion { get; set; }

    public string ApiKey { get; set; }

    public string Token { get; set; }

    public ushort RetryCount
    {
        get => _client.RetryCount;
        set => _client.RetryCount = value;
    }

    #endregion





    #region Authentication Endpoints

    public async Task<Response<LoginResponse>> LoginAsync(Credentials credentials, CancellationToken cancellationToken = default)
    {
        var response = await PostAsync<LoginResponse>(TokenRequirementLevels.None, "login", credentials, cancellationToken).ConfigureAwait(false);
        if (response.Success)
        {
            Token = response.Data.Token;
            response.Data.TokenExpiresUTC = DateTime.UtcNow.AddHours(24).AddMinutes(-1);
            SetHost(response.Data.BasedUrl);
        }
        return FinalResponseHandler(response);
    }

    public Task<Response<LogoutResponse>> LogoutAsync(CancellationToken cancellationToken = default) =>
        DeleteAsync<LogoutResponse>(TokenRequirementLevels.Required, "logout", cancellationToken);


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

    public Task<Response<MovieFeaturesResponse>> GetPopularMovieFeaturesAsync(string language = LANGUAGE_VALUE_ALL, CancellationToken cancellationToken = default)
    {
        string queryParams = GetLanguageAndTypeQueryParams(language, SubtitleTypes.Movie);
        return GetAsync<MovieFeaturesResponse>(TokenRequirementLevels.Optional, $"discover/popular?{queryParams}", cancellationToken);
    }

    public Task<Response<TvShowFeaturesResponse>> GetPopularTvShowFeaturesAsync(string language = LANGUAGE_VALUE_ALL, CancellationToken cancellationToken = default)
    {
        string queryParams = GetLanguageAndTypeQueryParams(language, SubtitleTypes.TvShow);
        return GetAsync<TvShowFeaturesResponse>(TokenRequirementLevels.Optional, $"discover/popular?{queryParams}", cancellationToken);
    }

    public Task<Response<SubtitlesResponse>> GetLatestSubtitlesAsync(SubtitleTypes subtitleType = SubtitleTypes.Movie, string language = LANGUAGE_VALUE_ALL, CancellationToken cancellationToken = default)
    {
        string queryParams = GetLanguageAndTypeQueryParams(language, subtitleType);
        return GetAsync<SubtitlesResponse>(TokenRequirementLevels.Optional, $"discover/latest?{queryParams}", cancellationToken);
    }

    public Task<Response<SubtitlesResponse>> GetMostDownloadedSubtitlesAsync(SubtitleTypes subtitleType = SubtitleTypes.Movie, string language = LANGUAGE_VALUE_ALL, CancellationToken cancellationToken = default)
    {
        string queryParams = GetLanguageAndTypeQueryParams(language, subtitleType);
        return GetAsync<SubtitlesResponse>(TokenRequirementLevels.Optional, $"discover/most_downloaded?{queryParams}", cancellationToken);
    }

    static string GetLanguageAndTypeQueryParams(string language, SubtitleTypes subtitleType)
    {
        string queryParams = string.Empty;
        if (string.IsNullOrWhiteSpace(language))
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
    public Task<Response<DownloadResponse>> GetDownloadAsync(DownloadRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<DownloadResponse>(TokenRequirementLevels.Required, "download", request, cancellationToken);

    #endregion





    #region Feature Endpoints

    public Task<Response<MovieFeaturesResponse>> SearchForMovieFeaturesAsync(SearchFeatureRequest request, CancellationToken cancellationToken = default)
    {
        string queryParams = request.ToQueryParams("movie");
        return GetAsync<MovieFeaturesResponse>(TokenRequirementLevels.Optional, $"features?{queryParams}", cancellationToken);
    }

    public Task<Response<TvShowFeaturesResponse>> SearchForTvShowFeaturesAsync(SearchFeatureRequest request, CancellationToken cancellationToken = default)
    {
        string queryParams = request.ToQueryParams("tvshow");
        return GetAsync<TvShowFeaturesResponse>(TokenRequirementLevels.Optional, $"features?{queryParams}", cancellationToken);
    }

    public Task<Response<EpisodeFeatureResponse>> SearchForEpisodeeaturesAsync(SearchFeatureRequest request, CancellationToken cancellationToken = default)
    {
        string queryParams = request.ToQueryParams("episode");
        return GetAsync<EpisodeFeatureResponse>(TokenRequirementLevels.Optional, $"features?{queryParams}", cancellationToken);
    }

    #endregion





    #region Infos Endpoints

    public Task<Response<SubtitleFormatsResponse>> GetSubtitleFormatsAsync(CancellationToken cancellationToken = default) =>
        GetAsync<SubtitleFormatsResponse>(TokenRequirementLevels.Optional, $"infos/formats", cancellationToken);

    public Task<Response<LanguagesResponse>> GetLanguagesAsync(CancellationToken cancellationToken = default) =>
        GetAsync<LanguagesResponse>(TokenRequirementLevels.Optional, $"infos/languages", cancellationToken);

    public Task<Response<UserInformationResponse>> GetUserInformationAsync(CancellationToken cancellationToken = default) =>
        GetAsync<UserInformationResponse>(TokenRequirementLevels.Required, $"infos/user", cancellationToken);

    #endregion





    #region Subtitle Endpoints

    public Task<Response<SubtitlesResponse>> SearchAsync(SearchSubtitleRequest request, CancellationToken cancellationToken = default)
    {
        string queryParams = request.ToQueryParams();
        return GetAsync<SubtitlesResponse>(TokenRequirementLevels.Optional, $"subtitles?{queryParams}", cancellationToken);
    }

    #endregion





    #region Guessit Endpoints

    public Task<Response<GuessitResponse>> GuessitAsync(string filename, CancellationToken cancellationToken = default)
    {
        string queryParams = QueryParamBuilder.AddQueryParam(string.Empty, "filename", filename);
        return GetAsync<GuessitResponse>(TokenRequirementLevels.Optional, $"utilities/guessit?{queryParams}", cancellationToken);
    }

    #endregion





    #region Internal Methods

    void SetHost(string host) => _client.BaseAddress = new($"https://{host}/{URL_PREFIX}/");
            
    Response<T> FinalResponseHandler<T>(Response<T> response)
    {
        if (!response.Success)
        {
            try
            {
                var imr = JsonSerializer.Deserialize<InternalMessageResponse>(response.RawContent, _client.JsonSerializerOptions);
                if (!string.IsNullOrWhiteSpace(imr.Message))
                    response.Error = new Exception(imr.Message);
                else if (imr.Errors?.Count > 0)
                    response.Error = new Exception("Errors: " + string.Join(", ", imr.Errors));
            }
            catch { }
        }

        if (!IncludeRawContentInResponse)
            response.RawContent = null;

        if(AutoThrowIfError)
            response.ThrowIfError();

        return response;
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
   

    async Task<Response<T>> PostAsync<T>(TokenRequirementLevels tokenRequirementLevel, string url, object data, CancellationToken cancellationToken)
    {
        Dictionary<string, string> headers;
        try
        {
            headers = CreateHeaders(tokenRequirementLevel);
        }
        catch (Exception ex)
        {
            var ret = new Response<T> { Error = ex };
            return ret;
        }
        var response = await _client.PostAsync<T>(url, data, headers, cancellationToken).ConfigureAwait(false);
        return FinalResponseHandler(response);
    }


    async Task<Response<T>> GetAsync<T>(TokenRequirementLevels tokenRequirementLevel, string url, CancellationToken cancellationToken)
    {
        Dictionary<string, string> headers;
        try
        {
            headers = CreateHeaders(tokenRequirementLevel);
        }
        catch (Exception ex)
        {
            return new Response<T> { Error = ex };
        }

        var response = await _client.GetAsync<T>(url, headers, cancellationToken).ConfigureAwait(false);
        return FinalResponseHandler(response);
    }


    async Task<Response<T>> DeleteAsync<T>(TokenRequirementLevels tokenRequirementLevel, string url, CancellationToken cancellationToken)
    {
        Dictionary<string, string> headers;
        try
        {
            headers = CreateHeaders(tokenRequirementLevel);
        }
        catch (Exception ex)
        {
            return new Response<T> { Error = ex };
        }

        var response = await _client.DeleteAsync<T>(url, headers, null, cancellationToken).ConfigureAwait(false);
        return FinalResponseHandler(response);
    }

    #endregion
}
