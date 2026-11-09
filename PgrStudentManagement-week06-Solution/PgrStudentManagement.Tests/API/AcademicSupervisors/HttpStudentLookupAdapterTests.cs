using System.Net;
using PgrStudentManagement.Adapters.API.AcademicSupervisors.StudentLookupClient;
using PgrStudentManagement.Tests.TestDoubles;

namespace PgrStudentManagement.Tests.API.AcademicSupervisors;

public sealed class HttpStudentLookupAdapterTests
{
    [Fact]
    public async Task GetStudentSummaryAsync_WhenFound_ReturnsSummary()
    {
        var handler = new StubHttpMessageHandler(_ =>
            StubHttpMessageHandler.JsonResponse(
                HttpStatusCode.OK,
                """{"studentNumber":"S100002","displayName":"Jane Smith"}"""));
        using var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://students.example.test/")
        };
        var adapter = new HttpStudentLookupAdapter(client);

        var summary = await adapter.GetStudentSummaryAsync(
            " S100002 ",
            CancellationToken.None);

        Assert.NotNull(summary);
        Assert.Equal("S100002", summary.StudentNumber);
        Assert.Equal("Jane Smith", summary.DisplayName);
        Assert.Equal(
            "https://students.example.test/api/students/S100002/lookup",
            handler.RequestedUri?.ToString());
    }

    [Fact]
    public async Task GetStudentSummaryAsync_WhenNotFound_ReturnsNull()
    {
        var handler = new StubHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.NotFound));
        using var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://students.example.test/")
        };
        var adapter = new HttpStudentLookupAdapter(client);

        var summary = await adapter.GetStudentSummaryAsync(
            "S999999",
            CancellationToken.None);

        Assert.Null(summary);
    }

    [Fact]
    public async Task GetStudentSummaryAsync_WhenServiceFails_ThrowsWithStatus()
    {
        var handler = new StubHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
        using var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://students.example.test/")
        };
        var adapter = new HttpStudentLookupAdapter(client);

        var exception = await Assert.ThrowsAsync<HttpRequestException>(() =>
            adapter.GetStudentSummaryAsync("S100002", CancellationToken.None));

        Assert.Equal(
            HttpStatusCode.ServiceUnavailable,
            exception.StatusCode);
    }
}
