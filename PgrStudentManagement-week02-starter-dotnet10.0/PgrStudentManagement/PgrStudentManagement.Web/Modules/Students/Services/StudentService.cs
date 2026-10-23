using PgrStudentManagement.Web.Models;
using PgrStudentManagement.Web.Data;
using PgrStudentManagement.Web.Data.Daos;
using PgrStudentManagement.Web.Modules.Students.Contracts;


namespace PgrStudentManagement.Web.Services;

public class StudentService : IStudentLookup
{
    private readonly EnrolmentDao _enrolmentDao;
    private readonly StudentStorage _studentStorage;
    public StudentService(StudentStorage studentStorage, EnrolmentDao enrolmentDao)
    {
        _studentStorage = studentStorage;
        _enrolmentDao = enrolmentDao;
    }

    public Task<IReadOnlyList<Enrolment>> GetEnrolmentsForStudentAsync(string studentNumber) // Adapt the constructor to the dependencies in your implementation.
    {
        _ = _enrolmentDao.GetStudentAsync(studentNumber)?? throw new KeyNotFoundException($"Student with number {studentNumber} not found.");

        return _enrolmentDao.GetEnrolmentsForStudentAsync(studentNumber); // The service coordinates the use case and delegates relational data access to the DAO.
    }
    public IReadOnlyList<Student> GetStudentsAsync()
    {
        var students  = _enrolmentDao.GetStudentsAsync().Result;
        if (!students.Any())
        {
            throw new KeyNotFoundException("No students found.");
        }
        return students.OrderBy(s => s.StudentNumber).ToList().AsReadOnly();
    }
    public async Task<Student> GetStudentAsync(string studentNumber)
    {
        var student = await _enrolmentDao.GetStudentAsync(studentNumber) ?? throw new KeyNotFoundException($"Student with number {studentNumber} not found.");

        return student;
    }
    public async void CreateStudentAsync(Student student)
    {
        // check that required info is supplied and valid
        // if any student.StudentNumber, student.FirstName, student.LastName, student.Course, student.ModeOfStudy, student.StartDate is null or empty, throw an ArgumentException
        if (string.IsNullOrEmpty(student.StudentNumber) || string.IsNullOrEmpty(student.FirstName) || string.IsNullOrEmpty(student.LastName) || string.IsNullOrEmpty(student.EmailAddress) || string.IsNullOrEmpty(student.ModeOfStudy) || student.StartDate == null) // no need to check Status as its never null
        {
            throw new ArgumentException("Required student information is missing or invalid.");
        }
        // check that the student number is not in use
        if (_enrolmentDao.GetStudentAsync(student.StudentNumber) != null)
        {
            throw new ArgumentException(" A Student with this number already exists.");
            // do not create a new student if the student number is already in use
        }

        await _enrolmentDao.CreateStudentAsync(student);
    }
    public async Task UpdateStatusAsync(string studentNumber, Status status)
    {
        _ = _enrolmentDao.GetStudentAsync(studentNumber) ?? throw new KeyNotFoundException($"Student with number {studentNumber} not found.");
        
        if (!Enum.IsDefined(status))
        {
            throw new ArgumentException("Invalid status.", nameof(status));
        }
        await _enrolmentDao.UpdateStatusAsync(studentNumber, status);
    }
    public async Task UpdateExpectedSubmissionDate(string studentNumber, DateTime? expectedSubmissionDate)
    {
        var student = await _enrolmentDao.GetStudentAsync(studentNumber) ?? throw new KeyNotFoundException($"Student with number {studentNumber} not found.");

        if (expectedSubmissionDate == null || expectedSubmissionDate < student.StartDate) // business rule - expected submission date cannot be null or earlier than the start date
        {
            throw new ArgumentException("Expected submission date cannot be null or earlier than the start date.", nameof(expectedSubmissionDate));
        }
        await _enrolmentDao.UpdateExpectedThesisSubmissionDate(studentNumber, expectedSubmissionDate.Value);
    }
    public async Task RecordSubmission(string studentNumber, DateTime? actualSubmissionDate)
    {
        var student = _studentStorage.GetStudent(studentNumber) ?? throw new KeyNotFoundException($"Student with number {studentNumber} not found.");

        if (actualSubmissionDate == null || actualSubmissionDate < student.StartDate) // business rule - expected submission date cannot be null or earlier than the start date
        {
            throw new ArgumentException("Expected submission date cannot be null or earlier than the start date.", nameof(actualSubmissionDate));
        }

        await _enrolmentDao.UpdateActualThesisSubmissionDate(studentNumber, actualSubmissionDate.Value);
        await _enrolmentDao.UpdateStatusAsync(studentNumber, Status.Submitted);
    }

    public StudentSummary? GetStudentSummary(string studentNumber)
    {
        var student = _studentStorage.GetStudent(studentNumber);
        if (student is null)
        {
            return null;
        }

        return new StudentSummary(student.StudentNumber, student.FullName);
    }
}