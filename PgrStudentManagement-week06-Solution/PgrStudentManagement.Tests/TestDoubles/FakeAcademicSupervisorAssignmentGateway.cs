using PgrStudentManagement.Business.AcademicSupervisors.Gateway;
using PgrStudentManagement.Domain.AcademicSupervisors;

namespace PgrStudentManagement.Tests.TestDoubles;

public sealed class FakeAcademicSupervisorAssignmentGateway
    : IAcademicSupervisorAssignmentGateway
{
    private readonly List<AcademicSupervisor> _supervisors = [];
    private readonly List<SupervisoryAssignment> _assignments = [];

    public SupervisoryAssignment? AddedAssignment { get; private set; }

    public void AddSupervisor(AcademicSupervisor supervisor) =>
        _supervisors.Add(supervisor);

    public void AddAssignment(SupervisoryAssignment assignment) =>
        _assignments.Add(assignment);

    public Task<AcademicSupervisor?> GetSupervisorAsync(
        string staffNumber,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_supervisors.FirstOrDefault(supervisor =>
            string.Equals(
                supervisor.StaffNumber,
                staffNumber,
                StringComparison.OrdinalIgnoreCase)));
    }

    public Task<IReadOnlyList<SupervisoryAssignment>> GetForStudentAsync(
        string studentNumber,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<SupervisoryAssignment> result = _assignments
            .Where(assignment => assignment.StudentNumber == studentNumber)
            .ToList();
        return Task.FromResult(result);
    }

    public Task AddAsync(
        SupervisoryAssignment assignment,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        AddedAssignment = assignment;
        _assignments.Add(assignment);
        return Task.CompletedTask;
    }
}
