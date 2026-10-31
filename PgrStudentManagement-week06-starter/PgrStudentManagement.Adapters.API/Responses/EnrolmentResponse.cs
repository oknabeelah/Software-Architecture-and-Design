namespace PgrStudentManagement.Adapters.API.Responses;

public sealed record EnrolmentResponse(
    string ProgrammeCode,
    DateTime StartDate,
    DateTime? ExpectedEndDate,
    string ModeOfStudy,
    string Status);
