using System.Collections.Generic;

namespace DustyPig.OpenSubtitles.Models;

internal class InternalMessageResponse
{
    public string Message { get; set; }

    public List<string> Errors { get; set; }
}
