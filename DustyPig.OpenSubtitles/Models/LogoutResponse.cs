using System.Net;

namespace DustyPig.OpenSubtitles.Models;

public class LogoutResponse
{
    public string Message { get; set; }

    public HttpStatusCode Status { get; set; }
}
