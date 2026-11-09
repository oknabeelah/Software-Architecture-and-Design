using PgrStudentManagement.Business.AcademicSupervisors.Gateway;

namespace PgrStudentManagement.Business.AcademicSupervisors.ViewSupervisoryTeam;

public record SupervisoryTeamOutput(
    bool StudentFound,
    StudentSummary? Student,
    IReadOnlyList<SupervisoryTeamMemberOutput> CurrentAssignments,
    IReadOnlyList<SupervisoryTeamMemberOutput> HistoricalAssignments);

