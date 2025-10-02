using DustyPig.REST;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace DustyPig.OpenSubtitles;

internal class CustomRateThrottle : DelegatingHandler
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <summary>
    /// API allows 5 requests per second, except for the login endpoint which is limited to 1 request per second
    /// </summary>
    public CustomRateThrottle() : base(new SlidingRateThrottle(5, TimeSpan.FromSeconds(1)))
    {
    }


    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        //Special case for login endpoint
        if (request.RequestUri.AbsolutePath == "/api/v1/login")
        {
            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                new Thread(WaitAndRelease) { IsBackground = true }.Start();
            }
        }

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        //Special case for login endpoint
        if (request.RequestUri.AbsolutePath == "/api/v1/login")
        {
            _semaphore.Wait(cancellationToken);
            try
            {
                return base.Send(request, cancellationToken);
            }
            finally
            {
                new Thread(WaitAndRelease) { IsBackground = true }.Start();
            }
        }

        return base.Send(request, cancellationToken);
    }

    private void WaitAndRelease()
    {
        Thread.Sleep(1000);
        _semaphore.Release();
    }
}