namespace PgrStudentManagement.Adapters.API.Responses;

public sealed record AssignAcademicSupervisorResponse(
    bool Succeeded,
    string Outcome,
    string? AssignmentId = null);
