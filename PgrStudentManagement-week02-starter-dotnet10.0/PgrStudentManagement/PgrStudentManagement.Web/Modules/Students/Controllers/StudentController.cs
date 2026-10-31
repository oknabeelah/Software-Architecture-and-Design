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

    public async Task<IActionResult> Index()
    {
        try
        {
            var students = _studentService.GetStudentsAsync();
            return View(students);
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
        try
        {
            var enrolments = await _studentService.GetEnrolmentsForStudentAsync(studentNumber);
            ViewData["StudentNumber"] = studentNumber;

            if (enrolments.Count == 0)
            {
                ViewData["Message"] = "No enrolments found";
            }
            return View(enrolments);
        }
        catch (KeyNotFoundException)
        {
            TempData["StudentNotFound"] = $"Student with number {studentNumber} not found.";
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> EnrolmentDetails(string studentNumber, string? returnTo)
    {
        try
        {
            Student? student = await _studentService.GetStudentAsync(studentNumber);

            ViewBag.ReturnTo = returnTo; // store the returnTo value in ViewData to be used in the view for navigation
            return View(student);
        }
        catch (KeyNotFoundException)
        {
            TempData["StudentNotFound"] = $"Student with number {studentNumber} not found.";
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> ThesisDetails(string studentNumber)
    {
        try
        {
            Student? student = await _studentService.GetStudentAsync(studentNumber);
            if (student?.ThesisTitle == null)
            {
                return View("NoThesisDetails", student);
            }

            return View(student);
        }
        catch (KeyNotFoundException)
        {
            TempData["StudentNotFound"] = $"Student with number {studentNumber} not found.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditStatus(string studentNumber)
    {
        try
        {
            var student = await _studentService.GetStudentAsync(studentNumber);
            return View(student);
        }
        catch (KeyNotFoundException)
        {
            TempData["StudentNotFound"] = $"Student with number {studentNumber} not found.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditStatus(string studentNumber, Status status)
    {
        try
        {
            await _studentService.UpdateStatusAsync(studentNumber, status);
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(nameof(status), exception.Message);
            var student = await _studentService.GetStudentAsync(studentNumber);
            return View(student);
        }
        catch (KeyNotFoundException exception)
        {
            TempData["StudentNotFound"] = exception.Message;
            return RedirectToAction(nameof(Index));
        }

        TempData["StatusUpdateSuccess"] = $"The student's status was updated to {status}.";
        return RedirectToAction(nameof(EditStatus), new { studentNumber });
    }

    [HttpGet]
    public async Task<IActionResult> EditExpectedSubmissionDate(string studentNumber)
    {
         try
        {
            var student = await _studentService.GetStudentAsync(studentNumber);
            return View(student);
        }
        catch (KeyNotFoundException)
        {
            TempData["StudentNotFound"] = $"Student with number {studentNumber} not found.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditExpectedSubmissionDate(string studentNumber, DateTime? expectedThesisSubmissionDate)
    {
        try
        {
            await _studentService.UpdateExpectedSubmissionDate(studentNumber, expectedThesisSubmissionDate);
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(nameof(expectedThesisSubmissionDate), exception.Message);
            Student? student = await _studentService.GetStudentAsync(studentNumber);
            return View(student);
        }

        TempData["ExpectedThesisSubmissionDateUpdateSuccess"] = $"The Student's expected thesis submission date was updated to {expectedThesisSubmissionDate:dd/MM/yyyy}.";
        return RedirectToAction(nameof(EditExpectedSubmissionDate), new { studentNumber });
    }

    [HttpGet]
    public async Task<IActionResult> RecordSubmission(string studentNumber)
    {
        Student? student = await _studentService.GetStudentAsync(studentNumber);
        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RecordSubmission(string studentNumber, DateTime? actualThesisSubmissionDate)
    {
        try
        {
           await _studentService.RecordSubmission(studentNumber, actualThesisSubmissionDate);
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(nameof(actualThesisSubmissionDate), exception.Message);
            Student? student = await _studentService.GetStudentAsync(studentNumber);
            return View(student);
        }

        TempData["RecordSubmissionSuccess"] = $"The student's actual thesis submission date was recorded as {actualThesisSubmissionDate:dd/MM/yyyy} and status updated to Submitted.";
        return RedirectToAction(nameof(RecordSubmission), new {studentNumber});
    }

    [HttpGet]
    public IActionResult CreateStudent()
    {
        return View("~/Views/Administrator/CreateStudent.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateStudent(Student student)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Administrator/CreateStudent.cshtml", student); // return the view with student to preserve the entered data and display validation errors
        }

        try
        {
            _studentService.CreateStudentAsync(student);
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(nameof(student.StudentNumber), exception.Message);
            return View("~/Views/Administrator/CreateStudent.cshtml", student);
        }
        TempData["CreateStudentSuccess"] = $"The student was successfully created.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> StudentDetails()
    {
        try
        {
            var students = _studentService.GetStudentsAsync();
            return View("~/Views/Administrator/StudentDetails.cshtml", students);
        }
        catch (KeyNotFoundException)
        {
            TempData["NoStudentsFound"] = "No students found.";
            return RedirectToAction(nameof(Administrator));
        }
    }
}