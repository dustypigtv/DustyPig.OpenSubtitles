using System.Text.Json.Serialization;

namespace DustyPig.OpenSubtitles.Models;

public class SubtitleFile
{
    [JsonPropertyName("file_id")]
    public long FileId { get; set; }

    [JsonPropertyName("cd_number")]
    public long CdNumber { get; set; }

    [JsonPropertyName("file_name")]
    public string FileName { get; set; }
}
