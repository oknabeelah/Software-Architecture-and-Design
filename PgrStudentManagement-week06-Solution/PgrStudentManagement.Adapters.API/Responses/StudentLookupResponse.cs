namespace PgrStudentManagement.Adapters.API.Responses;

public sealed record StudentLookupResponse(
    string StudentNumber,
    string DisplayName);
