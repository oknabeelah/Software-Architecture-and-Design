using PgrStudentManagement.Web.Modules.AcademicSupervisors.Models;

namespace PgrStudentManagement.Web.Modules.AcademicSupervisors.Data;

public class AcademicSupervisorStorage
{
    private readonly List<AcademicSupervisor> _academicSupervisors =
    [
        new AcademicSupervisor
        {
            StaffNumber = "STAFF001",
            FirstName = "Alan",
            LastName = "Turing",
            EmailAddress = "alan.turing@university.ac.uk",
            EmploymentType = SupervisorEmploymentType.InstitutionEmployed,
            TrainingCompleted = true,
            EligibleRoles =
            [
                SupervisoryRole.DirectorOfStudies,
                SupervisoryRole.FirstSupervisor,
                SupervisoryRole.SecondSupervisor,
                SupervisoryRole.Adviser
            ]
        },
        new AcademicSupervisor
        {
            StaffNumber = "STAFF002",
            FirstName = "Grace",
            LastName = "Hopper",
            EmailAddress = "grace.hopper@university.ac.uk",
            EmploymentType = SupervisorEmploymentType.InstitutionEmployed,
            TrainingCompleted = true,
            EligibleRoles =
            [
                SupervisoryRole.DirectorOfStudies,
                SupervisoryRole.FirstSupervisor,
                SupervisoryRole.SecondSupervisor,
                SupervisoryRole.Adviser
            ]
        },
        new AcademicSupervisor
        {
            StaffNumber = "STAFF003",
            FirstName = "Ada",
            LastName = "Lovelace",
            EmailAddress = "ada.lovelace@external-partner.org",
            EmploymentType = SupervisorEmploymentType.External,
            TrainingCompleted = true,
            EligibleRoles =
            [
                SupervisoryRole.SecondSupervisor,
                SupervisoryRole.Adviser
            ]
        },
        new AcademicSupervisor
        {
            StaffNumber = "STAFF004",
            FirstName = "John",
            LastName = "von Neumann",
            EmailAddress = "john.vonneumann@university.ac.uk",
            EmploymentType = SupervisorEmploymentType.InstitutionEmployed,
            TrainingCompleted = false,
            EligibleRoles =
            [
                SupervisoryRole.SecondSupervisor,
                SupervisoryRole.Adviser
            ]
        }
    ];

    private readonly List<SupervisoryAssignment> _supervisoryAssignments =
    [
        new SupervisoryAssignment
        {
            StudentNumber = "S100001",
            SupervisorStaffNumber = "STAFF001",
            Role = SupervisoryRole.DirectorOfStudies,
            StartDate = new DateTime(2025, 10, 1),
            EndDate = null,
            ExceptionAuthorised = false
        },
        new SupervisoryAssignment
        {
            StudentNumber = "S100001",
            SupervisorStaffNumber = "STAFF002",
            Role = SupervisoryRole.FirstSupervisor,
            StartDate = new DateTime(2025, 10, 1),
            EndDate = null,
            ExceptionAuthorised = false
        }
    ];

    public IReadOnlyList<AcademicSupervisor> GetAcademicSupervisors()
    {
        return _academicSupervisors.AsReadOnly();
    }

    public AcademicSupervisor? GetAcademicSupervisor(string staffNumber)
    {
        return _academicSupervisors.FirstOrDefault(
            s => string.Equals(s.StaffNumber, staffNumber, StringComparison.OrdinalIgnoreCase));
    }

    public bool StaffNumberExists(string staffNumber)
    {
        return _academicSupervisors.Any(
            s => string.Equals(s.StaffNumber, staffNumber, StringComparison.OrdinalIgnoreCase));
    }

    public void AddAcademicSupervisor(AcademicSupervisor supervisor)
    {
        _academicSupervisors.Add(supervisor);
    }

    public void UpdateAcademicSupervisor(AcademicSupervisor supervisor)
    {
        var index = _academicSupervisors.FindIndex(
            s => string.Equals(s.StaffNumber, supervisor.StaffNumber, StringComparison.OrdinalIgnoreCase));

        if (index >= 0)
        {
            _academicSupervisors[index] = supervisor;
        }
    }

    public IReadOnlyList<SupervisoryAssignment> GetAssignmentsForStudent(string studentNumber)
    {
        return _supervisoryAssignments
            .Where(a => string.Equals(a.StudentNumber, studentNumber, StringComparison.OrdinalIgnoreCase))
            .ToList()
            .AsReadOnly();
    }

    public IReadOnlyList<SupervisoryAssignment> GetAssignmentsForSupervisor(string staffNumber)
    {
        return _supervisoryAssignments
            .Where(a => string.Equals(a.SupervisorStaffNumber, staffNumber, StringComparison.OrdinalIgnoreCase))
            .ToList()
            .AsReadOnly();
    }

    public void AddSupervisoryAssignment(SupervisoryAssignment assignment)
    {
        _supervisoryAssignments.Add(assignment);
    }

    public void UpdateSupervisoryAssignment(SupervisoryAssignment assignment)
    {
        var index = _supervisoryAssignments.FindIndex(
            a => string.Equals(a.StudentNumber, assignment.StudentNumber, StringComparison.OrdinalIgnoreCase) &&
                 string.Equals(a.SupervisorStaffNumber, assignment.SupervisorStaffNumber, StringComparison.OrdinalIgnoreCase) &&
                 a.Role == assignment.Role &&
                 a.StartDate == assignment.StartDate);

        if (index >= 0)
        {
            _supervisoryAssignments[index] = assignment;
        }
    }
}
