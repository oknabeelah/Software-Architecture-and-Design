using PgrStudentManagement.Domain.AcademicSupervisors;

namespace PgrStudentManagement.Adapters.Persistence.Relational.Models;

public sealed class AcademicSupervisorRecord
{
    public string StaffNumber { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public SupervisorEmploymentType EmploymentType { get; set; }
    public bool TrainingCompleted { get; set; }
    public ICollection<AcademicSupervisorEligibleRoleRecord> EligibleRoles { get; set; } = [];
    
    //navigation property to the AcademicSupervisorEligibleRoleRecord
    public ICollection<SupervisoryAssignmentRecord> Assignments { get; set; } = [];
}
