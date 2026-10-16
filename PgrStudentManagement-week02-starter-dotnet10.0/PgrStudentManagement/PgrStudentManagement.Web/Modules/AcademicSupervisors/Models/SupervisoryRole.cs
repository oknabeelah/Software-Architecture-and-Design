namespace PgrAcademicSupervisorManagement.Web.Models;
public enum SupervisoryRole
{
    DirectorOfStudies,
    FirstSupervisor,
    SecondSupervisor,
    Adviser
}
// The Director of Studies and First Supervisor
// must be institution-employed. An external
// supervisor may be assigned as an additional
// supervisor or adviser.