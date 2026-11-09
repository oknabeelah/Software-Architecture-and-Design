using Microsoft.EntityFrameworkCore;
using PgrStudentManagement.Business.Students.Gateway;
using PgrStudentManagement.Domain.Students;

namespace PgrStudentManagement.Adapters.Persistence.Relational.Gateway;

public sealed class EfStudentGateway : IStudentGateway
{
    private readonly StudentManagementDbContext _context;

    public EfStudentGateway(StudentManagementDbContext context)
    {
        _context = context;
    }

    public async Task<Student?> GetByStudentNumberAsync(
        string studentNumber,
        CancellationToken cancellationToken = default)
    {
        var record = await _context.Students
            .AsNoTracking()
            .SingleOrDefaultAsync(
                student => student.StudentNumber == studentNumber,
                cancellationToken);

        return record is null
            ? null
            : new Student
            {
                StudentNumber = record.StudentNumber,
                FirstName = record.FirstName,
                LastName = record.LastName,
                EmailAddress = record.EmailAddress
            };
    }
}
