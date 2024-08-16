using DustyPig.OpenSubtitles.Utils;
using System;
using System.Collections.Generic;

namespace DustyPig.OpenSubtitles.Models;

public class SearchSubtitleRequest
{
    public enum IncludeTypes
    {
        Include,
        Exclude,
        Only
    }

    public enum OrderByFields
    {
        Language,
        Download_Count,
        New_Download_Count,
        Hearing_Impaired,
        HD,
        FPS,
        Votes,
        Points,
        Ratings,
        From_Trusted,
        Foreign_Parts_Only,
        AI_Translated,
        Machine_Translated,
        Upload_Date,
        Release,
        Comments
    }

    public enum Types
    {
        All,
        Movie,
        Episode
    }

    /// <summary>
    /// Include AI Translated
    /// </summary>
    public bool AI_Translated { get; set; } = true;

    /// <summary>
    /// For Tvshows
    /// </summary>
    public int? EpisodeNumber { get; set; }

    public IncludeTypes ForeignPartsOnly { get; set; } = IncludeTypes.Include;

    public IncludeTypes HearingImpaired { get; set; } = IncludeTypes.Include;

    /// <summary>
    /// ID of the movie or episode
    /// </summary>
    public long? Id { get; set; }

    /// <summary>
    /// IMDB ID of the movie or episode
    /// </summary>
    public long? ImdbId { get; set; }

    public List<string> LanguageCodes { get; set; } = [];

    public bool MachineTranslated { get; set; } 

    public string MovieHash { get; set; }

    public bool MovieHashMatchesOnly { get; set; }

    public OrderByFields? OrderBy { get; set; }

    public bool OrderByDescending { get; set; }

    /// <summary>
    /// Results page to display
    /// </summary>
    public int? Page { get; set; }

    /// <summary>
    /// For Tvshows
    /// </summary>
    public long? ParentFeatureId { get; set; }

    /// <summary>
    /// For Tvshows
    /// </summary>
    public long? ParentImdbId { get; set; }

    /// <summary>
    /// For Tvshows
    /// </summary>
    public long? ParentTmdbId { get; set; }

    /// <summary>
    /// File name or text search
    /// </summary>
    public string Query { get; set; }

    /// <summary>
    /// For Tvshows
    /// </summary>
    public int? SeasonNumber { get; set; }

    /// <summary>
    /// TMDB ID of the movie or episode
    /// </summary>
    public long? TmdbId { get; set; }

    public bool TrustedSourcesOnly { get; set; }

    public Types Type { get; set; } = Types.All;

    /// <summary>
    /// To be used alone - for user uploads listing
    /// </summary>
    public long? UploaderId { get; set; }

    /// <summary>
    /// Filter by movie/episode year
    /// </summary>
    public int? Year { get; set; }


    internal string ToQueryParams()
    {
        string queryParams = string.Empty;
        queryParams = QueryParamBuilder.AddQueryParam(queryParams, "ai_translated", AI_Translated, true);
        queryParams = QueryParamBuilder.AddQueryParam(queryParams, "episode_number", EpisodeNumber, null);
        queryParams = QueryParamBuilder.AddQueryParam(queryParams, "foreign_parts_only", ForeignPartsOnly, IncludeTypes.Include);
        queryParams = QueryParamBuilder.AddQueryParam(queryParams, "hearing_impaired", HearingImpaired, IncludeTypes.Include);
        queryParams = QueryParamBuilder.AddQueryParam(queryParams, "id", Id, null);
        queryParams = QueryParamBuilder.AddQueryParam(queryParams, "imdb_id", ImdbId, null);
        queryParams = QueryParamBuilder.AddQueryParam(queryParams, "machine_translated", MachineTranslated, false);


        queryParams = QueryParamBuilder.AddQueryParam(queryParams, "moviehash", MovieHash);
        if (!string.IsNullOrWhiteSpace(MovieHash))
            queryParams = QueryParamBuilder.AddQueryParam(queryParams, "moviehash_match", MovieHashMatchesOnly, false);


        if (OrderBy != null)
        {
            queryParams = QueryParamBuilder.AddQueryParam(queryParams, "order_by", OrderBy.Value.ToString());
            queryParams = QueryParamBuilder.AddQueryParam(queryParams, "order_direction", OrderByDescending ? "desc" : "asc");
        }

        queryParams = QueryParamBuilder.AddQueryParam(queryParams, "page", Page, null);
        queryParams = QueryParamBuilder.AddQueryParam(queryParams, "parent_feature_id", ParentFeatureId, null);
        queryParams = QueryParamBuilder.AddQueryParam(queryParams, "parent_imdb_id", ParentImdbId, null);
        queryParams = QueryParamBuilder.AddQueryParam(queryParams, "parent_tmdb_id", ParentTmdbId, null);

        if(!string.IsNullOrWhiteSpace(Query))
            queryParams = QueryParamBuilder.AddQueryParam(queryParams, "query", Uri.EscapeDataString(Query.ToLower()).Replace("%20", "+"));
    
        queryParams = QueryParamBuilder.AddQueryParam(queryParams, "season_number", SeasonNumber, null);
        queryParams = QueryParamBuilder.AddQueryParam(queryParams, "tmdb_id", TmdbId, null);
        queryParams = QueryParamBuilder.AddQueryParam(queryParams, "trusted_sources", TrustedSourcesOnly, false);

        if (Type == Types.Movie)
            queryParams = QueryParamBuilder.AddQueryParam(queryParams, "type", "movie");
        if (Type == Types.Episode)
            queryParams = QueryParamBuilder.AddQueryParam(queryParams, "type", "episode");

        queryParams = QueryParamBuilder.AddQueryParam(queryParams, "uploader_id", UploaderId, null);
        queryParams = QueryParamBuilder.AddQueryParam(queryParams, "year", Year, null);


        return queryParams.Trim('&');
    }

    
}
