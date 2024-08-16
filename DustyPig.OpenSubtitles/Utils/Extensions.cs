using DustyPig.OpenSubtitles.Models;

namespace DustyPig.OpenSubtitles.Utils;

public static class Extensions
{
    public static SessionInfo GetSessionInfo(this LoginResponse loginResponse) => new()
    {
        BaseUrl = loginResponse.BasedUrl,
        ExpiresUTC = loginResponse.TokenExpiresUTC,
        Token = loginResponse.Token
    };
}
