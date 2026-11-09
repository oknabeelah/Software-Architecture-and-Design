namespace PgrStudentManagement.Adapters.API.Responses;

public record SupervisoryTeamMemberResponse(
    string SupervisorStaffNumber,
    string SupervisorName,
    string EmploymentType,
    string Role,
    DateTime StartDate,
    DateTime? EndDate,
    bool Current);

public record SupervisoryTeamResponse(
    string StudentNumber,
    string StudentDisplayName,
    IReadOnlyList<SupervisoryTeamMemberResponse> CurrentAssignments,
    IReadOnlyList<SupervisoryTeamMemberResponse> HistoricalAssignments);

