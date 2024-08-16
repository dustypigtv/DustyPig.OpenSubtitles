using System.Text.Json.Serialization;

namespace DustyPig.OpenSubtitles.Models;

public class FeatureMovie
{
    public string Id { get; set; }

    public string Type { get; set; }

    public FeatureMovieAttributes Attributes { get; set; }
}
