
using PgrStudentManagement.Web.Models;

namespace PgrStudentManagement.Web.Data;

public class StudentStorage
{
    private readonly List<Student> _students = new ()
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

    public IReadOnlyList<Student> GetStudents()
    {
        return _students;
    }

    public Student? GetStudent(string studentNumber)
    {
        return _students.FirstOrDefault(s => s.StudentNumber == studentNumber);
    }

    public void AddStudent(Student student)
    {
        if (student == null)
        {
            throw new ArgumentNullException(nameof(student));
        }
        _students.Add(student);
    }
    // rather than a UpdateStudent method, I implemented method(s) to update the status, thesis details, etc of a student as when needed

    // Q: is input validation done in the business or data layer?
    public void UpdateStatus(string studentNumber, Status? status)
    {
        var student = GetStudent(studentNumber);
        student?.Status = status ?? student.Status;
        
        // if (student != null)
        // {
        //     student.Status = status ?? student.Status;
        // }
        // else
        // {
        //     throw new ArgumentException($"Student with StudentNumber {studentNumber} not found.");
        // }
    }

    public void UpdateExpectedThesisSubmissionDate(string studentNumber, DateTime? expectedSubmissionDate)
    {
        var student = GetStudent(studentNumber);
        student?.ExpectedThesisSubmissionDate = expectedSubmissionDate ?? student.ExpectedThesisSubmissionDate;
    }

    public void RecordSubmission(string studentNumber, DateTime? actualSubmissionDate)
    {
        // TODO W2-UC05:
    }

}