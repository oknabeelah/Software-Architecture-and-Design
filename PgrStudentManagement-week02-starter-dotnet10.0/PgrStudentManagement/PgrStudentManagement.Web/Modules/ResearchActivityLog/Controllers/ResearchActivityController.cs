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
}