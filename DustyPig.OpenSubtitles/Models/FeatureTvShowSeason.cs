using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DustyPig.OpenSubtitles.Models;

public class FeatureTvShowSeason
{
    [JsonPropertyName("season_number")]
    public int SeasonNumber { get; set; }

    public List<FeatureTvShowEpisode> Episodes { get; set; } = [];
}