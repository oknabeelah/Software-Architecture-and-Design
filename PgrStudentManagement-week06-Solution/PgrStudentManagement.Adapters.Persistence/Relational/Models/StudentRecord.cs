namespace PgrStudentManagement.Adapters.Persistence.Relational.Models;

public class StudentRecord
{
    public string StudentNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;

    // navigation property to the EnrolmentRecord
    public ICollection<EnrolmentRecord> Enrolments { get; set; } = new List<EnrolmentRecord>(); 

    // navigation property to the SupervisoryAssignmentRecord
    public ICollection<SupervisoryAssignmentRecord> SupervisoryAssignments { get; set; } = [];
}
