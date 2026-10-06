using Microsoft.AspNetCore.Mvc;
using PgrStudentManagement.Web.Models;

namespace PgrStudentManagement.Web.Controllers;

public class StudentsController : Controller
{
    private static List<Student> Students = new () //data layer
    {
        new Student
        {
            StudentNumber = "S100001",
            FirstName = "John",
            LastName = "Doe",
            Course = "PhD Computing",
            ModeOfStudy = "Full-time",
            StartDate = new DateTime(2025, 10, 1),
            Status = Status.Researching,
            ThesisTitle = "Explainable Models for Clinical Decision Support"
        },
        new Student
        {
            StudentNumber = "S100002",
            FirstName = "Jane",
            LastName = "Smith",
            Course = "Professional Doctorate",
            ModeOfStudy = "Part-time",
            StartDate = new DateTime(2024, 10, 1),
            Status = Status.WritingUp,
            ThesisTitle = null
        }
    };

    public IActionResult Index() //recieves request, selects view, accesses the static collection => presentation layer
    {
        return View(Students);
    }

    public IActionResult EnrolmentDetails(string studentNumber) //recieves request, selects view, accesses the static collection, finds student, => presentation layer
    {
        Student student = Students.Find(s => s.StudentNumber == studentNumber);
        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }

    public IActionResult ThesisDetails(string studentNumber) //recieves request, selects view, accesses the static collection, finds student, => presentation layer
    {
        Student student = Students.Find(s => s.StudentNumber == studentNumber);
        if (student == null)
        {
            return NotFound();
        }
        if (student.ThesisTitle == null)
        {
            // return an exception or error view indicating that thesis information is not available for this student
            return View("NoThesisDetails", student);
        }

        return View(student);
    }

    [HttpGet]
    public IActionResult EditStatus(string studentNumber) //recieves request, selects view, accesses the static collection, finds student => presentation layer
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
    public IActionResult EditStatus(string studentNumber, Status? status) //updates student info => business layer
    {
        Student student = Students.Find(s => s.StudentNumber == studentNumber);
        if (student == null)
        {
            return NotFound();
        }

        if (!status.HasValue || !Enum.IsDefined(typeof(Status), status.Value))
        {
            ModelState.AddModelError(nameof(Status), "Please select a valid status.");
            return View(student);
        }

        student.Status = status.Value;
        TempData["StatusUpdateSuccess"] = $"{student.FullName}'s status was updated to {student.Status}.";

        return RedirectToAction(nameof(EditStatus), new { studentNumber });
    }

    [HttpGet]
    public IActionResult EditExpectedSubmissionDate(string studentNumber) //presentation layer
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
    public IActionResult EditExpectedSubmissionDate(
        string studentNumber,
        DateTime? expectedThesisSubmissionDate) // business layer
    {
        Student student = Students.Find(s => s.StudentNumber == studentNumber);
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
