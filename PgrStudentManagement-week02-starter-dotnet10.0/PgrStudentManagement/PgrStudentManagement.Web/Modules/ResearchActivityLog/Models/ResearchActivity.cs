using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PgrResearchActivity.Web.Models;

public class ResearchActivity
{
    [BsonId]
    public string ActivityId { get; set; } = string.Empty;

    public string StudentNumber { get; set; } = string.Empty;

    public ResearchActivityType ActivityType { get; set; }

    public string Title { get; set; } = string.Empty;

    public DateTime ActivityDate { get; set; }

    public DateTime CreatedOn { get; set; }

    public List<string> Tags { get; set; } = new List<string>();

    //public ActivityDetails Details { get; set; } = new ActivityDetails();
    public Dictionary<string, object> Details { get; set; } = new();

}