namespace PgrStudentManagement.Business.AcademicSupervisors.AssignAcademicSupervisor;

public record SupervisoryAssignmentOutput(
    string StudentNumber,
    string SupervisorStaffNumber,
    string Role,
    DateTime StartDate,
    DateTime? EndDate,
    bool ExceptionAuthorised);

