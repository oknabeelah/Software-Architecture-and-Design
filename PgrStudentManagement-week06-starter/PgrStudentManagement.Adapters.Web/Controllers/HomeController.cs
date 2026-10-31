using Microsoft.AspNetCore.Mvc;

namespace PgrStudentManagement.Adapters.Web.Controllers;

[Route("")]
public sealed class HomeController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }
}
