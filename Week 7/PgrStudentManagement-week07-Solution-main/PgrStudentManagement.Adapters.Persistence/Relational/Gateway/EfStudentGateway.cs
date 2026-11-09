using Microsoft.EntityFrameworkCore;
using PgrStudentManagement.Adapters.Persistence.Relational.Mappers;
using PgrStudentManagement.Business.Students.Gateway;
using PgrStudentManagement.Domain.Students;

namespace PgrStudentManagement.Adapters.Persistence.Relational.Gateway;

public class EfStudentGateway : IStudentGateway
{
    private readonly StudentManagementDbContext _context;
    private readonly StudentPersistenceMapper _mapper;

    public EfStudentGateway(
        StudentManagementDbContext context,
        StudentPersistenceMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Student?> GetByStudentNumberAsync(
        string studentNumber,
        CancellationToken cancellationToken = default)
    {
        var record = await _context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(
                student => student.StudentNumber == studentNumber,
                cancellationToken);

        return record is null
            ? null
            : _mapper.ToDomain(record);
    }
}

