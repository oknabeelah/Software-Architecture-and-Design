namespace PgrResearchActivity.Web.Models;

public class ActivityDetails
{
    public class ReflectionDetails
    {
        public string Summary { get; set; } = string.Empty;
        public List<string> NextSteps { get; set; } = new List<string>();
    }

    public class MeetingDetails
    {
        public List<string> Attendees { get; set; } = new List<string>();
        public string Discussion { get; set; } = string.Empty;
        public List<string> Actions { get; set; } = new List<string>();
    }

    public class ConferenceDetails
    {
        public string ConferenceName { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? PresentationTitle { get; set; }
    }

}