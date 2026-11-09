using PgrStudentManagement.Adapters.API.Responses;
using PgrStudentManagement.Business.Students.ViewStudentSummary;

namespace PgrStudentManagement.Adapters.API.Mappers;

public sealed class StudentLookupApiMapper
{
    public StudentLookupResponse ToResponse(StudentSummaryOutput output) =>
        new(output.StudentNumber, output.DisplayName);
}
