using Microsoft.EntityFrameworkCore;
using PgrStudentManagement.Web.Models;
namespace PgrStudentManagement.Web.Data.Daos;

public class EnrolmentDao
{
    private readonly StudentManagementDbContext _context;
    public EnrolmentDao(StudentManagementDbContext context)
    {
        _context = context;
    }
    public async Task<IReadOnlyList<Enrolment>> GetEnrolmentsForStudentAsync(string studentNumber)
    {
        return await _context.Enrolments.AsNoTracking()
            .Where(enrolment => enrolment.StudentNumber == studentNumber)
                .OrderBy(enrolment => enrolment.StartDate)
                .ToListAsync();
    }
    public async Task<IReadOnlyList<Student>> GetStudentsAsync()
    {
        return await _context.Students.AsNoTracking()
            .ToListAsync();
    }
    public async Task<Student?> GetStudentAsync(string studentNumber)
    {
        return await _context.Students.AsNoTracking()
            .Where(s => s.StudentNumber == studentNumber)
            .FirstOrDefaultAsync();
    }
    public async Task<Student?> UpdateStatusAsync(string studentNumber, Status status)
    {
        // from the enrolment num and student num
        // update the enrolment status to the new status 
        return await _context.Students
            .Where(s => s.StudentNumber == studentNumber)
            .FirstOrDefaultAsync();
        // TO DO : Refactor
    }

    public async Task<Student?> CreateStudentAsync(Student student)
    {
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<Student?> UpdateExpectedThesisSubmissionDate(string studentNumber, DateTime expectedThesisSubmissionDate)
    {
        var student = await _context.Students
            .Where(s => s.StudentNumber == studentNumber)
            .FirstOrDefaultAsync();

        student?.ExpectedThesisSubmissionDate = expectedThesisSubmissionDate;
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<Student?> UpdateActualThesisSubmissionDate(string studentNumber, DateTime actualThesisSubmissionDate)
    {
        var student = await _context.Students
            .Where(s => s.StudentNumber == studentNumber)
            .FirstOrDefaultAsync();

        student?.ActualThesisSubmissionDate = actualThesisSubmissionDate;
        await _context.SaveChangesAsync();
        return student;
    }

}