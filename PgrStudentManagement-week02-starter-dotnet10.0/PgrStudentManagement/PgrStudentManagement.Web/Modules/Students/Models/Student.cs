using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PgrStudentManagement.Web.Models;

[Index(nameof(EmailAddress), IsUnique = true)]
public class Student
{
    [Key]
    [MaxLength(20)]
    public string StudentNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}";

    [Required]
    [MaxLength(255)]
    public string EmailAddress { get; set; } = string.Empty;

    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();

    public string? Course { get; set; }

    public string? ModeOfStudy { get; set; }

    public DateTime? StartDate { get; set; }

    public Status Status { get; set; }

    public string? ThesisTitle { get; set; }

    public DateTime? ExpectedThesisSubmissionDate { get; set; }

    public DateTime? ActualThesisSubmissionDate { get; set; }

    // TODO: Answer the following questions in the comments below:
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

/*
    suspensionEndDate : Date
    feeStatus : String
    HoRD : String
    pgrt : String
    college : String
    departmentSchool : String
    rikeHub : String
    additionalTimeMonths : Integer
    visa : Boolean
    bursary : Boolean
    learningContract : Boolean
    thesisTitle : String
    studentEmail : String
*/

}