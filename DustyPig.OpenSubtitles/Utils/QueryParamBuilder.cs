using static DustyPig.OpenSubtitles.Models.SearchSubtitleRequest;

namespace DustyPig.OpenSubtitles.Utils;

static class QueryParamBuilder
{
    public static string AddQueryParam(string queryParams, string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            return queryParams;
        if (string.IsNullOrWhiteSpace(value))
            return queryParams;
        return $"{queryParams}&{key.ToLower()}={value.ToLower()}";
    }

    public static string AddQueryParam(string queryParams, string key, bool val, bool defaultVal)
    {
        if (val == defaultVal)
            return queryParams;
        return AddQueryParam(queryParams, key, val.ToString());
    }

    public static string AddQueryParam(string queryParams, string key, IncludeTypes includeType, IncludeTypes defaultVal)
    {
        if (includeType == defaultVal)
            return queryParams;
        return AddQueryParam(queryParams, key, includeType.ToString());
    }

    public static string AddQueryParam(string queryParams, string key, int? val, int? defaultVal)
    {
        if (val == null || val == defaultVal)
            return queryParams;
        return AddQueryParam(queryParams, key, val.Value.ToString());
    }

    public static string AddQueryParam(string queryParams, string key, long? val, long? defaultVal)
    {
        if (val == null || val == defaultVal)
            return queryParams;
        return AddQueryParam(queryParams, key, val.Value.ToString());
    }
}
