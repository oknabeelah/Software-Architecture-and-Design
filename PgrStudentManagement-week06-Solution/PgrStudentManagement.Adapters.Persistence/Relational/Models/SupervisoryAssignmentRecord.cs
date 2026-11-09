using PgrStudentManagement.Domain.AcademicSupervisors;

namespace PgrStudentManagement.Adapters.Persistence.Relational.Models;

public sealed class SupervisoryAssignmentRecord
{
    public string StudentNumber { get; set; } = string.Empty;
    public string SupervisorStaffNumber { get; set; } = string.Empty;
    public SupervisoryRole Role { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool ExceptionAuthorised { get; set; }

    // navigation property to the StudentRecord
    public StudentRecord Student { get; set; } = null!;

    // navigation property to the AcademicSupervisorRecord
    public AcademicSupervisorRecord Supervisor { get; set; } = null!;
}
