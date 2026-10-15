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

    public IActionResult SupervisorDetails(string staffNumber)
    {
        try
        {
            AcademicSupervisor? supervisor = _academicSupervisorService.GetSupervisorByStaffNumber(staffNumber);
            return View(supervisor);
        }
        catch (KeyNotFoundException)
        {
            TempData["SupervisorNotFound"] = $"Supervisor with staff number '{staffNumber}' not found.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public IActionResult AssignSupervisor()
    {
    
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AssignSupervisor(SupervisoryAssignment assignment)
    {
        if (!ModelState.IsValid)
        {
            return View(assignment);
        }

        try
        {
            _academicSupervisorService.AssignSupervisorToStudent(assignment);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(assignment);
        }

        TempData["AssignSupervisorSuccess"] = "Supervisor assigned to student successfully.";
        return RedirectToAction(nameof(Index)); // redirect to supervisory team with student number
    }


}