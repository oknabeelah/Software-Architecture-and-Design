using PgrStudentManagement.Adapters.API.Responses;
using PgrStudentManagement.Business.Students.GetStudentSummary;

namespace PgrStudentManagement.Adapters.API.Mappers;

public class StudentLookupApiMapper
{
    public GetStudentSummaryInput ToInput(string studentNumber)
    {
        return new GetStudentSummaryInput(studentNumber?.Trim() ?? string.Empty);
    }

    public StudentLookupResponse? ToResponse(GetStudentSummaryOutput output)
    {
        if (!output.Found || output.DisplayName is null)
        {
            return null;
        }

        return new StudentLookupResponse(
            output.StudentNumber,
            output.DisplayName);
    }
}

