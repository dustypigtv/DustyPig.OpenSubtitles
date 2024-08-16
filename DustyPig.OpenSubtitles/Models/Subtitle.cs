namespace DustyPig.OpenSubtitles.Models;

public class Subtitle
{
    public string Id { get; set; }

    public string Type { get; set; }

    public SubtitleAttributes Attributes { get; set; }
}
