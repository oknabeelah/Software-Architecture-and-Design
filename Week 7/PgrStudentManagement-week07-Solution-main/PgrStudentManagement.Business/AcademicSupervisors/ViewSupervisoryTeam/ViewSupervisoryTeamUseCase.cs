using PgrStudentManagement.Business.AcademicSupervisors.Gateway;

namespace PgrStudentManagement.Business.AcademicSupervisors.ViewSupervisoryTeam;

public class ViewSupervisoryTeamUseCase : IViewSupervisoryTeamUseCase
{
    private readonly IAcademicSupervisorGateway _gateway;
    private readonly IStudentLookup _studentLookup;

    public ViewSupervisoryTeamUseCase(
        IAcademicSupervisorGateway gateway,
        IStudentLookup studentLookup)
    {
        _gateway = gateway;
        _studentLookup = studentLookup;
    }

    public async Task<SupervisoryTeamOutput> ExecuteAsync(
        ViewSupervisoryTeamInput input,
        CancellationToken cancellationToken = default)
    {
        var student = await _studentLookup.GetStudentSummaryAsync(
            input.StudentNumber,
            cancellationToken);

        if (student is null)
        {
            return new SupervisoryTeamOutput(
                StudentFound: false,
                Student: null,
                CurrentAssignments: Array.Empty<SupervisoryTeamMemberOutput>(),
                HistoricalAssignments: Array.Empty<SupervisoryTeamMemberOutput>());
        }

        var assignments = await _gateway.GetAssignmentsForStudentAsync(
            input.StudentNumber,
            cancellationToken);

        var resolvedAssignments = new List<SupervisoryTeamMemberOutput>();

        foreach (var assignment in assignments)
        {
            var supervisor = await _gateway.GetByStaffNumberAsync(
                assignment.SupervisorStaffNumber,
                cancellationToken);

            if (supervisor is null)
            {
                continue;
            }

            resolvedAssignments.Add(new SupervisoryTeamMemberOutput(
                SupervisorStaffNumber: supervisor.StaffNumber,
                SupervisorName: supervisor.FullName,
                EmploymentType: supervisor.EmploymentType,
                Role: assignment.Role,
                StartDate: assignment.StartDate,
                EndDate: assignment.EndDate));
        }

        var currentAssignments = resolvedAssignments
            .Where(a => a.Current)
            .OrderBy(a => a.Role)
            .ThenBy(a => a.SupervisorName)
            .ToList();

        var historicalAssignments = resolvedAssignments
            .Where(a => !a.Current)
            .OrderByDescending(a => a.EndDate)
            .ThenBy(a => a.Role)
            .ToList();

        return new SupervisoryTeamOutput(
            StudentFound: true,
            Student: student,
            CurrentAssignments: currentAssignments,
            HistoricalAssignments: historicalAssignments);
    }
}

