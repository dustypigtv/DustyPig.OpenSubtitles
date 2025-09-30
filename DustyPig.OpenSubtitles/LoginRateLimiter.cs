using DustyPig.REST;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace DustyPig.OpenSubtitles;

internal class LoginRateLimiter : DelegatingHandler
{
#if NET9_0_OR_GREATER
    private readonly Lock _locker = new();
#else
    private readonly object _locker = new();
#endif

    private readonly TimeSpan _delay = TimeSpan.FromSeconds(1);
    private DateTime _lastRequest = DateTime.MinValue;

    public LoginRateLimiter() : base(new SlidingRateLimiter(5, TimeSpan.FromSeconds(1)))
    {
    }


    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        //Special case for login endpoint
        if (request.RequestUri.AbsolutePath == "/api/v1/login")
        {
            lock (_locker)
            {
                TimeSpan timeSpan = _delay - (DateTime.UtcNow - _lastRequest);
                if (timeSpan > TimeSpan.Zero)
                {
                    HttpResponseMessage httpResponseMessage = new(HttpStatusCode.TooManyRequests);
                    httpResponseMessage.Headers.RetryAfter = new RetryConditionHeaderValue(timeSpan);
                    return Task.FromResult(httpResponseMessage);
                }

                try
                {
                    return base.SendAsync(request, cancellationToken);
                }
                finally
                {
                    _lastRequest = DateTime.UtcNow;
                }
            }
        }

        return base.SendAsync(request, cancellationToken);
    }
}