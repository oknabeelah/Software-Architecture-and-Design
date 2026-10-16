using PgrStudentManagement.Web.Modules.Students.Models;

namespace PgrStudentManagement.Web.Modules.Students.Data;

public class StudentStorage
{
    private readonly List<Student> _students =
    [
        new Student
        {
            StudentNumber = "S100001",
            FirstName = "John",
            LastName = "Doe",
            Course = "PhD Computing",
            ModeOfStudy = "Full-time",
            StartDate = new DateTime(2025, 10, 1),
            Status = Status.Researching,
            ThesisTitle =
                "Explainable Models for Clinical Decision Support",
            OriginalExpectedSubmissionDate =
                new DateTime(2028, 9, 30),
            ExpectedSubmissionDate =
                new DateTime(2028, 9, 30)
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
            ThesisTitle = null,
            OriginalExpectedSubmissionDate =
                new DateTime(2030, 9, 30),
            ExpectedSubmissionDate =
                new DateTime(2030, 9, 30)
        }
    ];

    public IReadOnlyList<Student> GetStudents()
    {
        return _students.AsReadOnly();
    }

    public Student? GetStudent(string studentNumber)
    {
        return _students.FirstOrDefault(
            student => student.StudentNumber == studentNumber);
    }

    public bool StudentNumberExists(string studentNumber)
    {
        return _students.Any(
            student => student.StudentNumber == studentNumber);
    }

    public void AddStudent(Student student)
    {
        _students.Add(student);
    }

    public void UpdateStudent(Student student)
    {
        var index = _students.FindIndex(
            existing =>
                existing.StudentNumber == student.StudentNumber);

        if (index >= 0)
        {
            _students[index] = student;
        }
    }
}
