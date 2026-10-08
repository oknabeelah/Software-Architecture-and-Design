namespace PgrStudentManagement.Web.Models;

public class Student
{
    // no model changes made becuase the model is already designed to support the requirements of the application
    public string StudentNumber { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}";

    public string? Course { get; set; }

    public string? ModeOfStudy { get; set; }

    public DateTime? StartDate { get; set; }

    public Status Status { get; set; }

    public string? ThesisTitle { get; set; }

    public DateTime? ExpectedThesisSubmissionDate { get; set; }

    public DateTime? ActualThesisSubmissionDate { get; set; }

    /*
     * DESIGN TASK:
     *
     * Decide how the model should represent:
     *   - Expected thesis submission date
     *   - Original expected thesis submission date
     *   - Actual thesis submission date
     *
     * Review the supplied domain model before adding or renaming fields.
     */

}