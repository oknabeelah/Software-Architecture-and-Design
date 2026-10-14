using Microsoft.AspNetCore.Mvc;
using PgrAcademicSupervisorManagement.Web.Models;
using PgrAcademicSupervisorManagement.Web.Services;

namespace PgrAcademicSupervisorManagement.Web.Controllers;

public class AcademicSupervisorsController : Controller
{
    private readonly AcademicSupervisorService _academicSupervisorService;

    public AcademicSupervisorsController(AcademicSupervisorService academicSupervisorService)
    {
        _academicSupervisorService = academicSupervisorService;
    }
    public IActionResult Index()
    {
        try
        {
            return View(_academicSupervisorService.GetAllSupervisors());
        }
        catch (KeyNotFoundException)
        {
            TempData["NoSupervisorsFound"] = "No supervisors found.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public IActionResult CreateSupervisor()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateSupervisor(AcademicSupervisor supervisor)
    {
        if (!ModelState.IsValid)
        {
            return View(supervisor);
        }

        try
        {
            _academicSupervisorService.CreateSupervisor(supervisor);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(supervisor.StaffNumber), ex.Message);
            return View(supervisor);
        }

        TempData["SupervisorSuccessMessage"] = "Academic Supervisor created successfully.";
        return RedirectToAction(nameof(Index));
    }



}