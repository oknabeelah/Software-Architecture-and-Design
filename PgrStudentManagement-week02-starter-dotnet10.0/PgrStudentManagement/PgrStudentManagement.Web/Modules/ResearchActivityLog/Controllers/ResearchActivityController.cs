using Microsoft.AspNetCore.Mvc;
using PgrResearchActivity.Web.Data;
using PgrResearchActivity.Web.Models;

namespace PgrResearchActivity.Web.Controllers;

public class ResearchActivityController : Controller
{
    private readonly ResearchActivityRepository _repository;

    public ResearchActivityController(ResearchActivityRepository repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> Index(string? studentNumber)
    {
        if (string.IsNullOrWhiteSpace(studentNumber))
        {
            ViewData["StudentNumber"] = string.Empty;
            ViewData["Message"] = "Enter a student number to view research activities.";
            return View("~/Views/ResearchActivities/Index.cshtml", Array.Empty<ResearchActivity>());
        }

        var activities = await _repository.GetForStudentAsync(studentNumber.Trim());
        ViewData["StudentNumber"] = studentNumber.Trim();
        ViewData["Message"] = activities.Count == 0
            ? "No research activities were found for this student."
            : null;

        return View("~/Views/ResearchActivities/Index.cshtml", activities);
    }

    [HttpGet]
    public IActionResult Create(string? studentNumber)
    {
        return View("~/Views/ResearchActivities/Create.cshtml",
            new CreateResearchActivityRequest { StudentNumber = studentNumber?.Trim() ?? string.Empty });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateResearchActivityRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/ResearchActivities/Create.cshtml", request);
        }

        var details = request.Details
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(line => line.Split('=', 2, StringSplitOptions.TrimEntries))
            .Where(parts => parts.Length == 2 && parts[0].Length > 0)
            .ToDictionary(parts => parts[0], parts => (object)parts[1]);

        var activity = new ResearchActivity
        {
            ActivityId = Guid.NewGuid().ToString("N"),
            StudentNumber = request.StudentNumber.Trim(),
            ActivityType = request.ActivityType,
            Title = request.Title.Trim(),
            ActivityDate = request.ActivityDate,
            CreatedOn = DateTime.UtcNow,
            Tags = request.Tags
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList(),
            Details = details
        };

        await _repository.AddAsync(activity);
        return RedirectToAction(nameof(Details), new { activityId = activity.ActivityId });
    }

    [HttpGet("ResearchActivity/Details/{activityId}")]
    public async Task<IActionResult> Details(string activityId)
    {
        if (string.IsNullOrWhiteSpace(activityId))
        {
            return NotFound();
        }

        var activity = await _repository.GetByIdAsync(activityId.Trim());
        return activity is null ? NotFound() : View("~/Views/ResearchActivities/Details.cshtml", activity);
    }
}