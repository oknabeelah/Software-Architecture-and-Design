using PgrStudentManagement.Web.Modules.Students.Contracts;

namespace PgrStudentManagement.Web.Modules.AcademicSupervisors.Models;

public class SupervisoryTeamItemViewModel
{
    public required SupervisoryAssignment Assignment { get; set; }
    public AcademicSupervisor? Supervisor { get; set; }
}

public class SupervisoryTeamViewModel
{
    public required StudentSummary Student { get; set; }
    public required IReadOnlyList<SupervisoryTeamItemViewModel> Assignments { get; set; }
    public required SupervisoryTeamAssessment Assessment { get; set; }
}
