using System.Text.Json.Serialization;

namespace PgrStudentManagement.Web.Modules.ResearchActivities.Models;

public class ResearchActivity
{
    public string Id { get; set; } = string.Empty;

    public string StudentNumber { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ResearchActivityType ActivityType { get; set; }

    public string Title { get; set; } = string.Empty;

    public DateTime ActivityDate { get; set; }

    public DateTime CreatedOn { get; set; }

    public List<string> Tags { get; set; } = [];

    public Dictionary<string, object> Details { get; set; } = new();
}
