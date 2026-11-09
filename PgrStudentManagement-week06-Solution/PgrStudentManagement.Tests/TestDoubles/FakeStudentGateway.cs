using PgrStudentManagement.Business.Students.Gateway;
using PgrStudentManagement.Domain.Students;

namespace PgrStudentManagement.Tests.TestDoubles;

public sealed class FakeStudentGateway : IStudentGateway
{
    private readonly List<Student> _students = [];

    public string? LastRequestedStudentNumber { get; private set; }

    public int NumberOfCalls { get; private set; }

    public void Add(Student student) =>
        _students.Add(student);

    public Task<Student?> GetByStudentNumberAsync(
        string studentNumber,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        LastRequestedStudentNumber = studentNumber;
        NumberOfCalls++;

        return Task.FromResult(_students.FirstOrDefault(student =>
            string.Equals(
                student.StudentNumber,
                studentNumber,
                StringComparison.OrdinalIgnoreCase)));
    }
}
