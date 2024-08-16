using System.Text.Json.Serialization;

namespace DustyPig.OpenSubtitles.Models;

public class DownloadRequest
{
    public DownloadRequest(long fileId) => FileId = fileId;

    [JsonPropertyName("file_id")]
    public long FileId { get; set; }

    /// <summary>
    /// From <see cref="Endpoints.Infos.GetSubtitleFormatsAsync(System.Threading.CancellationToken)"/>
    /// </summary>
    [JsonPropertyName("sub_format")]
    public string SubFormat { get; set; }

    /// <summary>
    /// Desired file name
    /// </summary>
    [JsonPropertyName("file_name")]
    public string FileName { get; set; }

    /// <summary>
    /// Used for conversions, in_fps and out_fps must then be indicated
    /// </summary>
    [JsonPropertyName("in_fps")]
    public double? InFPS { get; set; }

    /// <summary>
    /// Used for conversions, in_fps and out_fps must then be indicated
    /// </summary>
    [JsonPropertyName("out_fps")]
    public double? OutFPS { get; set; }

    /// <summary>
    /// Delay to add or remove to the subtitle, + or - value, in seconds, i.e. 2.5s or -1s
    /// </summary>
    public double? Timeshift { get; set; }

    /// <summary>
    /// Set subtitle file headers to "application/force-download"
    /// </summary>
    [JsonPropertyName("force_download")]
    public bool ForceDownload { get; set; }

}
