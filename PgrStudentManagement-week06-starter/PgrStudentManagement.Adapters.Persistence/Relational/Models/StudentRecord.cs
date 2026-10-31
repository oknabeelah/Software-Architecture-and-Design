namespace PgrStudentManagement.Adapters.Persistence.Relational.Models;

public class StudentRecord
{
    public string StudentNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string? Course { get; set; }
    public string? ModeOfStudy { get; set; }
    public DateTime? StartDate { get; set; }
    public string? ThesisTitle { get; set; }
    public DateTime? ExpectedThesisSubmissionDate { get; set; }
    public DateTime? ActualThesisSubmissionDate { get; set; }
    public ICollection<EnrolmentRecord> Enrolments { get; set; } = [];
}
