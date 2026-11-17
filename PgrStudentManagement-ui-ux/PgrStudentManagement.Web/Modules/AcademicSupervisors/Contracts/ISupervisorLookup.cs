namespace PgrStudentManagement.Web.Modules.AcademicSupervisors.Contracts;

public interface ISupervisorLookup
{
    Task<SupervisorSummary?> GetSupervisorSummaryAsync(
        string staffNumber,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SupervisorSummary>> GetSupervisorSummariesAsync(
        CancellationToken cancellationToken = default);
}

