namespace PgrStudentManagement.Business.Students.ViewStudentSummary;

public interface IViewStudentSummaryUseCase
{
    Task<StudentSummaryOutput?> ExecuteAsync(
        ViewStudentSummaryInput input,
        CancellationToken cancellationToken = default);
}
