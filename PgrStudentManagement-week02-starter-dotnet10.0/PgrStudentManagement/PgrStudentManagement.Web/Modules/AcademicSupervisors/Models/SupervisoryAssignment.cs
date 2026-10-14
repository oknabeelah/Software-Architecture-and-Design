namespace PgrAcademicSupervisorManagement.Web.Models;

public class SupervisoryAssignment {
    public string StudentNumber { get; set; } = string.Empty;
    public string AcademicSupervisorNumber { get; set; } = string.Empty;
    //public Student Student { get; set; } = null!; 
    //public AcademicSupervisor AcademicSupervisor { get; set; } = null!;
    //identifiers vs object references - avoids circular references and simplifies serialization, but requires additional lookups to retrieve the full object data. although this is not so bad
    public SupervisoryRole Role { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? EndReason { get; set; }
    public bool Current => EndDate == null; // derived from evaluating whether the assignment has an end date
    public bool ExceptionAuthorised { get; set; } // if the supervisor is not eligible for the role, but has been authorised to supervise the student in that role, this property will be set to true.

}
// Business Rule: A current combination of student,
// Academic Supervisor, and role must
// not be recorded more than once. // implement in Service

// Business Rule: A completed supervisory arrangement normally
// contains two or three supervisors. A fourth
// supervisor requires an authorised exception.
// ie rule - check before adding a new assignment, if the student already has 3 current assignments, then the new assignment can only be added if ExceptionAuthorised is true. // implement in Service

// Business Rule: A student must not have more than one current
// Director of Studies or more than one current
// First Supervisor. // implement in Service

// Business Rule: The Director of Studies and First Supervisor
// must be institution-employed. An external
// supervisor may be assigned as an additional
// supervisor or adviser. // implement in service and authorised exception process