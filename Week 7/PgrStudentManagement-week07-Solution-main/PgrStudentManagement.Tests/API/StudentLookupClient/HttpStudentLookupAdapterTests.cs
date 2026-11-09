using System.Net;
using System.Text;
using PgrStudentManagement.Adapters.API.AcademicSupervisors.StudentLookupClient;

namespace PgrStudentManagement.Tests.API.StudentLookupClient;

public class HttpStudentLookupAdapterTests
{
    [Fact]
    public async Task GetStudentSummaryAsync_WhenEndpointReturns200Ok_ReturnsStudentSummary()
    {
        Uri? capturedUri = null;
        var handler = new StubHttpMessageHandler((request, _) =>
        {
            capturedUri = request.RequestUri;
            var json = """{"studentNumber":"S100002","displayName":"Jane Smith"}""";
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5066")
        };

        var adapter = new HttpStudentLookupAdapter(httpClient);

        var summary = await adapter.GetStudentSummaryAsync("S100002", CancellationToken.None);

        Assert.NotNull(summary);
        Assert.Equal("S100002", summary.StudentNumber);
        Assert.Equal("Jane Smith", summary.DisplayName);
        Assert.Equal("http://localhost:5066/api/students/S100002/lookup", capturedUri?.ToString());
    }

    [Fact]
    public async Task GetStudentSummaryAsync_WhenEndpointReturns404NotFound_ReturnsNull()
    {
        var handler = new StubHttpMessageHandler((_, _) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)));

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5066")
        };

        var adapter = new HttpStudentLookupAdapter(httpClient);

        var summary = await adapter.GetStudentSummaryAsync("UNKNOWN", CancellationToken.None);

        Assert.Null(summary);
    }

    [Fact]
    public async Task GetStudentSummaryAsync_WhenEndpointReturns500_ThrowsHttpRequestException()
    {
        var handler = new StubHttpMessageHandler((_, _) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)));

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5066")
        };

        var adapter = new HttpStudentLookupAdapter(httpClient);

        await Assert.ThrowsAsync<HttpRequestException>(() =>
            adapter.GetStudentSummaryAsync("S100002", CancellationToken.None));
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _responder;

        public StubHttpMessageHandler(
            Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responder)
        {
            _responder = responder;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return _responder(request, cancellationToken);
        }
    }
}

