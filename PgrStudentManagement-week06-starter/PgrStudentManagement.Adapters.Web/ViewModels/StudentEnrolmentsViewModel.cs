namespace PgrStudentManagement.Adapters.Web.ViewModels;

public sealed class StudentEnrolmentsViewModel
{
    public string StudentNumber { get; init; } = string.Empty;

    public IReadOnlyList<EnrolmentViewModel> Enrolments { get; init; } =
        Array.Empty<EnrolmentViewModel>();
}