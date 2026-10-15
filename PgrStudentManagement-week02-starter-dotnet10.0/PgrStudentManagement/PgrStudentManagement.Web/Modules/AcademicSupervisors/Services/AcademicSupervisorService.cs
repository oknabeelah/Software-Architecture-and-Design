using PgrAcademicSupervisorManagement.Web.Models;
using PgrAcademicSupervisorManagement.Web.Data;
using PgrStudentManagement.Web.Modules.Students.Contracts;

namespace PgrAcademicSupervisorManagement.Web.Services;

// The service should coordinate:
// Supervisor retrieval and ordering 
// Supervisory-assignment creation 
// Eligibility and assignment business rules 

public class AcademicSupervisorService
{
    private readonly AcademicSupervisorStorage _academicSupervisorStorage;

    private readonly IStudentLookup _studentLookup;
    public AcademicSupervisorService(AcademicSupervisorStorage academicSupervisorStorage, IStudentLookup studentLookup)
    {
        _academicSupervisorStorage = academicSupervisorStorage;
        _studentLookup = studentLookup;
    }

    public IEnumerable<AcademicSupervisor> GetAllSupervisors()
    {
        var supervisors = _academicSupervisorStorage.GetAcademicSupervisors();
        if (supervisors == null || !supervisors.Any())
        {
            throw new InvalidOperationException("No supervisors found.");
        }
        return supervisors;
    }

    public AcademicSupervisor GetSupervisorByStaffNumber(string staffNumber)
    {
        var supervisor = _academicSupervisorStorage.GetAcademicSupervisor(staffNumber);
        if (supervisor == null)
        {
            throw new KeyNotFoundException($"Supervisor with staff number '{staffNumber}' not found.");
        }

        return supervisor;
    }

    public void CreateSupervisor(AcademicSupervisor supervisor)
    {
        if (supervisor == null)
        {
            throw new ArgumentNullException(nameof(supervisor), "Supervisor cannot be null.");
        }

        // Business rule: Check if a supervisor with the same staff number already exists
        var existingSupervisor = _academicSupervisorStorage.GetAcademicSupervisor(supervisor.StaffNumber);
        if (existingSupervisor != null)
        {
            throw new InvalidOperationException($"A supervisor with staff number '{supervisor.StaffNumber}' already exists.");
        }

        // Business rule: check that the staff number has the necessary prefix to signify employment type "IE" for institution-employed, "E" for external
        if (supervisor.EmploymentType == SupervisorEmploymentType.InstitutionEmployed)
        {
            if (!supervisor.StaffNumber.StartsWith("IE"))
            {
                if (char.IsDigit(supervisor.StaffNumber[0]))
                {
                    supervisor.StaffNumber = "IE" + supervisor.StaffNumber;
                }
                else
                {
                    throw new InvalidOperationException($"Invalid staff number format for institution-employed supervisor '{supervisor.StaffNumber}'.");
                }
            }
        }
        else if (supervisor.EmploymentType == SupervisorEmploymentType.External)
        {
            if (!supervisor.StaffNumber.StartsWith("E"))
            {
                if (char.IsDigit(supervisor.StaffNumber[0]))
                {
                    supervisor.StaffNumber = "E" + supervisor.StaffNumber;
                }
                else
                {
                    throw new InvalidOperationException($"Invalid staff number format for external supervisor '{supervisor.StaffNumber}'.");
                }
            }
        }
        else
        {
            throw new InvalidOperationException($"Invalid employment type for supervisor '{supervisor.StaffNumber}'.");
        }

        // Business rule: use the employment type to determine eligibility roles
        if (supervisor.EmploymentType == SupervisorEmploymentType.InstitutionEmployed)
        {
            supervisor.EligibleRoles = new HashSet<SupervisoryRole> 
            { SupervisoryRole.DirectorOfStudies, 
                SupervisoryRole.FirstSupervisor, 
                SupervisoryRole.SecondSupervisor, 
                SupervisoryRole.Adviser };
        }
        else if (supervisor.EmploymentType == SupervisorEmploymentType.External)
        {
            supervisor.EligibleRoles = new HashSet<SupervisoryRole> 
            { SupervisoryRole.SecondSupervisor, 
                SupervisoryRole.Adviser };
        }
        else
        {
            throw new InvalidOperationException($"Invalid employment type for supervisor '{supervisor.StaffNumber}'.");
        }

        _academicSupervisorStorage.AddAcademicSupervisor(supervisor);
    }

