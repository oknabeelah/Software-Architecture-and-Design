namespace PgrStudentManagement.Business.AcademicSupervisors.ViewSupervisoryTeam;

public interface IViewSupervisoryTeamUseCase
{
    Task<SupervisoryTeamOutput> ExecuteAsync(
        ViewSupervisoryTeamInput input,
        CancellationToken cancellationToken = default);
}

