using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DustyPig.OpenSubtitles.Models;

public class SubtitleAttributes
{
    [JsonPropertyName("subtitle_id")]
    public string SubtitleId { get; set; }

    public string Language { get; set; }

    [JsonPropertyName("download_count")]
    public int DownloadCount { get; set; }

    [JsonPropertyName("new_download_count")]
    public int NewDownloadCount { get; set; }

    [JsonPropertyName("hearing_impaired")]
    public bool HearingImpaired { get; set; }

    [JsonPropertyName("hd")]
    public bool HD { get; set; }

    [JsonPropertyName("fps")]
    public double FPS { get; set; }

    public int Votes { get; set; }

    public double Ratings { get; set; }

    [JsonPropertyName("from_trusted")]
    public bool? FromTrusted { get; set; }

    [JsonPropertyName("foreign_parts_only")]
    public bool ForeignPartsOnly { get; set; }

    [JsonPropertyName("upload_date")]
    public DateTime UploadDate { get; set; }

    [JsonPropertyName("file_hashes")]
    public List<string> FileHashes { get; set; }

    [JsonPropertyName("ai_translated")]
    public bool AiTranslated { get; set; }

    [JsonPropertyName("nb_cd")]
    public int NbCd { get; set; }

    public string Slug { get; set; }

    [JsonPropertyName("machine_translated")]
    public bool MachineTranslated { get; set; }

    public string Release { get; set; }

    public string Comments { get; set; }

    [JsonPropertyName("legacy_subtitle_id")]
    public long? LegacySubtitleId { get; set; }

    [JsonPropertyName("legacy_uploader_id")]
    public long? LegacyUploaderId { get; set; }

    public SubtitleUploader Uploader { get; set; }

    [JsonPropertyName("feature_details")]
    public SubtitleFeatureDetails FeatureDetails { get; set; }

    public Uri Url { get; set; }

    [JsonPropertyName("related_links")]
    public List<SubtitleRelatedLink> RelatedLinks { get; set; }

    public List<SubtitleFile> Files { get; set; }
}
