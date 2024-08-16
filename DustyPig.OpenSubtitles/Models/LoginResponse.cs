using System;
using System.Net;
using System.Text.Json.Serialization;

namespace DustyPig.OpenSubtitles.Models;

public class LoginResponse
{
    public User User { get; set; }

    [JsonPropertyName("base_url")]
    public string BasedUrl { get; set; }

    /// <summary>
    /// Token is good for 24 hours
    /// </summary>
    public string Token { get; set; }

    [JsonPropertyName("status")]
    public HttpStatusCode Status { get; set; }

    /// <summary>
    /// NOT part of the api. This is added by the library for convenience
    /// </summary>
    [JsonIgnore]
    public DateTime TokenExpiresUTC { get; set; }

}
