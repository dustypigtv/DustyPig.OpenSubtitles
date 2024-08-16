using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DustyPig.OpenSubtitles.Models;

public class FeatureTvShowAttributes
{
    public string Title { get; set; }

    [JsonPropertyName("original_title")]
    public string OriginalTitle { get; set; }

    public string Year { get; set; }

    [JsonPropertyName("imdb_id")]
    public long? ImdbId { get; set; }

    [JsonPropertyName("tmdb_id")]
    public long? TmdbId { get; set; }

    [JsonPropertyName("title_aka")]
    public List<string> TitleAKA { get; set; } = [];

    [JsonPropertyName("feature_id")]
    public string FeatureId { get; set; }

    public string Url { get; set; }

    [JsonPropertyName("img_url")]
    public string ImgUrl { get; set; }

    [JsonPropertyName("subtitles_count")]
    public int SubtitlesCount { get; set; }

    [JsonPropertyName("seasons_count")]
    public int SeasonsCount { get; set; }

    public List<FeatureTvShowSeason> Seasons { get; set; } = [];
}
