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
    public async Task<IReadOnlyList<Enrolment>>
    GetEnrolmentsForStudentAsync(string studentNumber)
    {
        return await _context.Enrolments.AsNoTracking()
            .Where(enrolment => enrolment.StudentNumber == studentNumber)
                .OrderBy(enrolment => enrolment.StartDate)
                .ToListAsync();
    }
}