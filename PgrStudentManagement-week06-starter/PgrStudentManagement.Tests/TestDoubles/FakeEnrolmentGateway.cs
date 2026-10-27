
using PgrStudentManagement.Business.Students.Gateway;
using PgrStudentManagement.Domain.Students;

// The fake gateway should implement IEnrolmentGateway without EF Core or SQLite
namespace PgrStudentManagement.Tests.TestDoubles;

public sealed class FakeEnrolmentGateway : IEnrolmentGateway
{
    private readonly IReadOnlyList<Enrolment> _enrolments;

    public FakeEnrolmentGateway(IEnumerable<Enrolment> enrolments)
    {
        _enrolments = enrolments.ToList();
    }

    public IReadOnlyList<Enrolment> Enrolments => _enrolments;

    public Task<IReadOnlyList<Enrolment>> GetForStudentAsync(
        string studentNumber,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<Enrolment> matchingEnrolments = _enrolments
            .Where(enrolment => enrolment.StudentNumber == studentNumber)
            .ToList();

        return Task.FromResult(matchingEnrolments);
    }
}