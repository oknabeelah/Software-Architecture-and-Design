using System.Net;
using System.Net.Http.Json;
using PgrStudentManagement.Business.AcademicSupervisors.Gateway;

namespace PgrStudentManagement.Adapters.API.AcademicSupervisors.StudentLookupClient;

public sealed class HttpStudentLookupAdapter : IStudentLookup
{
    private readonly HttpClient _httpClient;

    public HttpStudentLookupAdapter(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<StudentSummary?> GetStudentSummaryAsync(
        string studentNumber,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(studentNumber))
        {
            return null;
        }

        var encodedStudentNumber = Uri.EscapeDataString(studentNumber.Trim());
        var requestUri = $"api/students/{encodedStudentNumber}/lookup";

        using var response = await _httpClient.GetAsync(
            requestUri,
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"The Students lookup service returned an unexpected status code {(int)response.StatusCode} ({response.StatusCode}).",
                inner: null,
                statusCode: response.StatusCode);
        }

        var payload = await response.Content.ReadFromJsonAsync<StudentLookupApiResponse>(
            cancellationToken: cancellationToken);

        if (payload is null || string.IsNullOrWhiteSpace(payload.StudentNumber))
        {
            throw new InvalidOperationException(
                "The Students lookup service returned an empty or invalid response payload.");
        }

        return new StudentSummary(
            payload.StudentNumber,
            payload.DisplayName);
    }
}

