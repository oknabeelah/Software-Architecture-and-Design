using PgrResearchActivity.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace PgrResearchActivity.Web.Controllers;

public class ResearchActivityController : Controller
{

    public async Task<IActionResult> Index(string studentNumber)
    {
        //var repository = HttpContext.RequestServices.GetRequiredService<ResearchActivityRepository>();
        //var activities = await repository.GetForStudentAsync(studentNumber);
        return View();
    }
}