using PgrAcademicSupervisorManagement.Web.Models;

namespace PgrAcademicSupervisorManagement.Web.Data;

public class AcademicSupervisorStorage
{
    // in memory data class owned by the AcademicSupervisorStorage module
    // separare collections for Academic supervisors and Supervisory Assignments
    private readonly List<AcademicSupervisor> _academicSupervisors = new()
    {
        //IE = Institution Employed, E = External
        new AcademicSupervisor
        {
            StaffNumber = "IE001",
            FirstName = "Alice",
            LastName = "Johnson",
            EmailAddress = "alice.johnson@university.edu.co.uk",
            EmploymentType = SupervisorEmploymentType.InstitutionEmployed,
            TrainingCompleted = true,
            EligibleRoles = new HashSet<SupervisoryRole> 
            { 
                SupervisoryRole.DirectorOfStudies, 
                SupervisoryRole.FirstSupervisor, 
                SupervisoryRole.SecondSupervisor, 
                SupervisoryRole.Adviser 
            }
        },
        new AcademicSupervisor
        {
            StaffNumber = "E002",
            FirstName = "Sam",
            LastName = "Parkson",
            EmailAddress = "sam.parkson@university.edu.external.co.uk",
            EmploymentType = SupervisorEmploymentType.External,
            TrainingCompleted = true,
            EligibleRoles = new HashSet<SupervisoryRole> 
            { 
                SupervisoryRole.SecondSupervisor 
            }
        },
        new AcademicSupervisor
        {
            StaffNumber = "E003",
            FirstName = "John",
            LastName = "Smith",
            EmailAddress = "john.smith@university.edu.external.co.uk",
            EmploymentType = SupervisorEmploymentType.External,
            TrainingCompleted = true,
            EligibleRoles = new HashSet<SupervisoryRole> 
            { 
                SupervisoryRole.SecondSupervisor, 
                SupervisoryRole.Adviser 
            }
        },
        new AcademicSupervisor
        {
            StaffNumber = "IE002",
            FirstName = "Sam",
            LastName = "Parkson",
            EmailAddress = "sam.parkson@university.edu.co.uk",
            EmploymentType = SupervisorEmploymentType.InstitutionEmployed,
            TrainingCompleted = true,
            EligibleRoles = new HashSet<SupervisoryRole> 
            { 
                SupervisoryRole.SecondSupervisor, 
                SupervisoryRole.Adviser 
            }
        }
    };
    private readonly List<SupervisoryAssignment> _supervisoryAssignments = new()
    {
        new SupervisoryAssignment
        {
            Role = SupervisoryRole.DirectorOfStudies,
            StartDate = new DateTime(2023, 1, 1),
            //Property or indexer 'SupervisoryAssignment.Current' cannot be assigned to -- it is read only
            ExceptionAuthorised = false
        },
        new SupervisoryAssignment
        {
            StudentNumber = "S100001",
            AcademicSupervisorNumber = "IE001",
            Role = SupervisoryRole.DirectorOfStudies,
            StartDate = new DateTime(2023, 1, 1),
            ExceptionAuthorised = false
        },
        new SupervisoryAssignment
        {
            StudentNumber = "S100002",
            AcademicSupervisorNumber = "E003",
            Role = SupervisoryRole.Adviser,
            StartDate = new DateTime(2023, 1, 1),
            ExceptionAuthorised = false
        },
        new SupervisoryAssignment
        {
            StudentNumber = "S100001",
            AcademicSupervisorNumber = "IE001",
            Role = SupervisoryRole.FirstSupervisor,
            StartDate = new DateTime(2023, 1, 1),
            ExceptionAuthorised = false
        },
        new SupervisoryAssignment
        {
            StudentNumber = "S100002",
            AcademicSupervisorNumber = "E003",
            Role = SupervisoryRole.SecondSupervisor,
            StartDate = new DateTime(2023, 1, 1),
            ExceptionAuthorised = false
        }
    };

    public IReadOnlyList<AcademicSupervisor> GetAcademicSupervisors()
    {
        return _academicSupervisors.AsReadOnly();
    }

    public AcademicSupervisor? GetAcademicSupervisor(string staffNumber)
    {
        return _academicSupervisors.FirstOrDefault(s => s.StaffNumber == staffNumber);
    }

    public void AddAcademicSupervisor(AcademicSupervisor supervisor)
    {
        _academicSupervisors.Add(supervisor);
    }

    public IReadOnlyList<SupervisoryAssignment> GetAssignmentsForStudent(string studentNumber)
    {
        return _supervisoryAssignments.Where(a => a.StudentNumber == studentNumber).ToList().AsReadOnly();
    }

    public IReadOnlyList<SupervisoryAssignment> GetAssignmentsForSupervisor(string staffNumber)
    {
        return _supervisoryAssignments.Where(a => a.AcademicSupervisorNumber == staffNumber).ToList().AsReadOnly();
    }

    public void AddSupervisoryAssignment(SupervisoryAssignment assignment)
    {
        _supervisoryAssignments.Add(assignment);
    }

    public void UpdateSupervisoryAssignment(SupervisoryAssignment assignment)
    {
        var existingAssignment = _supervisoryAssignments.FirstOrDefault(a => a.StudentNumber == assignment.StudentNumber && a.Role == assignment.Role);
        existingAssignment = assignment;
    }
}

// the storage class must not:
// - select Razor views
// - return redirects
// - apply supervisory business rules
// - access the student module's collection directly