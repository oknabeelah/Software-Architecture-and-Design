using PgrStudentManagement.Business.Students.Gateway;

namespace PgrStudentManagement.Business.Students.ViewStudentSummary;

public sealed class ViewStudentSummaryUseCase : IViewStudentSummaryUseCase
{
    private readonly IStudentGateway _gateway;

    public ViewStudentSummaryUseCase(IStudentGateway gateway)
    {
        _gateway = gateway;
    }

    public async Task<StudentSummaryOutput?> ExecuteAsync(
        ViewStudentSummaryInput input,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(input.StudentNumber))
        {
            return null;
        }

        var student = await _gateway.GetByStudentNumberAsync(
            input.StudentNumber.Trim(),
            cancellationToken);

        return student is null
            ? null
            : new StudentSummaryOutput(
                student.StudentNumber,
                student.FullName);
    }
}
