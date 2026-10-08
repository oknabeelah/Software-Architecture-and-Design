
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

    public void UpdateStudent(Student student)
    {
        //find student obj in list and replace it with the updated student obj
        var existingStudent = GetStudent(student.StudentNumber);
        if (existingStudent != null)
        {
            _students[_students.IndexOf(existingStudent)] = student;
        }
        else
        {
            throw new ArgumentException($"Student with StudentNumber {student.StudentNumber} not found.");
        }
    }

}