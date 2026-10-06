
using Microsoft.AspNetCore.Mvc;
using PgrStudentManagement.Web.Models;

namespace PgrStudentManagement.Web.Controllers;

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

    public IEnumerable<Student> GetAll()
    {
        return _students;
    }

    public Student? Find(Predicate<Student> match)
    {
        return _students.Find(match);
    }

}