    public IReadOnlyList<SupervisoryAssignment> GetSupervisoryTeam(string studentNumber)
    {
        // Business rule: check that the student exists in the Students module
        var studentSummary = _studentLookup.GetStudentSummary(studentNumber);
        if (studentSummary == null)
        {
            throw new KeyNotFoundException($"Student with student number '{studentNumber}' not found.");
        }
        var supervisors = _academicSupervisorStorage.GetAssignmentsForStudent(studentSummary.StudentNumber);
        if (supervisors == null || !supervisors.Any())
        {
            throw new InvalidOperationException($"No supervisors found assigned to this student.");
        }
        return supervisors;
    }

    public IReadOnlyList<SupervisoryAssignment> GetSupervisorAssignments(string staffNumber)
    {
        var assignments = _academicSupervisorStorage.GetAssignmentsForSupervisor(staffNumber);
        if (assignments == null || !assignments.Any())
        {
            throw new InvalidOperationException($"No assignments found for supervisor with staff number '{staffNumber}'.");
        }
        return assignments;
    }

    public void AddSupervisoryAssignment(SupervisoryAssignment assignment) //TODO: use ILookup
    {
        if (assignment == null)
        {
            throw new ArgumentNullException(nameof(assignment), "Assignment cannot be null.");
        }        

        var existingAssignments = _academicSupervisorStorage.GetAssignmentsForStudent(assignment.StudentNumber);
        var existingAssignment = existingAssignments.FirstOrDefault(a => a.AcademicSupervisorNumber == assignment.AcademicSupervisorNumber && a.Role == assignment.Role && a.Current);
        if (existingAssignment != null) // duplicate assignment ie same student, same supervisor, same role and is current
        {
            throw new InvalidOperationException($"This is a duplicate assignment");
        }
        else if (existingAssignments.Count(a => a.Current) > 3 && !assignment.ExceptionAuthorised) // if there are more than 3 current assignments and the new assignment is not authorised as an exception
        {
            throw new InvalidOperationException($"Cannot add assignment for student as they already have 3 current assignments and the new assignment is not authorised as an exception.");
        }
        else if ( (assignment.Role == SupervisoryRole.DirectorOfStudies || assignment.Role == SupervisoryRole.FirstSupervisor) && existingAssignments.Any(a => a.Current && (a.Role == SupervisoryRole.DirectorOfStudies || a.Role == SupervisoryRole.FirstSupervisor))) // if the new assignment would result in more than one current Director of Studies or First Supervisor
        {
            throw new InvalidOperationException($"An assignment for student '{assignment.StudentNumber}' with supervisor '{assignment.AcademicSupervisorNumber}' as '{assignment.Role}' already exists.");
        }

        _academicSupervisorStorage.AddSupervisoryAssignment(assignment);
    }

// TODO: REFACTOR Update Assignment
    public void UpdateSupervisoryAssignment(SupervisoryAssignment assignment) // use ILookup // needs refactoring to allow input - a list of fields to update
    {
        //Controller
        //     receives UpdateSupervisoryAssignmentRequest

        // AcademicSupervisorService
        //     finds assignment by Id
        //     applies only supplied fields
        //     validates resulting assignment
        //     sends it to storage

        // AcademicSupervisorStorage
        //     replaces or updates the stored assignment with the updated assignment

        if (assignment == null)
        {
            throw new ArgumentNullException(nameof(assignment), "Assignment cannot be null.");
        }

        var existingAssignments = _academicSupervisorStorage.GetAssignmentsForStudent(assignment.StudentNumber); // one of which is the one we are replacing
        var existingAssignment = existingAssignments.FirstOrDefault(a => a.AcademicSupervisorNumber == assignment.AcademicSupervisorNumber && a.Role == assignment.Role && a.Current);
        
        if (existingAssignment != null) // if the updated assignment would create another duplicate assignment
        {
            throw new InvalidOperationException($"This is a duplicate assignment");
        }
        // else if (existingAssignments.Count(a => a.Current) == 3 && !assignment.ExceptionAuthorised) // if the updated assignment would result in more than 3 current assignments and the new assignment is not authorised as an exception
        // {
        //     throw new InvalidOperationException($"Cannot update assignment for student as they already have 3 current assignments and the updated assignment is not authorised as an exception.");
        // } // not a relevant scenario for updating an assignment, as the existing assignment is being replaced, so the count of current assignments will not increase
        else if ( (assignment.Role == SupervisoryRole.DirectorOfStudies || assignment.Role == SupervisoryRole.FirstSupervisor) && (existingAssignments.Count(a => a.Current && (a.Role == SupervisoryRole.DirectorOfStudies || a.Role == SupervisoryRole.FirstSupervisor)) <= 2)) // if the updated assignment would result in more than one current Director of Studies or First Supervisor
        {
            throw new InvalidOperationException($"An assignment for student '{assignment.StudentNumber}' with supervisor '{assignment.AcademicSupervisorNumber}' as '{assignment.Role}' already exists.");
        }

        _academicSupervisorStorage.UpdateSupervisoryAssignment(assignment);
    }

