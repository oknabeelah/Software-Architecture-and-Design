namespace PgrStudentManagement.Adapters.Web.ViewModels;

public sealed class EnrolmentViewModel
{
    public string ProgrammeCode { get; init; } = string.Empty;

    public DateTime StartDate { get; init; }

    public string ExpectedEndDate { get; init; } = string.Empty;

    public string ModeOfStudy { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;
}