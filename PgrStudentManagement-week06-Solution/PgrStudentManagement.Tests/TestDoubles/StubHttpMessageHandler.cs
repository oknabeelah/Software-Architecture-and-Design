using System.Net;

namespace PgrStudentManagement.Tests.TestDoubles;

public sealed class StubHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

    public StubHttpMessageHandler(
        Func<HttpRequestMessage, HttpResponseMessage> handler)
    {
        _handler = handler;
    }

    public Uri? RequestedUri { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        RequestedUri = request.RequestUri;
        return Task.FromResult(_handler(request));
    }

    public static HttpResponseMessage JsonResponse(
        HttpStatusCode statusCode,
        string json) =>
        new(statusCode)
        {
            Content = new StringContent(
                json,
                System.Text.Encoding.UTF8,
                "application/json")
        };
}
