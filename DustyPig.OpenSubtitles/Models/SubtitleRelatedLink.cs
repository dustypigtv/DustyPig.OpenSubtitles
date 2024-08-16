using System;
using System.Text.Json.Serialization;

namespace DustyPig.OpenSubtitles.Models;

public class SubtitleRelatedLink
{
    public string Label { get; set; }

    public Uri Url { get; set; }

    [JsonPropertyName("img_url")]
    public Uri ImgUrl { get; set; } 
}
