using PgrStudentManagement.Adapters.API.Requests;
using PgrStudentManagement.Adapters.API.Responses;
using PgrStudentManagement.Business.AcademicSupervisors.AssignAcademicSupervisor;
using PgrStudentManagement.Business.AcademicSupervisors.ViewSupervisoryTeam;

namespace PgrStudentManagement.Adapters.API.Mappers;

public class AcademicSupervisorsApiMapper
{
    public AssignAcademicSupervisorInput ToInput(
        string studentNumber,
        AssignAcademicSupervisorRequest request)
    {
        var effectiveStudentNumber = !string.IsNullOrWhiteSpace(studentNumber)
            ? studentNumber.Trim()
            : request.StudentNumber?.Trim() ?? string.Empty;

        return new AssignAcademicSupervisorInput(
            StudentNumber: effectiveStudentNumber,
            StaffNumber: request.StaffNumber?.Trim() ?? string.Empty,
            Role: request.Role,
            StartDate: request.StartDate,
            ExceptionAuthorised: request.ExceptionAuthorised);
    }

    public AssignAcademicSupervisorResponse ToResponse(AssignAcademicSupervisorResult result)
    {
        var message = result.Outcome switch
        {
            AssignmentOutcome.Created =>
                "Academic Supervisor assigned successfully.",
            AssignmentOutcome.StudentNotFound =>
                "Student not found.",
            AssignmentOutcome.SupervisorNotFound =>
                "Academic Supervisor not found.",
            AssignmentOutcome.DuplicateAssignment =>
                "This current supervisory assignment already exists.",
            AssignmentOutcome.RoleAlreadyFilled =>
                "The selected supervisory role is already filled for this student.",
            AssignmentOutcome.SupervisorNotEligible =>
                "The selected Academic Supervisor is not eligible for this role.",
            AssignmentOutcome.EmploymentTypeNotPermitted =>
                "The supervisor's employment type is not permitted for the selected role.",
            AssignmentOutcome.ExceptionRequired =>
                "An authorised exception is required before a fourth supervisor can be assigned.",
            _ =>
                "The supervisory assignment could not be created."
        };

        SupervisoryAssignmentResponse? assignmentResponse = result.Assignment is null
            ? null
            : new SupervisoryAssignmentResponse(
                result.Assignment.StudentNumber,
                result.Assignment.SupervisorStaffNumber,
                result.Assignment.Role,
                result.Assignment.StartDate,
                result.Assignment.EndDate,
                result.Assignment.ExceptionAuthorised);

        return new AssignAcademicSupervisorResponse(
            result.Outcome.ToString(),
            message,
            assignmentResponse);
    }

    public SupervisoryTeamResponse? ToResponse(SupervisoryTeamOutput output)
    {
        if (!output.StudentFound || output.Student is null)
        {
            return null;
        }

        var current = output.CurrentAssignments
            .Select(ToMemberResponse)
            .ToList();

        var historical = output.HistoricalAssignments
            .Select(ToMemberResponse)
            .ToList();

        return new SupervisoryTeamResponse(
            output.Student.StudentNumber,
            output.Student.DisplayName,
            current,
            historical);
    }

    private static SupervisoryTeamMemberResponse ToMemberResponse(
        SupervisoryTeamMemberOutput member)
    {
        return new SupervisoryTeamMemberResponse(
            member.SupervisorStaffNumber,
            member.SupervisorName,
            member.EmploymentType.ToString(),
            member.Role.ToString(),
            member.StartDate,
            member.EndDate,
            member.Current);
    }
}

