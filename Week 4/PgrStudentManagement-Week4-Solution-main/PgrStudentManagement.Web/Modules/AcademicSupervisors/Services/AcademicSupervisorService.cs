using PgrStudentManagement.Web.Modules.AcademicSupervisors.Data;
using PgrStudentManagement.Web.Modules.AcademicSupervisors.Models;
using PgrStudentManagement.Web.Modules.Students.Contracts;

namespace PgrStudentManagement.Web.Modules.AcademicSupervisors.Services;

public class AcademicSupervisorService
{
    private readonly AcademicSupervisorStorage _supervisorStorage;
    private readonly IStudentLookup _studentLookup;

    public AcademicSupervisorService(
        AcademicSupervisorStorage supervisorStorage,
        IStudentLookup studentLookup)
    {
        _supervisorStorage = supervisorStorage;
        _studentLookup = studentLookup;
    }

    public IReadOnlyList<AcademicSupervisor> GetSupervisors()
    {
        return _supervisorStorage
            .GetAcademicSupervisors()
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ThenBy(s => s.StaffNumber)
            .ToList();
    }

    public AcademicSupervisor? GetSupervisor(string staffNumber)
    {
        return _supervisorStorage.GetAcademicSupervisor(staffNumber);
    }

    public StudentSummary? GetStudentSummary(string studentNumber)
    {
        return _studentLookup.GetStudentSummary(studentNumber);
    }

    public (bool Succeeded, string? ErrorMessage, AcademicSupervisor? Supervisor) CreateSupervisor(AcademicSupervisor supervisor)
    {
        if (string.IsNullOrWhiteSpace(supervisor.StaffNumber))
        {
            return (false, "Staff number is required", null);
        }

        if (_supervisorStorage.StaffNumberExists(supervisor.StaffNumber))
        {
            return (false, "An Academic Supervisor with this staff number already exists", null);
        }

        // Set default role eligibility based on employment type and training if none specified
        if (supervisor.EligibleRoles.Count == 0)
        {
            supervisor.EligibleRoles.Add(SupervisoryRole.SecondSupervisor);
            supervisor.EligibleRoles.Add(SupervisoryRole.Adviser);

            if (supervisor.EmploymentType == SupervisorEmploymentType.InstitutionEmployed && supervisor.TrainingCompleted)
            {
                supervisor.EligibleRoles.Add(SupervisoryRole.DirectorOfStudies);
                supervisor.EligibleRoles.Add(SupervisoryRole.FirstSupervisor);
            }
        }

        _supervisorStorage.AddAcademicSupervisor(supervisor);

        return (true, null, supervisor);
    }

    public IReadOnlyList<SupervisoryAssignment> GetAssignmentsForStudent(string studentNumber)
    {
        return _supervisorStorage.GetAssignmentsForStudent(studentNumber)
            .OrderByDescending(a => a.Current)
            .ThenBy(a => a.Role)
            .ThenBy(a => a.StartDate)
            .ToList();
    }

