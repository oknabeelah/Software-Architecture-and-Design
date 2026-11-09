using PgrStudentManagement.Business.AcademicSupervisors.Gateway;
using PgrStudentManagement.Domain.AcademicSupervisors;

namespace PgrStudentManagement.Business.AcademicSupervisors.AcademicSupervisorAssignment;

public sealed class AcademicSupervisorAssignmentUseCase: IAcademicSupervisorUseCase
{
    private readonly IAcademicSupervisorAssignmentGateway _gateway;
    private readonly IStudentLookup _studentLookup;

    public AcademicSupervisorAssignmentUseCase(
        IAcademicSupervisorAssignmentGateway gateway,
        IStudentLookup studentLookup)
    {
        _gateway = gateway;
        _studentLookup = studentLookup;
    }

    public async Task<AcademicSupervisorAssignmentResult> ExecuteAsync(AcademicSupervisorAssignmentInput input, CancellationToken cancellationToken)
    { // this is inherited from I...UseCase interface
        // validate input
        if (string.IsNullOrWhiteSpace(input.StudentNumber)) return new AcademicSupervisorAssignmentResult(AcademicSupervisorAssignmentOutcome.InvalidInput);
        if (string.IsNullOrWhiteSpace(input.SupervisorStaffNumber)) return new AcademicSupervisorAssignmentResult(AcademicSupervisorAssignmentOutcome.InvalidInput);
        if (!Enum.IsDefined(typeof(SupervisoryRole), input.Role))
        {
            return new AcademicSupervisorAssignmentResult(AcademicSupervisorAssignmentOutcome.InvalidInput);
        }

        // using IStudentlookup
        var student = await _studentLookup.GetStudentSummaryAsync(
            input.StudentNumber.Trim(),
            cancellationToken);
        if (student is null) return new AcademicSupervisorAssignmentResult(AcademicSupervisorAssignmentOutcome.StudentNotFound); // Student not found

        var supervisor = await _gateway.GetSupervisorAsync(input.SupervisorStaffNumber.Trim(),cancellationToken);
        if (supervisor is null) return new AcademicSupervisorAssignmentResult(AcademicSupervisorAssignmentOutcome.AcademicSupervisorNotFound); // Academic supervisor not found
       
        var assignments = (await _gateway.GetForStudentAsync(input.StudentNumber.Trim(), cancellationToken))
            .Where(assignment => assignment.Current)
            .ToList(); // return all current assignments for the student  //remember to show all assignments in student details regardless of completion status

        if (assignments.Any(assignment => string.Equals(assignment.SupervisorStaffNumber, input.SupervisorStaffNumber.Trim(), StringComparison.OrdinalIgnoreCase)
                        && assignment.Role == input.Role))
        {
            return new AcademicSupervisorAssignmentResult(AcademicSupervisorAssignmentOutcome.DuplicateAssignment); // Duplicate assignment
        }

        // below can be granted with an authorised exception
        if (!input.ExceptionAuthorised)
        {
            var roleMustBeUnique = input.Role == SupervisoryRole.DirectorOfStudies || input.Role == SupervisoryRole.FirstSupervisor;
            if (roleMustBeUnique && assignments.Any(assignment => assignment.Role == input.Role))
            {
                return new AcademicSupervisorAssignmentResult(AcademicSupervisorAssignmentOutcome.RoleAlreadyFilled); // Unique role already filled
            }

            if (!supervisor.IsEligibleFor(input.Role))
            {
                return new AcademicSupervisorAssignmentResult(AcademicSupervisorAssignmentOutcome.SupervisorNotEligible); // Supervisor not eligible for the role
            }

            var requiresInstitutionEmployee = input.Role == SupervisoryRole.DirectorOfStudies || input.Role == SupervisoryRole.FirstSupervisor;
            if (requiresInstitutionEmployee && supervisor.EmploymentType != SupervisorEmploymentType.InstitutionEmployed)
            {
                return new AcademicSupervisorAssignmentResult(AcademicSupervisorAssignmentOutcome.EmploymentTypeNotPermitted); // Employment type not permitted for the role
            }

            var currentSupervisorCount = assignments.Count(assignment => assignment.Role != SupervisoryRole.Adviser);
            if (currentSupervisorCount >= 3) return new AcademicSupervisorAssignmentResult(AcademicSupervisorAssignmentOutcome.MaximumSupervisorsReached); // Maximum supervisors reached
        }

        // create assignment
        var assignmentId = $"assignment-{Guid.NewGuid():N}"[..16];
        var assignment = new SupervisoryAssignment
        {
            //Id = assignmentId, // possible add to db
            StudentNumber = input.StudentNumber.Trim(),
            SupervisorStaffNumber = input.SupervisorStaffNumber.Trim(),
            Role = input.Role,
            StartDate = input.StartDate,
            ExceptionAuthorised = input.ExceptionAuthorised
        };

        await _gateway.AddAsync(assignment, cancellationToken);
        return new AcademicSupervisorAssignmentResult(AcademicSupervisorAssignmentOutcome.Created, assignmentId);
    }
}
// Ternary/Conditional operator is written as "condition ? valueIfTrue : valueIfFalse"
// Predicate -> Action is "if (condition) { action } else { action }" i.e an if statement
// Expression-bodied members are a shorthand syntax for defining methods, properties, etc
//  e.g public bool IsCurrent => EndDate == null;