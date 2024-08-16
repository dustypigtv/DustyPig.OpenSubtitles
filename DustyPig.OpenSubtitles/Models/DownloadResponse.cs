using System;
using System.Text.Json.Serialization;

namespace DustyPig.OpenSubtitles.Models;

public class DownloadResponse
{
    public string Link { get; set; }

    [JsonPropertyName("file_name")]
    public string FileName { get; set; }

    public int Requests { get; set; }

    public int Remaining { get; set; }

    public string Message { get; set; }

    [JsonPropertyName("reset_time")]
    public string ResetTime { get; set; }

    [JsonPropertyName("reset_time_utc")]
    public DateTime ResetTimeUTC { get; set; }
}
