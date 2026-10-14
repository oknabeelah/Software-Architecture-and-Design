using PgrAcademicSupervisorManagement.Web.Models;
using PgrAcademicSupervisorManagement.Web.Data;

namespace PgrAcademicSupervisorManagement.Web.Services;

// The service should coordinate:
// Academic Supervisor creation
// Supervisor retrieval and ordering
// Supervisory-assignment creation
// Supervisory-team retrieval
// Eligibility and assignment business rules

// Inject the Academic Supervisor storage dependency through the constructor. x
// The service may also depend on an explicit Students-module operation or contract for student lookup.
// Do not inject StudentStorage directly into the Academic Supervisor service. Other modules should not depend on the
// internal data implementation of the Students module.
public class AcademicSupervisorService
{
    private readonly AcademicSupervisorStorage _academicSupervisorStorage;
    public AcademicSupervisorService(AcademicSupervisorStorage academicSupervisorStorage)
    {
        _academicSupervisorStorage = academicSupervisorStorage;
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
        var supervisors = _academicSupervisorStorage.GetAssignmentsForStudent(studentNumber);
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

    public void AddSupervisoryAssignment(SupervisoryAssignment assignment)
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
    public void UpdateSupervisoryAssignment(SupervisoryAssignment assignment) // needs refactoring to allow input - a list of fields to update
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


}