    public void AssignSupervisorToStudent(SupervisoryAssignment assignment)
    {
        var studentSummary = _studentLookup.GetStudentSummary(assignment.StudentNumber);
        if (studentSummary is null)
        {
            throw new InvalidOperationException($"Student '{assignment.StudentNumber}' not found.");
        }

        var supervisor = _academicSupervisorStorage.GetAcademicSupervisor(assignment.AcademicSupervisorNumber);
        if (supervisor is null)
        {
            throw new InvalidOperationException($"Supervisor '{assignment.AcademicSupervisorNumber}' not found.");
        }

        var supervisoryTeam = _academicSupervisorStorage.GetAssignmentsForStudent(assignment.StudentNumber);
        if (supervisoryTeam.Count(a => a.Current) >= 3 && !assignment.ExceptionAuthorised) // if the student already has 3 current assignments and the new assignment is not authorised as an exception
        {
            throw new InvalidOperationException($"Cannot assign supervisor '{assignment.AcademicSupervisorNumber}' to student '{assignment.StudentNumber}' as they already have 3 current assignments and the new assignment is not authorised as an exception.");
        }
        else if (supervisoryTeam.Contains(assignment)) // duplicate assignment
        {
            throw new InvalidOperationException($"Cannot assign supervisor '{assignment.AcademicSupervisorNumber}' to student '{assignment.StudentNumber}' as this is a duplicate assignment.");
        }
        else if ((assignment.Role == SupervisoryRole.DirectorOfStudies || assignment.Role == SupervisoryRole.FirstSupervisor) && supervisoryTeam.Any(a => a.Current && (a.Role == SupervisoryRole.DirectorOfStudies || a.Role == SupervisoryRole.FirstSupervisor))) // if the new assignment would result in more than one current Director of Studies or First Supervisor
        {
            throw new InvalidOperationException($"Cannot assign supervisor '{assignment.AcademicSupervisorNumber}' to student '{assignment.StudentNumber}' as they already have a current {assignment.Role}.");
        }
        else if (!supervisor.EligibleRoles.Contains(assignment.Role) && !assignment.ExceptionAuthorised) // if the supervisor is not eligible for the role and the assignment is not authorised as an exception
        {
            throw new InvalidOperationException($"Cannot assign supervisor '{assignment.AcademicSupervisorNumber}' to student '{assignment.StudentNumber}' as they are not eligible for the role '{assignment.Role}' and the assignment is not authorised as an exception.");
        }

        _academicSupervisorStorage.AddSupervisoryAssignment(assignment);
    }

}


