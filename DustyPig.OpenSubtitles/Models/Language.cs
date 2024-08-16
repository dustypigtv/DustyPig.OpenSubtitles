using System.Text.Json.Serialization;

namespace DustyPig.OpenSubtitles.Models;

public class Language
{
    [JsonPropertyName("language_code")]
    public string Code { get; set; }

    [JsonPropertyName("language_name")]
    public string Name { get; set; }
}
