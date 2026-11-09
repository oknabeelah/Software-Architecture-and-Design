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
        using var response = await _httpClient.GetAsync(
            $"api/students/{encodedStudentNumber}/lookup",
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Student lookup failed with HTTP status {(int)response.StatusCode} ({response.StatusCode}).",
                inner: null,
                response.StatusCode);
        }

        var apiResponse = await response.Content.ReadFromJsonAsync<
            StudentLookupApiResponse>(
                cancellationToken);

        if (apiResponse is null)
        {
            throw new HttpRequestException(
                "Student lookup returned an empty response.",
                inner: null,
                response.StatusCode);
        }

        return new StudentSummary(
            apiResponse.StudentNumber,
            apiResponse.DisplayName);
    }
}
