using PgrStudentManagement.Adapters.API.Requests;
using PgrStudentManagement.Adapters.API.Responses;
using PgrStudentManagement.Business.AcademicSupervisors.AcademicSupervisorAssignment;
using PgrStudentManagement.Domain.AcademicSupervisors;

namespace PgrStudentManagement.Adapters.API.Mappers;

public sealed class AcademicSupervisorApiMapper
{
    public AcademicSupervisorAssignmentInput? ToInput(
        string studentNumber,
        AssignAcademicSupervisorRequest request)
    {
        if (!Enum.TryParse<SupervisoryRole>(
                request.Role,
                ignoreCase: true,
                out var role))
        {
            return null;
        }

        return new AcademicSupervisorAssignmentInput(
            studentNumber.Trim(),
            request.SupervisorStaffNumber.Trim(),
            role,
            request.StartDate,
            request.ExceptionAuthorised);
    }

    public AssignAcademicSupervisorResponse ToResponse(
        AcademicSupervisorAssignmentResult result) =>
        new(
            result.Succeeded,
            result.Outcome.ToString(),
            result.AssignmentId);
}
