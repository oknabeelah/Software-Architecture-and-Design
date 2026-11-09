using PgrStudentManagement.Business.AcademicSupervisors.Gateway;
using PgrStudentManagement.Domain.AcademicSupervisors;

namespace PgrStudentManagement.Business.AcademicSupervisors.AcademicSupervisorAssignment;

public class AcademicSupervisorAssignmentUseCase : IAcademicSupervisorUseCase
{
    private readonly IAcademicSupervisorAssignmentGateway _gateway;
    public AcademicSupervisorAssignmentUseCase(IAcademicSupervisorAssignmentGateway gateway)
    {
        _gateway = gateway;
    }

    public async Task<AcademicSupervisorAssignmentResult> ExecuteAsync(AcademicSupervisorAssignmentInput input, CancellationToken cancellationToken)
    { // this is inherited from I...UseCase interface
        if (string.IsNullOrWhiteSpace(input.StudentNumber)) return new AcademicSupervisorAssignmentResult(false, null, "Student number is required.");
        if (string.IsNullOrWhiteSpace(input.SupervisorStaffNumber)) return new AcademicSupervisorAssignmentResult(false, null, "Supervisor ID is required.");
        
        // TO DO use IStudentLookup and IStudentSummary to confirm the student exists
        //var student = await _studentLookup.GetStudentSummaryAsync(input.StudentNumber, cancellationToken);
        // if (student is null) return new AcademicSupervisorAssignmentResult(false, null, "The specified student does not exist.");
        
        //var assignments = await _gateway.GetAssignmentForStudentAsync(input.StudentNumber, cancellationToken); //return all assignments for StudentNumber

        //var duplicateAssignment = assignments.FirstOrDefault(a => a.Current && a.SupervisorStaffNumber == input.SupervisorStaffNumber && a.Role == input.Role);
        //if (duplicateAssignment is not null) return new AcademicSupervisorAssignmentResult(false, null, "The specified academic supervisor assignment already exists for this student.");

        //if (input.Role == SupervisoryRole.DirectorOfStudies || input.Role == SupervisoryRole.FirstSupervisor)
        //{
        //  var uniqueRole = assignments.Where(a => a.Current).Any(a => a.Role == input.Role)
        //  if (uniqueRole is not null && !input.ExceptionAuthorised) return new AcademicSupervisorAssignmentResult(false, null, "The specified academic supervisor role is already filled for this student, Consider getting an Authorised Exception."); 
        //}

        if (!Enum.IsDefined(typeof(SupervisoryRole), input.Role))
        {
            return new AcademicSupervisorAssignmentResult(false, null, "The selected academic supervisor role is not supported.");
        }

        var assignmentId = $"assignment-{Guid.NewGuid():N}"[..16];
        var assignment = new SupervisoryAssignment
        {
            //Id = assignmentId,
            StudentNumber = input.StudentNumber.Trim(),
            SupervisorStaffNumber = input.SupervisorStaffNumber.Trim(),
            Role = input.Role,
            StartDate = input.StartDate,
            ExceptionAuthorised = input.ExceptionAuthorised,
        };

        await _gateway.AddAsync(assignment, cancellationToken);
        return new AcademicSupervisorAssignmentResult(true, assignmentId);
    }
}
// Ternary/Conditional operator is written as "condition ? valueIfTrue : valueIfFalse"
// Predicate -> Action is "if (condition) { action } else { action }" i.e an if statement
// Expression-bodied members are a shorthand syntax for defining methods, properties, etc
//  e.g public bool IsCurrent => EndDate == null;