using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PgrStudentManagement.Web.Modules.PhdApplications.Models;

// NOTE (UI/UX lab starting point):
// Validation attributes ([Required], [MaxLength], [EmailAddress], etc.)
// have intentionally been removed from this model so that the
// PhD application forms start without validation. Database
// constraints are still enforced via the fluent configuration in
// StudentManagementDbContext.ConfigurePhdApplications.
// See LabResources/lab-instructions.md.
[Index(nameof(EmailAddress))]
public class PhdApplication
{
    [Key]
    public string ApplicationReference { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string EmailAddress { get; set; } = string.Empty;

    public string ProgrammeCode { get; set; } = string.Empty;

    public string ModeOfStudy { get; set; } = string.Empty;

    public DateTime ProposedStartDate { get; set; }

    public string ProposedThesisTitle { get; set; } = string.Empty;

    public string? ResearchSummary { get; set; }

    public FundingType FundingSource { get; set; }

    public string? ProposedSupervisorStaffNumber { get; set; }

    public ApplicationStatus Status { get; set; } = ApplicationStatus.Submitted;

    public DateTime SubmittedDate { get; set; }

    public DateTime? DecisionDate { get; set; }

    public string? EnrolledStudentNumber { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}
