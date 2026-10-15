
namespace PgrStudentManagement.Web.Modules.Students.Contracts;
public interface IStudentLookup
{
    StudentSummary? GetStudentSummary(string studentNumber);
}

// Lookup contract
// This interface defines a contract for looking up student summaries based on their student number.
// to be used by Academic Supervisor

// Academic Supervisors Module -> Students Module