using System.ComponentModel.DataAnnotations;

namespace PgrResearchActivity.Web.Models;

public class CreateResearchActivityRequest
{
    [Required]
    public string StudentNumber { get; set; } = string.Empty;

    [Required]
    public ResearchActivityType ActivityType { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime ActivityDate { get; set; } = DateTime.Today;

    public string Tags { get; set; } = string.Empty;

    public string Details { get; set; } = string.Empty;
}
