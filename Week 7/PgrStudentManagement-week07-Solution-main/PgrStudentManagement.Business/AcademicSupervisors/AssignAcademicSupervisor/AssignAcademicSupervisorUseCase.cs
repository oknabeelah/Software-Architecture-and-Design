using PgrStudentManagement.Business.AcademicSupervisors.Gateway;
using PgrStudentManagement.Domain.AcademicSupervisors;

namespace PgrStudentManagement.Business.AcademicSupervisors.AssignAcademicSupervisor;

public class AssignAcademicSupervisorUseCase : IAssignAcademicSupervisorUseCase
{
    private readonly IAcademicSupervisorGateway _gateway;
    private readonly IStudentLookup _studentLookup;

    public AssignAcademicSupervisorUseCase(
        IAcademicSupervisorGateway gateway,
        IStudentLookup studentLookup)
    {
        _gateway = gateway;
        _studentLookup = studentLookup;
    }

    public async Task<AssignAcademicSupervisorResult> ExecuteAsync(
        AssignAcademicSupervisorInput input,
        CancellationToken cancellationToken = default)
    {
        var student = await _studentLookup.GetStudentSummaryAsync(
            input.StudentNumber,
            cancellationToken);

        if (student is null)
        {
            return new AssignAcademicSupervisorResult(AssignmentOutcome.StudentNotFound);
        }

        var supervisor = await _gateway.GetByStaffNumberAsync(
            input.StaffNumber,
            cancellationToken);

        if (supervisor is null)
        {
            return new AssignAcademicSupervisorResult(AssignmentOutcome.SupervisorNotFound);
        }

        var assignments = await _gateway.GetAssignmentsForStudentAsync(
            input.StudentNumber,
            cancellationToken);

        var currentAssignments = assignments
            .Where(assignment => assignment.Current)
            .ToList();

        var duplicateAssignment = currentAssignments.Any(assignment =>
            assignment.SupervisorStaffNumber == input.StaffNumber
            && assignment.Role == input.Role);

        if (duplicateAssignment)
        {
            return new AssignAcademicSupervisorResult(AssignmentOutcome.DuplicateAssignment);
        }

        var roleMustBeUnique =
            input.Role == SupervisoryRole.DirectorOfStudies
            || input.Role == SupervisoryRole.FirstSupervisor;

        if (roleMustBeUnique)
        {
            var roleAlreadyFilled = currentAssignments.Any(
                assignment => assignment.Role == input.Role);

            if (roleAlreadyFilled)
            {
                return new AssignAcademicSupervisorResult(AssignmentOutcome.RoleAlreadyFilled);
            }
        }

        if (!supervisor.IsEligibleFor(input.Role))
        {
            return new AssignAcademicSupervisorResult(AssignmentOutcome.SupervisorNotEligible);
        }

        var requiresInstitutionEmployee =
            input.Role == SupervisoryRole.DirectorOfStudies
            || input.Role == SupervisoryRole.FirstSupervisor;

        var hasPermittedEmploymentType =
            supervisor.EmploymentType == SupervisorEmploymentType.InstitutionEmployed;

        if (requiresInstitutionEmployee && !hasPermittedEmploymentType)
        {
            return new AssignAcademicSupervisorResult(AssignmentOutcome.EmploymentTypeNotPermitted);
        }

        var currentSupervisorCount = currentAssignments.Count(
            assignment => assignment.Role != SupervisoryRole.Adviser);

        var addingFourthSupervisor = currentSupervisorCount >= 3;

        if (addingFourthSupervisor && !input.ExceptionAuthorised)
        {
            return new AssignAcademicSupervisorResult(AssignmentOutcome.ExceptionRequired);
        }

        var assignment = new SupervisoryAssignment
        {
            StudentNumber = input.StudentNumber,
            SupervisorStaffNumber = input.StaffNumber,
            Role = input.Role,
            StartDate = input.StartDate.Date,
            EndDate = null,
            ExceptionAuthorised = input.ExceptionAuthorised
        };

        await _gateway.AddAssignmentAsync(assignment, cancellationToken);

        var output = new SupervisoryAssignmentOutput(
            assignment.StudentNumber,
            assignment.SupervisorStaffNumber,
            assignment.Role.ToString(),
            assignment.StartDate,
            assignment.EndDate,
            assignment.ExceptionAuthorised);

        return new AssignAcademicSupervisorResult(
            AssignmentOutcome.Created,
            output);
    }
}

