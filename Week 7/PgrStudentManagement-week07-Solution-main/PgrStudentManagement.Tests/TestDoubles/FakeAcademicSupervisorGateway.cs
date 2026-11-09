using PgrStudentManagement.Business.AcademicSupervisors.Gateway;
using PgrStudentManagement.Domain.AcademicSupervisors;

namespace PgrStudentManagement.Tests.TestDoubles;

public class FakeAcademicSupervisorGateway : IAcademicSupervisorGateway
{
    private readonly Dictionary<string, AcademicSupervisor> _supervisors =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly List<SupervisoryAssignment> _assignments = new();

    public Task<IReadOnlyList<AcademicSupervisor>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<AcademicSupervisor> result = _supervisors.Values
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ThenBy(s => s.StaffNumber)
            .ToList();

        return Task.FromResult(result);
    }

    public Task<AcademicSupervisor?> GetByStaffNumberAsync(
        string staffNumber,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _supervisors.TryGetValue(staffNumber, out var supervisor);
        return Task.FromResult(supervisor);
    }

    public Task<bool> StaffNumberExistsAsync(
        string staffNumber,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(_supervisors.ContainsKey(staffNumber));
    }

    public Task AddSupervisorAsync(
        AcademicSupervisor supervisor,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _supervisors[supervisor.StaffNumber] = supervisor;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<SupervisoryAssignment>> GetAssignmentsForStudentAsync(
        string studentNumber,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<SupervisoryAssignment> result = _assignments
            .Where(a => string.Equals(a.StudentNumber, studentNumber, StringComparison.OrdinalIgnoreCase))
            .OrderBy(a => a.StartDate)
            .ToList();

        return Task.FromResult(result);
    }

    public Task<bool> AssignmentExistsAsync(
        string studentNumber,
        string staffNumber,
        SupervisoryRole role,
        DateTime startDate,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var exists = _assignments.Any(a =>
            string.Equals(a.StudentNumber, studentNumber, StringComparison.OrdinalIgnoreCase)
            && string.Equals(a.SupervisorStaffNumber, staffNumber, StringComparison.OrdinalIgnoreCase)
            && a.Role == role
            && a.StartDate.Date == startDate.Date);

        return Task.FromResult(exists);
    }

    public Task AddAssignmentAsync(
        SupervisoryAssignment assignment,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _assignments.Add(assignment);
        return Task.CompletedTask;
    }
}

