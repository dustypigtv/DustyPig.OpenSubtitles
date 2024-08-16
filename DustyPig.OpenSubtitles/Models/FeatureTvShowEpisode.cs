using System.Text.Json.Serialization;

namespace DustyPig.OpenSubtitles.Models;

public class FeatureTvShowEpisode
{
    [JsonPropertyName("episode_number")]
    public int EpisodeNumber { get; set; }

    public string Title { get; set; }

    [JsonPropertyName("feature_id")]
    public long FeatureId { get; set; }

    [JsonPropertyName("feature_imdb_id")]
    public long? FeatureImdbId { get; set; }

}
