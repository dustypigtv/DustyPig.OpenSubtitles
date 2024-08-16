using System.Text.Json.Serialization;

namespace DustyPig.OpenSubtitles.Models;

public class GuessitResponse
{
    public string Title { get; set; }

    public int? Year { get; set; }

    public string Language { get; set; }

    [JsonPropertyName("subtitle_language")]
    public string SubtitleLanguage { get; set; }

    [JsonPropertyName("screen_size")]
    public string ScreenSize { get; set; }

    [JsonPropertyName("streaming_service")]
    public string StreamingService { get; set; }

    public string Sourcce { get; set; }

    public string Other { get; set; }

    [JsonPropertyName("audio_codec")]
    public string AudioCodec { get; set; }

    [JsonPropertyName("audio_channels")]
    public string AudioChannels { get; set; }

    [JsonPropertyName("video_codec")]
    public string VideoCodec { get; set; }

    [JsonPropertyName("release_group")]
    public string ReleaseGroup { get; set; }

    public string Type { get; set; }

    public int? Season { get; set; }

    public int? Episode { get; set; }

    [JsonPropertyName("episode_title")]
    public string EpisodeTitle { get; set; }

    public string Container { get; set; }

    [JsonPropertyName("mimetype")]
    public string MimeType { get; set; }
}
