namespace PgrStudentManagement.Domain.Students;

public class Student
{
    public string StudentNumber { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string EmailAddress { get; init; } = string.Empty;
    public ICollection<Enrolment> Enrolments { get; init; } = [];
    public string? Course { get; init; }
    public string? ModeOfStudy { get; init; }
    public DateTime? StartDate { get; init; }
    public string? ThesisTitle { get; init; }
    public DateTime? ExpectedThesisSubmissionDate { get; init; }
    public DateTime? ActualThesisSubmissionDate { get; init; }
}