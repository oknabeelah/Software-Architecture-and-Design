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
        if (!_studentStorage.GetStudents().Any())
        {
            throw new KeyNotFoundException("No students found.");
        }      
        return _studentStorage.GetStudents().OrderBy(s => s.StudentNumber).ToList().AsReadOnly();
        //apply ordering for W3UC02
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
    public void CreateStudent(Student student) // business rule - ask StudentStorage to add student to the list of students if student is not null
    {
        // check that required info is supplied and valid
        // if any student.StudentNumber, student.FirstName, student.LastName, student.Course, student.ModeOfStudy, student.StartDate is null or empty, throw an ArgumentException
        if (string.IsNullOrEmpty(student.StudentNumber) || string.IsNullOrEmpty(student.FirstName) || string.IsNullOrEmpty(student.LastName) || string.IsNullOrEmpty(student.Course) || string.IsNullOrEmpty(student.ModeOfStudy) || student.StartDate == null) // no need to check Status as its never null
        {
            throw new ArgumentException("Required student information is missing or invalid.");
        }
        // check that the student number is not in use
        if (_studentStorage.GetStudent(student.StudentNumber) != null)
        {
            throw new ArgumentException(" A Student with this number already exists.");
            // do not create a new student if the student number is already in use
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
        var student = _studentStorage.GetStudent(studentNumber);
        if (student == null)
        {
            throw new KeyNotFoundException($"Student with number {studentNumber} not found.");
        }
        if (expectedSubmissionDate == null || expectedSubmissionDate < student.StartDate) // business rule - expected submission date cannot be null or earlier than the start date
        {
            throw new ArgumentException("Expected submission date cannot be null or earlier than the start date.", nameof(expectedSubmissionDate));
        }
        _studentStorage.UpdateExpectedThesisSubmissionDate(studentNumber, expectedSubmissionDate.Value);
    }
    public void RecordSubmission(string studentNumber, DateTime? actualSubmissionDate)
    {
        var student = _studentStorage.GetStudent(studentNumber);
        if (student == null)
        {
            throw new KeyNotFoundException($"Student with number {studentNumber} not found.");
        }
        if (actualSubmissionDate == null || actualSubmissionDate < student.StartDate) // business rule - expected submission date cannot be null or earlier than the start date
        {
            throw new ArgumentException("Expected submission date cannot be null or earlier than the start date.", nameof(actualSubmissionDate));
        }
        _studentStorage.UpdateActualThesisSubmissionDate(studentNumber, actualSubmissionDate.Value);
        _studentStorage.UpdateStatus(studentNumber, Status.Submitted);
    }

}