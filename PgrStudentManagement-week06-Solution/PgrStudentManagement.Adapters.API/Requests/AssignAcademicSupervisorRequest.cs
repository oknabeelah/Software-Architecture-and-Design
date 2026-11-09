namespace PgrStudentManagement.Adapters.API.Requests;

public sealed record AssignAcademicSupervisorRequest(
    string SupervisorStaffNumber,
    string Role,
    DateTime StartDate,
    bool ExceptionAuthorised);
