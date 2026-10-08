using Microsoft.AspNetCore.Mvc;
using PgrStudentManagement.Web.Models;
using PgrStudentManagement.Web.Data;

namespace PgrStudentManagement.Web.Services;
public class StudentService
{
    private readonly StudentStorage _studentStorage;
    public StudentService(StudentStorage studentStorage)
    {
        _studentStorage = studentStorage; // inject the StudentStorage through its constructor and assign it to the private field _studentStorage
    }
    public IReadOnlyList<Student> GetStudents()
    {
        return _studentStorage.GetStudents();
    }
    public Student? GetStudent(string studentNumber)  //business rule - return not found if student is null
    {
        var student = _studentStorage.GetStudent(studentNumber);
        if (student == null)
        {
            throw new KeyNotFoundException($"Student with number {studentNumber} not found.");
        }
        return student;
    }
    public void CreateStudent(Student student) // business rule - ask StudentStorage toadd student to the list of students if student is not null
    {
        if (student == null)
        {
            //return NotFound(); // because StudentService is not a controller, it cannot return Notfound(). 
            // Instead, we can throw an exception or handle the null case in a different way.
            throw new ArgumentNullException(nameof(student), "Student cannot be null.");
        }
        _studentStorage.AddStudent(student);
    }
    public void UpdateStatus(string studentNumber, Status status) // business rule - ensure student exists and status is a valid status
    {
        var student = _studentStorage.GetStudent(studentNumber);
        if (student == null)
        {
            throw new KeyNotFoundException($"Student with number {studentNumber} not found.");
        }
        if (!Enum.IsDefined(typeof(Status), status))
        {
            throw new ArgumentException("Invalid status.", nameof(status));
        }
        _studentStorage.UpdateStatus(studentNumber, status);
    }
    public void UpdateExpectedSubmissionDate(string studentNumber, DateTime? expectedSubmissionDate)
    {
        // check student and datetime
        var student = _studentStorage.GetStudent(studentNumber);
        if (student == null)
        {
            throw new KeyNotFoundException($"Student with number {studentNumber} not found.");
        }
        if (expectedSubmissionDate == null || expectedSubmissionDate < student.StartDate)
        {
            throw new ArgumentException("Expected submission date cannot be null or earlier than the start date.", nameof(expectedSubmissionDate));
        }
        _studentStorage.UpdateExpectedThesisSubmissionDate(studentNumber, expectedSubmissionDate.Value);
    }
    public void RecordSubmission(string studentNumber, DateTime? actualSubmissionDate)
    {
        //check student and datetime
        //do record submission
        //do update status to submitted
        var student = _studentStorage.GetStudent(studentNumber);
        if (student == null)
        {
            throw new KeyNotFoundException($"Student with number {studentNumber} not found.");
        }
        // TODO W2-UC05:
    }
}