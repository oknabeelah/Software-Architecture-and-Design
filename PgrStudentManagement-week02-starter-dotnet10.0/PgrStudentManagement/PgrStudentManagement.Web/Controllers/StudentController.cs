using Microsoft.AspNetCore.Mvc;
using PgrStudentManagement.Web.Models;

namespace PgrStudentManagement.Web.Controllers;

public class StudentsController : Controller
{
    private static List<Student> Students = new ()
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

    public IActionResult Index()
    {
        return View(Students);
    }

    public IActionResult EnrolmentDetails(string studentNumber)
    {
        // TODO W2-UC01:
        // 1. Find the student.
        // 2. Handle Student Not Found.
        // 3. Supply the selected enrolment fields to the view.
        Student student = Students.Find(s => s.StudentNumber == studentNumber);
        if (student == null)
        {
            return NotFound();
        }
        // else
        // {
        //     ViewData["StudentNumber"] = student.StudentNumber;
        //     ViewData["FullName"] = student.FullName;
        //     ViewData["Course"] = student.Course;
        //     ViewData["ModeOfStudy"] = student.ModeOfStudy;
        //     ViewData["StartDate"] = student.StartDate?.ToString("dd/MM/yyyy");
        // }

        return View(student);
    }

    public IActionResult ThesisDetails(string studentNumber)
    {
        // TODO W2-UC02:
        // 1. Find the student.
        // 2. Determine whether thesis information is available.
        // 3. Supply the selected thesis fields to the view.
        Student student = Students.Find(s => s.StudentNumber == studentNumber);
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
    public IActionResult EditStatus(string studentNumber)
    {
        // TODO W2-UC03:
        // Find the student and display the current status.

        return View();
    }

    [HttpPost]
    public IActionResult EditStatus(string studentNumber, Status status)
    {
        // TODO W2-UC03:
        // Validate and update the selected student's status.

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult EditExpectedSubmissionDate(string studentNumber)
    {
        // TODO W2-UC04:
        // Find the student and display the existing expected date.

        return View();
    }

    [HttpPost]
    public IActionResult EditExpectedSubmissionDate(string studentNumber,     
            DateTime expectedSubmissionDate)
    {
        // TODO W2-UC04:
        // Validate and update the expected submission date.

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult RecordSubmission(string studentNumber)
    {
        // TODO W2-UC05:
        // Find the student and display current thesis information.

        return View();
    }

    [HttpPost]
    public IActionResult RecordSubmission(string studentNumber,
        DateTime actualSubmissionDate)
    {
        // TODO W2-UC05:
        // 1. Validate the date.
        // 2. Record the actual submission date.
        // 3. Change the status to Submitted.
        // 4. Ensure that no partial update occurs.

        return RedirectToAction(nameof(Index));
    }
}
