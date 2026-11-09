namespace PgrStudentManagement.Business.AcademicSupervisors.AcademicSupervisorAssignment;

public record AcademicSupervisorAssignmentResult(
    bool Succeeded,
    string? AssignmentId = null,
    string? ErrorMessage = null );