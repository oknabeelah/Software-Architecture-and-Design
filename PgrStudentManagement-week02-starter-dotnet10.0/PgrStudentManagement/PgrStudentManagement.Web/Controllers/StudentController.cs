using Microsoft.AspNetCore.Mvc;
using PgrStudentManagement.Web.Models;
using PgrStudentManagement.Web.Data;

namespace PgrStudentManagement.Web.Controllers;

public class StudentsController : Controller
{
    private readonly StudentStorage Students;

    public StudentsController(StudentStorage _students) // _student is the readonly list, and Students is the property of the controller that will be used to access the list of students in the controller's actions
    {
        this.Students = _students;
    }

    public IActionResult Index() //recieves request, selects view, accesses the static collection => presentation layer
    {
        return View(Students.GetStudents());
    }

    public IActionResult EnrolmentDetails(string studentNumber) //recieves request, selects view, accesses the static collection, finds student, => presentation layer
    {
        Student? student = Students.GetStudent(studentNumber);
        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }

    public IActionResult ThesisDetails(string studentNumber) //recieves request, selects view, accesses the static collection, finds student, => presentation layer
    {
        Student? student = Students.GetStudent(studentNumber);
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
        Student? student = Students.GetStudent(studentNumber);
        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditStatus(string studentNumber, Status? status) //updates student info => business layer
    {
        Students.UpdateStatus(studentNumber, status);
        TempData["StatusUpdateSuccess"] = $"The student's status is now updated to {status}.";

        return RedirectToAction(nameof(EditStatus), new { studentNumber });
    }

    [HttpGet]
    public IActionResult EditExpectedSubmissionDate(string studentNumber) //presentation layer
    {
       Student? student = Students.GetStudent(studentNumber);
        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditExpectedSubmissionDate(
        string studentNumber,
        DateTime? expectedThesisSubmissionDate) // business layer
    {
        Student? student = Students.GetStudent(studentNumber);
        if (student == null)
        {
            return NotFound();
        }

        if (!expectedThesisSubmissionDate.HasValue || expectedThesisSubmissionDate.Value.Date <= DateTime.Today)
        {
            ModelState.AddModelError(nameof(expectedThesisSubmissionDate), "Please select a valid expected submission date.");
            return View(student);
        }

        student.ExpectedThesisSubmissionDate = expectedThesisSubmissionDate.Value.Date;
        TempData["ExpectedThesisSubmissionDateUpdateSuccess"] = $"{student.FullName}'s expected thesis submission date was updated to {student.ExpectedThesisSubmissionDate:dd/MM/yyyy}.";

        return RedirectToAction(nameof(EditExpectedSubmissionDate), new { studentNumber });
    }

    [HttpGet]
    public IActionResult RecordSubmission(string studentNumber) //presentation layer
    {
        Student student = Students.Find(s => s.StudentNumber == studentNumber);
        if (student == null)
        {
            return NotFound();
        }
    
        return View(student);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RecordSubmission(string studentNumber,
        DateTime? actualThesisSubmissionDate)
    {
        Student student = Students.Find(s => s.StudentNumber == studentNumber);
        if (student == null)
        {
            return NotFound();
        }

        if (!actualThesisSubmissionDate.HasValue ||
            actualThesisSubmissionDate.Value.Date > DateTime.Today)
        {
            ModelState.AddModelError(
                nameof(actualThesisSubmissionDate),
                "Please enter a valid actual submission date that is not in the future.");
            return View(student);
        }

        student.ActualThesisSubmissionDate = actualThesisSubmissionDate.Value.Date;
        student.Status = Status.Submitted;
        TempData["RecordSubmissionSuccess"] = $"{student.FullName}'s actual thesis submission date was recorded as {student.ActualThesisSubmissionDate:dd/MM/yyyy} and status updated to {student.Status}.";

        return RedirectToAction(nameof(RecordSubmission), new { studentNumber });
    }
}
