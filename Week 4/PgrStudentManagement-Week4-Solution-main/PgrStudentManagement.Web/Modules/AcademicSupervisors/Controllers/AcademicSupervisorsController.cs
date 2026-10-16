using Microsoft.AspNetCore.Mvc;
using PgrStudentManagement.Web.Modules.AcademicSupervisors.Models;
using PgrStudentManagement.Web.Modules.AcademicSupervisors.Services;

namespace PgrStudentManagement.Web.Modules.AcademicSupervisors.Controllers;

public class AcademicSupervisorsController : Controller
{
    private readonly AcademicSupervisorService _supervisorService;

    public AcademicSupervisorsController(AcademicSupervisorService supervisorService)
    {
        _supervisorService = supervisorService;
    }

    public IActionResult Index()
    {
        var supervisors = _supervisorService.GetSupervisors();
        return View(supervisors);
    }

    public IActionResult Details(string staffNumber)
    {
        var supervisor = _supervisorService.GetSupervisor(staffNumber);
        if (supervisor is null)
        {
            return View("SupervisorNotFound");
        }

        return View(supervisor);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var supervisor = new AcademicSupervisor
        {
            EmploymentType = SupervisorEmploymentType.InstitutionEmployed,
            TrainingCompleted = true,
            EligibleRoles =
            [
                SupervisoryRole.DirectorOfStudies,
                SupervisoryRole.FirstSupervisor,
                SupervisoryRole.SecondSupervisor,
                SupervisoryRole.Adviser
            ]
        };

        return View(supervisor);
    }

    [HttpPost]
    public IActionResult Create(AcademicSupervisor supervisor, List<SupervisoryRole> eligibleRoles)
    {
        if (!ModelState.IsValid)
        {
            return View(supervisor);
        }

        if (eligibleRoles is not null && eligibleRoles.Count > 0)
        {
            supervisor.EligibleRoles = new HashSet<SupervisoryRole>(eligibleRoles);
        }

        var result = _supervisorService.CreateSupervisor(supervisor);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(
                nameof(AcademicSupervisor.StaffNumber),
                result.ErrorMessage!);

            return View(supervisor);
        }

        return RedirectToAction(
            nameof(Details),
            new { staffNumber = supervisor.StaffNumber });
    }

    public IActionResult SupervisoryTeam(string studentNumber)
    {
        var student = _supervisorService.GetStudentSummary(studentNumber);
        if (student is null)
        {
            return View("StudentNotFound");
        }

        var assignments = _supervisorService.GetAssignmentsForStudent(studentNumber);
        var assessment = _supervisorService.EvaluateSupervisoryTeam(studentNumber);

        var items = assignments.Select(a => new SupervisoryTeamItemViewModel
        {
            Assignment = a,
            Supervisor = _supervisorService.GetSupervisor(a.SupervisorStaffNumber)
        }).ToList();

        var viewModel = new SupervisoryTeamViewModel
        {
            Student = student,
            Assignments = items.AsReadOnly(),
            Assessment = assessment
        };

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Assign(string studentNumber)
    {
        var student = _supervisorService.GetStudentSummary(studentNumber);
        if (student is null)
        {
            return View("StudentNotFound");
        }

        var viewModel = new AssignSupervisorViewModel
        {
            StudentNumber = studentNumber,
            Student = student,
            AvailableSupervisors = _supervisorService.GetSupervisors()
        };

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult Assign(AssignSupervisorViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Student = _supervisorService.GetStudentSummary(model.StudentNumber);
            model.AvailableSupervisors = _supervisorService.GetSupervisors();
            return View(model);
        }

        var (outcome, errorMessage) = _supervisorService.AssignSupervisor(
            model.StudentNumber,
            model.StaffNumber,
            model.Role,
            model.StartDate,
            model.ExceptionAuthorised);

        if (outcome != AssignmentOutcome.Created)
        {
            ModelState.AddModelError(string.Empty, errorMessage ?? "Failed to assign supervisor.");
            model.Student = _supervisorService.GetStudentSummary(model.StudentNumber);
            model.AvailableSupervisors = _supervisorService.GetSupervisors();
            return View(model);
        }

        return RedirectToAction(
            nameof(SupervisoryTeam),
            new { studentNumber = model.StudentNumber });
    }
}
