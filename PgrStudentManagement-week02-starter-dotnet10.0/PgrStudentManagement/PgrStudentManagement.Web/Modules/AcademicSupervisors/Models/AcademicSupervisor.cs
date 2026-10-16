
namespace PgrAcademicSupervisorManagement.Web.Models;

public class AcademicSupervisor
{
    public string StaffNumber { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string EmailAddress { get; set; } = string.Empty; // null! vs string.Empty - null! is used to indicate that the property is required and should not be null, while string.Empty is used to initialize the property with an empty string value.
    public SupervisorEmploymentType EmploymentType { get; set; }
    public bool TrainingCompleted { get; set; }
    public HashSet<SupervisoryRole> EligibleRoles { get; set; } = new(); // set vs hashset - set is an unordered collection of unique elements, while hashset is a collection of unique elements that uses a hash table for storage and provides faster lookup times.
    // eligible roles - roles for which the academic adversor is eligible to supervise students.

}

// Future Implementation
 
// public DateTime? StartDate {get; set;}

// RoleEligibilityException model
// namespace PgrAcademicSupervisorManagement.Web.Models;
// public class RoleEligibilityException
// {
//     public SupervisoryRole Role { get; set; }
//     public bool ExceptionAuthorised { get; set; }
//     public string? ExceptionReason { get; set; }
//     public string? ExceptionAuthorisedBy { get; set; }
//     public DateTime? ExceptionAuthorisedDate { get; set; }
// }

// Business Rule : eligibility
// var eligibleByRole = supervisor.EligibleRoles.Contains(assignment.Role);
// var eligibleByException = supervisor.ExceptedRoles.Any(exception =>
//     exception.Role == assignment.Role &&
//     exception.ExceptionAuthorised);
// var eligible = eligibleByRole || eligibleByException;


// Training model
// namespace PgrAcademicSupervisorManagement.Web.Models;
// public class SupervisorTraining
// {
//     public string TrainingName { get; set; } = string.Empty;
//     public DateTime AssignedDate { get; set; }
//     public string AssignedTo { get; set; } = string.Empty;
//     public DateTime? CompletedDate { get; set; }
//     public bool Completed => CompletedDate.HasValue;
// }

// public bool TrainingCompleted =>
    // Training
    //     .Any(training => training.AssignedDate >= StartDate) && Training
    //     .Where(training => training.AssignedDate >= StartDate)
    //     .All(training => training.CompletedDate.HasValue);
