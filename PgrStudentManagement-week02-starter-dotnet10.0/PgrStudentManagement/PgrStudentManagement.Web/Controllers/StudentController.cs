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
        //check here maybe?
        return View(_studentService.GetStudents());
    }

    public IActionResult EnrolmentDetails(string studentNumber) //recieves request, selects view, accesses the static collection, finds student, => presentation layer
    {
        Student? student = _studentService.GetStudent(studentNumber);
        if (student == null)
        {
            return NotFound();
        }

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
}