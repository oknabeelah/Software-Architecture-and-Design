using PgrStudentManagement.Domain.AcademicSupervisors;

namespace PgrStudentManagement.Adapters.Persistence.Relational.Models;

public sealed class AcademicSupervisorEligibleRoleRecord
{
    public string SupervisorStaffNumber { get; set; } = string.Empty;
    public SupervisoryRole Role { get; set; }
    
    //navigation property to the AcademicSupervisorRecord
    public AcademicSupervisorRecord Supervisor { get; set; } = null!; 
}
