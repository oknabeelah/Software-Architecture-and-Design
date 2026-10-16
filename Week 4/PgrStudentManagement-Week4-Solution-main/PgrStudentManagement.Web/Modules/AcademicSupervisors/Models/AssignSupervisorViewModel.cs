using System.ComponentModel.DataAnnotations;
using PgrStudentManagement.Web.Modules.Students.Contracts;

namespace PgrStudentManagement.Web.Modules.AcademicSupervisors.Models;

public class AssignSupervisorViewModel
{
    [Required]
    public string StudentNumber { get; set; } = string.Empty;

    public StudentSummary? Student { get; set; }

    [Required]
    [Display(Name = "Academic Supervisor")]
    public string StaffNumber { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Supervisory Role")]
    public SupervisoryRole Role { get; set; } = SupervisoryRole.FirstSupervisor;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Start Date")]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Display(Name = "Authorise Fourth Supervisor Exception")]
    public bool ExceptionAuthorised { get; set; }

    public IReadOnlyList<AcademicSupervisor> AvailableSupervisors { get; set; } = [];
}
