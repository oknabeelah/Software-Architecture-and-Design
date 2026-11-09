using PgrStudentManagement.Business.AcademicSupervisors.Gateway;

namespace PgrStudentManagement.Tests.TestDoubles;

public class FakeStudentLookup : IStudentLookup
{
    private readonly Dictionary<string, StudentSummary> _students =
        new(StringComparer.OrdinalIgnoreCase);

    public int NumberOfCalls { get; private set; }

    public string? LastRequestedStudentNumber { get; private set; }

    public void AddStudent(string studentNumber, string displayName)
    {
        _students[studentNumber] = new StudentSummary(studentNumber, displayName);
    }

    public Task<StudentSummary?> GetStudentSummaryAsync(
        string studentNumber,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        NumberOfCalls++;
        LastRequestedStudentNumber = studentNumber;

        _students.TryGetValue(studentNumber, out var student);
        return Task.FromResult(student);
    }
}

