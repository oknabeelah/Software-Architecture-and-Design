namespace PgrStudentManagement.Business.Students.GetStudentSummary;

public record GetStudentSummaryOutput(
    bool Found,
    string StudentNumber,
    string? DisplayName);