    public (AssignmentOutcome Outcome, string? ErrorMessage) AssignSupervisor(
        string studentNumber,
        string staffNumber,
        SupervisoryRole role,
        DateTime startDate,
        bool exceptionAuthorised)
    {
        var student = _studentLookup.GetStudentSummary(studentNumber);
        if (student is null)
        {
            return (AssignmentOutcome.StudentNotFound, "Student not found");
        }

        var supervisor = _supervisorStorage.GetAcademicSupervisor(staffNumber);
        if (supervisor is null)
        {
            return (AssignmentOutcome.SupervisorNotFound, "Academic Supervisor not found");
        }

        // BR13: External supervisor restriction
        if (supervisor.EmploymentType == SupervisorEmploymentType.External &&
            (role == SupervisoryRole.DirectorOfStudies || role == SupervisoryRole.FirstSupervisor))
        {
            return (AssignmentOutcome.EmploymentTypeNotPermitted,
                "An external supervisor cannot be assigned as Director of Studies or First Supervisor.");
        }

        // Role eligibility check
        if (!supervisor.EligibleRoles.Contains(role))
        {
            return (AssignmentOutcome.SupervisorNotEligible,
                $"The supervisor is not currently eligible for the role of {role}.");
        }

        var currentAssignments = _supervisorStorage
            .GetAssignmentsForStudent(studentNumber)
            .Where(a => a.Current)
            .ToList();

        // BR15: Duplicate active assignment check
        if (currentAssignments.Any(a =>
            string.Equals(a.SupervisorStaffNumber, staffNumber, StringComparison.OrdinalIgnoreCase) &&
            a.Role == role))
        {
            return (AssignmentOutcome.DuplicateAssignment,
                "This Academic Supervisor is already assigned to the student in this role.");
        }

        // BR14: Unique current Director of Studies / First Supervisor check
        if ((role == SupervisoryRole.DirectorOfStudies || role == SupervisoryRole.FirstSupervisor) &&
            currentAssignments.Any(a => a.Role == role))
        {
            return (AssignmentOutcome.RoleAlreadyFilled,
                $"The student already has a current {role}.");
        }

        // BR02 / BR03: Maximum 3 supervisors without authorised exception
        if (currentAssignments.Count >= 3 && !exceptionAuthorised)
        {
            return (AssignmentOutcome.ExceptionRequired,
                "A fourth supervisor requires an authorised exception.");
        }

        var assignment = new SupervisoryAssignment
        {
            StudentNumber = studentNumber,
            SupervisorStaffNumber = staffNumber,
            Role = role,
            StartDate = startDate,
            EndDate = null,
            ExceptionAuthorised = exceptionAuthorised
        };

        _supervisorStorage.AddSupervisoryAssignment(assignment);

        return (AssignmentOutcome.Created, null);
    }

    public SupervisoryTeamAssessment EvaluateSupervisoryTeam(string studentNumber)
    {
        var currentAssignments = _supervisorStorage
            .GetAssignmentsForStudent(studentNumber)
            .Where(a => a.Current)
            .ToList();

        var issues = new List<string>();

        var dosAssignment = currentAssignments.FirstOrDefault(a => a.Role == SupervisoryRole.DirectorOfStudies);
        if (dosAssignment is null)
        {
            issues.Add("Missing required Director of Studies.");
        }
        else
        {
            var dosSupervisor = _supervisorStorage.GetAcademicSupervisor(dosAssignment.SupervisorStaffNumber);
            if (dosSupervisor?.EmploymentType != SupervisorEmploymentType.InstitutionEmployed)
            {
                issues.Add("Director of Studies must be institution-employed.");
            }
        }

        var firstSupervisorAssignment = currentAssignments.FirstOrDefault(a => a.Role == SupervisoryRole.FirstSupervisor);
        if (firstSupervisorAssignment is null)
        {
            issues.Add("Missing required First Supervisor.");
        }
        else
        {
            var firstSupervisor = _supervisorStorage.GetAcademicSupervisor(firstSupervisorAssignment.SupervisorStaffNumber);
            if (firstSupervisor?.EmploymentType != SupervisorEmploymentType.InstitutionEmployed)
            {
                issues.Add("First Supervisor must be institution-employed.");
            }
        }

        if (currentAssignments.Count < 2)
        {
            issues.Add("A completed supervisory arrangement normally contains at least two supervisors.");
        }
        else if (currentAssignments.Count > 3)
        {
            var hasUnauthorisedFourth = currentAssignments.Skip(3).Any(a => !a.ExceptionAuthorised);
            if (hasUnauthorisedFourth)
            {
                issues.Add("A fourth supervisor requires an authorised exception.");
            }
        }

        return new SupervisoryTeamAssessment(
            IsComplete: issues.Count == 0,
            Issues: issues.AsReadOnly());
    }
}
