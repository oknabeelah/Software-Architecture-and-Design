namespace PgrStudentManagement.Adapters.API.Responses;

public record SupervisoryAssignmentResponse(
    string StudentNumber,
    string SupervisorStaffNumber,
    string Role,
    DateTime StartDate,
    DateTime? EndDate,
    bool ExceptionAuthorised);

