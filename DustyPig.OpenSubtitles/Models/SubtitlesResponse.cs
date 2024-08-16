using System.Collections.Generic;

namespace DustyPig.OpenSubtitles.Models;

public class SubtitlesResponse
{
    public int TotalPages { get; set; }

    public int TotalCount { get; set; }

    public int Page { get; set; }

    public List<Subtitle> Data { get; set; } = [];
}
