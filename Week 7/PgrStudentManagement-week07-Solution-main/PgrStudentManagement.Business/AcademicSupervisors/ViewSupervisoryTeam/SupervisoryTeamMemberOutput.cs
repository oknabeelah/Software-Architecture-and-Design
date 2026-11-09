using PgrStudentManagement.Domain.AcademicSupervisors;

namespace PgrStudentManagement.Business.AcademicSupervisors.ViewSupervisoryTeam;

public record SupervisoryTeamMemberOutput(
    string SupervisorStaffNumber,
    string SupervisorName,
    SupervisorEmploymentType EmploymentType,
    SupervisoryRole Role,
    DateTime StartDate,
    DateTime? EndDate)
{
    public bool Current => EndDate is null;
}

