namespace PgrStudentManagement.Business.AcademicSupervisors.AssignAcademicSupervisor;

public record AssignAcademicSupervisorResult(
    AssignmentOutcome Outcome,
    SupervisoryAssignmentOutput? Assignment = null)
{
    public bool Succeeded => Outcome == AssignmentOutcome.Created;
}

