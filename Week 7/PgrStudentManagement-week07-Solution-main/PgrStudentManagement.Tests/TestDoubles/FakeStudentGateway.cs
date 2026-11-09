using PgrStudentManagement.Business.Students.Gateway;
using PgrStudentManagement.Domain.Students;

namespace PgrStudentManagement.Tests.TestDoubles;

public class FakeStudentGateway : IStudentGateway
{
    private readonly Dictionary<string, Student> _students =
        new(StringComparer.OrdinalIgnoreCase);

    public int NumberOfCalls { get; private set; }

    public string? LastRequestedStudentNumber { get; private set; }

    public void Add(Student student)
    {
        _students[student.StudentNumber] = student;
    }

    public Task<Student?> GetByStudentNumberAsync(
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

