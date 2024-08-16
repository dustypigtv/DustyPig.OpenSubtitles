using System.Text.Json.Serialization;

namespace DustyPig.OpenSubtitles.Models;

public class UserData
{
    [JsonPropertyName("allowed_downloads")]
    public int AllowedDownloads { get; set; }

    public string Level { get; set; }

    [JsonPropertyName("user_id")]
    public int UserId { get; set; }

    [JsonPropertyName("ext_installed")]
    public bool ExtInstalled { get; set; }

    [JsonPropertyName("vip")]
    public bool VIP { get; set; }

    [JsonPropertyName("downloads_count")]
    public int DownloadsCount { get; set; }

    [JsonPropertyName("remaining_downloads")]
    public int RemainingDownloads { get; set; }
}
