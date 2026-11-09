namespace PgrStudentManagement.Adapters.API.Responses;

public record AssignAcademicSupervisorResponse(
    string Outcome,
    string Message,
    SupervisoryAssignmentResponse? Assignment = null);

