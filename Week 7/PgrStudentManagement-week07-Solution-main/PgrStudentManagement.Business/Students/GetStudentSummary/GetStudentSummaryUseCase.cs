using PgrStudentManagement.Business.Students.Gateway;

namespace PgrStudentManagement.Business.Students.GetStudentSummary;

public class GetStudentSummaryUseCase : IGetStudentSummaryUseCase
{
    private readonly IStudentGateway _studentGateway;

    public GetStudentSummaryUseCase(IStudentGateway studentGateway)
    {
        _studentGateway = studentGateway;
    }

    public async Task<GetStudentSummaryOutput> ExecuteAsync(
        GetStudentSummaryInput input,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(input.StudentNumber))
        {
            return new GetStudentSummaryOutput(
                Found: false,
                StudentNumber: input.StudentNumber ?? string.Empty,
                DisplayName: null);
        }

        var trimmedNumber = input.StudentNumber.Trim();
        var student = await _studentGateway.GetByStudentNumberAsync(
            trimmedNumber,
            cancellationToken);

        if (student is null)
        {
            return new GetStudentSummaryOutput(
                Found: false,
                StudentNumber: trimmedNumber,
                DisplayName: null);
        }

        return new GetStudentSummaryOutput(
            Found: true,
            StudentNumber: student.StudentNumber,
            DisplayName: student.FullName);
    }
}

