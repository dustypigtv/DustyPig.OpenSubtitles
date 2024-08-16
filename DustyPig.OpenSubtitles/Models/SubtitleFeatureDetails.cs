using System.Text.Json.Serialization;

namespace DustyPig.OpenSubtitles.Models;

public class SubtitleFeatureDetails
{
    [JsonPropertyName("feature_id")]
    public long FeatureId { get; set; }

    [JsonPropertyName("feature_type")]
    public string FeatureType { get; set; }

    public int? Year { get; set; }

    public string Title { get; set; }

    [JsonPropertyName("movie_name")]
    public string MovieName { get; set; }

    [JsonPropertyName("imdb_id")]
    public long? ImdbId { get; set; }

    [JsonPropertyName("tmdb_id")]
    public long? TmdbId { get; set; }
}
