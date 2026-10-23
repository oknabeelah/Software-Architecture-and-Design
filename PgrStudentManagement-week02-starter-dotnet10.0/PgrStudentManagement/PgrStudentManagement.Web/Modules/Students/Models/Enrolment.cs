using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PgrStudentManagement.Web.Models;

public class Enrolment
{
    [Key]
    public int EnrolmentId { get; set; }

    [Required]
    [MaxLength(20)]
    public string StudentNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string ProgrammeCode { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }
    public DateTime? ExpectedEndDate { get; set; }

    [Required]
    [MaxLength(20)]
    public string ModeOfStudy { get; set; } = string.Empty;
    
    [Required]
    public EnrolmentStatus Status { get; set; }
    public Student Student { get; set; } = null!;
}