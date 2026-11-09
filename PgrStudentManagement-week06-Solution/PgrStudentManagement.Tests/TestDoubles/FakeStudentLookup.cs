using PgrStudentManagement.Business.AcademicSupervisors.Gateway;

namespace PgrStudentManagement.Tests.TestDoubles;

public sealed class FakeStudentLookup : IStudentLookup
{
    private readonly Dictionary<string, StudentSummary> _students =
        new(StringComparer.OrdinalIgnoreCase);

    public void Add(StudentSummary student) =>
        _students[student.StudentNumber] = student;

    public Task<StudentSummary?> GetStudentSummaryAsync(
        string studentNumber,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _students.TryGetValue(studentNumber, out var student);
        return Task.FromResult(student);
    }
}
