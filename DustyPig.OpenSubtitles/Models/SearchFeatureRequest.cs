using DustyPig.OpenSubtitles.Utils;
using System;

namespace DustyPig.OpenSubtitles.Models;

public class SearchFeatureRequest
{
    public long? Featureid { get; set; }

    public long? ImdbId { get; set; }

    public string Query { get; set; }

    public long? TmdbId { get; set; }

    public int? Year { get; set; }


    internal string ToQueryParams(string type)
    {
        string queryParams = string.Empty;
        queryParams = QueryParamBuilder.AddQueryParam(queryParams, "feature_id", Featureid, null);
        queryParams = QueryParamBuilder.AddQueryParam(queryParams, "imdb_id", ImdbId, null);


        if (!string.IsNullOrWhiteSpace(Query))
            queryParams = QueryParamBuilder.AddQueryParam(queryParams, "query", Uri.EscapeDataString(Query.ToLower()).Replace("%20", "+"));

        queryParams = QueryParamBuilder.AddQueryParam(queryParams, "tmdb_id", TmdbId, null);

        if (type == "movie" || type == "tvshow" || type == "episode")
            queryParams = QueryParamBuilder.AddQueryParam(queryParams, "type", type);

        queryParams = QueryParamBuilder.AddQueryParam(queryParams, "year", Year, null);

        return queryParams.Trim('&');
    }

    static string AddQueryParam(string queryParams, string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            return queryParams;
        if (string.IsNullOrWhiteSpace(value))
            return queryParams;
        return $"{queryParams}&{key.ToLower()}={value.ToLower()}";
    }

}
