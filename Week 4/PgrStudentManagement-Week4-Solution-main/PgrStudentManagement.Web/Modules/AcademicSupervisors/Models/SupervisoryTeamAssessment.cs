namespace PgrStudentManagement.Web.Modules.AcademicSupervisors.Models;

public record SupervisoryTeamAssessment(
    bool IsComplete,
    IReadOnlyList<string> Issues);
