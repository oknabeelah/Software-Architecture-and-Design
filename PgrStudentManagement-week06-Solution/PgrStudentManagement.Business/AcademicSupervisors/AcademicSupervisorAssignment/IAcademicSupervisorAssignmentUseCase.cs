namespace PgrStudentManagement.Business.AcademicSupervisors.AcademicSupervisorAssignment;

public interface IAcademicSupervisorUseCase
{
    Task<AcademicSupervisorAssignmentResult> ExecuteAsync(AcademicSupervisorAssignmentInput input,CancellationToken cancellationToken);
}
