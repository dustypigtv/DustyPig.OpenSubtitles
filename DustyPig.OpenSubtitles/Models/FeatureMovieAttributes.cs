using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DustyPig.OpenSubtitles.Models;

public class FeatureMovieAttributes
{
    public string Title { get; set; }

    [JsonPropertyName("original_title")]
    public string OriginalTitle { get; set; }

    public string Year { get; set; }

    [JsonPropertyName("subtitles_counts")]
    public Dictionary<string, long> SubtitlesCounts { get; set; }

    [JsonPropertyName("subtitles_count")]
    public int SubtitlesCount { get; set; }

    [JsonPropertyName("seasons_count")]
    public int SeasonsCount { get; set; }

    [JsonPropertyName("parent_title")]
    public string ParentTitle { get; set; }

    [JsonPropertyName("season_number")]
    public int SeasonNumber { get; set; }

    [JsonPropertyName("episode_number")]
    public int? EpisodeNumber { get; set; }

    [JsonPropertyName("imdb_id")]
    public long? ImdbId { get; set; }

    [JsonPropertyName("tmdb_id")]
    public long? TmdbId { get; set; }
    
    [JsonPropertyName("parent_imdb_id")]
    public long? ParentImdbId { get; set; }

    [JsonPropertyName("feature_id")]
    public string FeatureId { get; set; }

    [JsonPropertyName("title_aka")]
    public List<string> TitleAKA { get; set; } = [];

    [JsonPropertyName("feature_type")]
    public string FeatureType { get; set; }
    
    public string Url { get; set; }

    [JsonPropertyName("img_url")]
    public string ImgUrl { get; set; }
}