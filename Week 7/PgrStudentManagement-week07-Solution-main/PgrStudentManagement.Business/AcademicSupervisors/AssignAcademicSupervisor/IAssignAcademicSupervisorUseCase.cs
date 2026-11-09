namespace PgrStudentManagement.Business.AcademicSupervisors.AssignAcademicSupervisor;

public interface IAssignAcademicSupervisorUseCase
{
    Task<AssignAcademicSupervisorResult> ExecuteAsync(
        AssignAcademicSupervisorInput input,
        CancellationToken cancellationToken = default);
}

