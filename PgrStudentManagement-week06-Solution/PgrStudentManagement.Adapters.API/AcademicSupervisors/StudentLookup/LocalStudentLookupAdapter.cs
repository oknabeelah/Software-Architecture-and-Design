using PgrStudentManagement.Business.AcademicSupervisors.Gateway;
using PgrStudentManagement.Business.Students.ViewStudentSummary;

namespace PgrStudentManagement.Adapters.API.AcademicSupervisors.StudentLookup;

public sealed class LocalStudentLookupAdapter : IStudentLookup
{
    private readonly IViewStudentSummaryUseCase _studentSummaryUseCase;

    public LocalStudentLookupAdapter(
        IViewStudentSummaryUseCase studentSummaryUseCase)
    {
        _studentSummaryUseCase = studentSummaryUseCase;
    }

    public async Task<StudentSummary?> GetStudentSummaryAsync(
        string studentNumber,
        CancellationToken cancellationToken = default)
    {
        var output = await _studentSummaryUseCase.ExecuteAsync(
            new ViewStudentSummaryInput(studentNumber),
            cancellationToken);

        return output is null
            ? null
            : new StudentSummary(output.StudentNumber, output.DisplayName);
    }
}
