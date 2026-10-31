namespace PgrStudentManagement.Adapters.API.Responses;

public sealed record StudentEnrolmentsResponse(
    string StudentNumber,
    IReadOnlyList<EnrolmentResponse> Enrolments);
