using System.Text.Json.Serialization;

namespace DustyPig.OpenSubtitles.Models;

public class SubtitleUploader
{
    [JsonPropertyName("uploader_id")]
    public long? UploaderId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("rank")]
    public string Rank { get; set; }
}
