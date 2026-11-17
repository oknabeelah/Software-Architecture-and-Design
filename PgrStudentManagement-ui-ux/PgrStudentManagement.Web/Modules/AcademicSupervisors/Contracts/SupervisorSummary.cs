namespace PgrStudentManagement.Web.Modules.AcademicSupervisors.Contracts;

public record SupervisorSummary(
    string StaffNumber,
    string DisplayName,
    bool IsEligibleMainSupervisor);

