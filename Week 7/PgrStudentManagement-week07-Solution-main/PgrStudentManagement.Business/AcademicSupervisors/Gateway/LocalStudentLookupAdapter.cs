using PgrStudentManagement.Business.Students.GetStudentSummary;

namespace PgrStudentManagement.Business.AcademicSupervisors.Gateway;

public class LocalStudentLookupAdapter : IStudentLookup
{
    private readonly IGetStudentSummaryUseCase _getStudentSummaryUseCase;

    public LocalStudentLookupAdapter(IGetStudentSummaryUseCase getStudentSummaryUseCase)
    {
        _getStudentSummaryUseCase = getStudentSummaryUseCase;
    }

    public async Task<StudentSummary?> GetStudentSummaryAsync(
        string studentNumber,
        CancellationToken cancellationToken = default)
    {
        var output = await _getStudentSummaryUseCase.ExecuteAsync(
            new GetStudentSummaryInput(studentNumber),
            cancellationToken);

        if (!output.Found || output.DisplayName is null)
        {
            return null;
        }

        return new StudentSummary(
            output.StudentNumber,
            output.DisplayName);
    }
}

