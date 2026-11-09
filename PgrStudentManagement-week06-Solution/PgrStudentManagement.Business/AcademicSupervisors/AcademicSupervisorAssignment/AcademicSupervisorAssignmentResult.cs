namespace PgrStudentManagement.Business.AcademicSupervisors.AcademicSupervisorAssignment;

public record AcademicSupervisorAssignmentResult(
    AcademicSupervisorAssignmentOutcome Outcome,
    string? AssignmentId = null)
{
    public bool Succeeded =>
        Outcome == AcademicSupervisorAssignmentOutcome.Created;
}