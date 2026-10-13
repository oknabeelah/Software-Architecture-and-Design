
namespace PgrAcademicSupervisorManagement.Web.Models;

public class AcademicSupervisor
{
    public string StaffNumber { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string EmailAddress { get; set; } = string.Empty; // null! vs string.Empty - null! is used to indicate that the property is required and should not be null, while string.Empty is used to initialize the property with an empty string value.
    public bool TrainingCompleted { get; set; }
    public HashSet<SupervisoryRole> EligibleRoles { get; set; } = new(); // set vs hashset - set is an unordered collection of unique elements, while hashset is a collection of unique elements that uses a hash table for storage and provides faster lookup times.

}