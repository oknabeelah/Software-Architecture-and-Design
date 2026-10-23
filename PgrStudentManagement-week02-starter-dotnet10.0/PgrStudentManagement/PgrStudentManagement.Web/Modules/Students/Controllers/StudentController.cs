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
            return View(_studentService.GetStudents()); //TO DO: update this to pull from DAO instead of static collection
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

    public async Task<IActionResult> Enrolments(string studentNumber)
    {
        var enrolments = await _studentService.GetEnrolmentsForStudentAsync(studentNumber);
        ViewData["StudentNumber"] = studentNumber;

        if (enrolments.Count == 0)
        {
            ViewData["Message"] = "No enrolments found";
        }
        // distinguish between no enrolments and student not found

        // The controller selects the presentation response. It must not query StudentManagementDbContext or call EnrolmentDao directly.
        return View(enrolments);
    }

    public IActionResult EnrolmentDetails(string studentNumber, string? returnTo) //recieves request, selects view, accesses the static collection, finds student, => presentation layer
    {
        Student? student = _studentService.GetStudent(studentNumber);

        ViewBag.ReturnTo = returnTo; // store the returnTo value in ViewData to be used in the view for navigation
        return View(student);
    }

    public IActionResult ThesisDetails(string studentNumber) //recieves request, selects view, accesses the static collection, finds student, => presentation layer
    {
        Student? student = _studentService.GetStudent(studentNumber);
        if (student?.ThesisTitle == null)
        {
            return View("NoThesisDetails", student);
        }

        return View(student);
    }

    [HttpGet]
    public IActionResult EditStatus(string studentNumber) //recieves request, selects view, accesses the static collection, finds student => presentation layer
    {
        Student? student = _studentService.GetStudent(studentNumber);
        return View(student);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditStatus(string studentNumber, Status status) //updates student info => business layer
    {
        try
        {
            _studentService.UpdateStatus(studentNumber, status);
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(nameof(status), exception.Message);
            Student? student = _studentService.GetStudent(studentNumber);
            return View(student);
        }

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
        try
        {
            _studentService.UpdateExpectedSubmissionDate(studentNumber, expectedThesisSubmissionDate);
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(nameof(expectedThesisSubmissionDate), exception.Message);
            Student? student = _studentService.GetStudent(studentNumber);
            return View(student);
        }

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
        try
        {
            _studentService.RecordSubmission(studentNumber, actualThesisSubmissionDate);
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(nameof(actualThesisSubmissionDate), exception.Message);
            Student? student = _studentService.GetStudent(studentNumber);
            return View(student);
        }

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