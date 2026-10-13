using Microsoft.AspNetCore.Mvc;
using PgrStudentManagement.Web.Models;
using PgrStudentManagement.Web.Services;

namespace PgrStudentManagement.Web.Controllers;

public class StudentsController : Controller
{    
    private readonly StudentService _studentService;

    public StudentsController(StudentService studentService)
    {
        _studentService = studentService;
    }

    public IActionResult Index() //recieves request, selects view, accesses the static collection => presentation layer
    {
        try
        {
            return View(_studentService.GetStudents());
        }
        catch (KeyNotFoundException)
        {
            TempData["NoStudentsFound"] = "No students found.";
            return RedirectToAction(nameof(Index));
        }
    }
    public IActionResult Administrator()
    {
        return View("~/Views/Administrator/Index.cshtml");
    }

    public IActionResult EnrolmentDetails(string studentNumber, string? returnTo) //recieves request, selects view, accesses the static collection, finds student, => presentation layer
    {
        Student? student = _studentService.GetStudent(studentNumber);
        if (student == null)
        {
            return NotFound();
        }

        ViewBag.ReturnTo = returnTo; // store the returnTo value in ViewData to be used in the view for navigation

        return View(student);
    }

    public IActionResult ThesisDetails(string studentNumber) //recieves request, selects view, accesses the static collection, finds student, => presentation layer
    {
        Student? student = _studentService.GetStudent(studentNumber);
        if (student == null)
        {
            return NotFound();
        }
        if (student.ThesisTitle == null)
        {
            return View("NoThesisDetails", student);
        }

        return View(student);
    }

    [HttpGet]
    public IActionResult EditStatus(string studentNumber) //recieves request, selects view, accesses the static collection, finds student => presentation layer
    {
        Student? student = _studentService.GetStudent(studentNumber);
        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditStatus(string studentNumber, Status status) //updates student info => business layer
    {
        _studentService.UpdateStatus(studentNumber, status);
        TempData["StatusUpdateSuccess"] = $"The student's status was updated to {status}.";

        return RedirectToAction(nameof(EditStatus), new { studentNumber });
    }

    [HttpGet]
    public IActionResult EditExpectedSubmissionDate(string studentNumber) //presentation layer
    {
        Student? student = _studentService.GetStudent(studentNumber);

        return View(student);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditExpectedSubmissionDate(
        string studentNumber,
        DateTime? expectedThesisSubmissionDate) // business layer
    {
        _studentService.UpdateExpectedSubmissionDate(studentNumber, expectedThesisSubmissionDate);
        TempData["ExpectedThesisSubmissionDateUpdateSuccess"] = $"The Student's expected thesis submission date was updated to {expectedThesisSubmissionDate:dd/MM/yyyy}.";

        return RedirectToAction(nameof(EditExpectedSubmissionDate), new { studentNumber });
    }

    [HttpGet]
    public IActionResult RecordSubmission(string studentNumber) //presentation layer
    {
        Student? student = _studentService.GetStudent(studentNumber);
        if (student == null)
        {
            return NotFound();
        }
    
        return View(student);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RecordSubmission(string studentNumber,
        DateTime? actualThesisSubmissionDate) // business layer
    {
        _studentService.RecordSubmission(studentNumber, actualThesisSubmissionDate); // abstract the tasks of record submission (update actual thessi submission date and update status) from the controller
        TempData["RecordSubmissionSuccess"] = $"The student's actual thesis submission date was recorded as {actualThesisSubmissionDate:dd/MM/yyyy} and status updated to Submitted.";

        return RedirectToAction(nameof(RecordSubmission), new { studentNumber });
    }

    [HttpGet]
    public IActionResult CreateStudent() //business layer
    {
        return View("~/Views/Administrator/CreateStudent.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateStudent(Student student) // business layer
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Administrator/CreateStudent.cshtml", student); // return the view with student to preserve the entered data and display validation errors
        }

        try
        {
            _studentService.CreateStudent(student);
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(nameof(student.StudentNumber), exception.Message);
            return View("~/Views/Administrator/CreateStudent.cshtml", student);
        }
        TempData["CreateStudentSuccess"] = $"The student was successfully created.";

        return RedirectToAction(nameof(Index));
    }

    public IActionResult StudentDetails() // business layer
    {
        try
        {
            var students = _studentService.GetStudents();
            return View("~/Views/Administrator/StudentDetails.cshtml", students);
        }
        catch (KeyNotFoundException)
        {
            TempData["NoStudentsFound"] = "No students found.";
            return RedirectToAction(nameof(Administrator));
        }
    }
}