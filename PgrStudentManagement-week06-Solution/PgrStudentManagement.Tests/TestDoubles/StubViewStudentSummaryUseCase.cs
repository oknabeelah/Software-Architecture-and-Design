using PgrStudentManagement.Business.Students.ViewStudentSummary;

namespace PgrStudentManagement.Tests.TestDoubles;

public sealed class StubViewStudentSummaryUseCase : IViewStudentSummaryUseCase
{
    public Task<StudentSummaryOutput?> ExecuteAsync(
        ViewStudentSummaryInput input,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<StudentSummaryOutput?>(
            new StudentSummaryOutput(input.StudentNumber, "Test Student"));
}
