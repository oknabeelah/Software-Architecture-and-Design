using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PgrStudentManagement.Web.Models;

namespace PgrStudentManagement.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Progression()
    {
        return View();
    }

    public IActionResult Ethics()
    {
        return View();
    }

    public IActionResult Training()
    {
        return View();
    }

    public IActionResult Reports()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
