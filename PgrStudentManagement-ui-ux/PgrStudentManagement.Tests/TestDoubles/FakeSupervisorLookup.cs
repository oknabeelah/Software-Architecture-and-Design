using PgrStudentManagement.Web.Modules.AcademicSupervisors.Contracts;

namespace PgrStudentManagement.Tests.TestDoubles;

public class FakeSupervisorLookup : ISupervisorLookup
{
    private readonly Dictionary<string, SupervisorSummary> _supervisors =
        new(StringComparer.OrdinalIgnoreCase);

    public void AddSupervisor(
        string staffNumber,
        string displayName,
        bool isEligibleMainSupervisor = true)
    {
        _supervisors[staffNumber] = new SupervisorSummary(
            staffNumber,
            displayName,
            isEligibleMainSupervisor);
    }

    public Task<SupervisorSummary?> GetSupervisorSummaryAsync(
        string staffNumber,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _supervisors.TryGetValue(staffNumber, out var supervisor);

        return Task.FromResult(supervisor);
    }

    public Task<IReadOnlyList<SupervisorSummary>> GetSupervisorSummariesAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<SupervisorSummary> list = _supervisors.Values.ToList();

        return Task.FromResult(list);
    }
}

