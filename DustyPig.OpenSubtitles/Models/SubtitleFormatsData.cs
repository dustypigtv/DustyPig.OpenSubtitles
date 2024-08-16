using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DustyPig.OpenSubtitles.Models;

public class SubtitleFormatsData
{
    [JsonPropertyName("output_formats")]
    public List<string> OutputFormats { get; set; } = [];
}
