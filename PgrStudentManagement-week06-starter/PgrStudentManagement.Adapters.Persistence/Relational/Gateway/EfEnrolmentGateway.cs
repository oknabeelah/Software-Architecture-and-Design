using Microsoft.EntityFrameworkCore;
using PgrStudentManagement.Adapters.Persistence.Relational;
using PgrStudentManagement.Adapters.Persistence.Relational.Mappers;
using PgrStudentManagement.Business.Students.Gateway;
using PgrStudentManagement.Domain.Students;
using PgrStudentManagement.Adapters.Persistence.Relational.Models;

namespace PgrStudentManagement.Adapters.Persistence.Relational.Gateway;

// The gateway should:
// 1. Query EnrolmentRecord objects using StudentManagementDbContext.
// 2. Use AsNoTracking() because the use case is read-only.
// 3. Filter using the supplied student number.
// 4. Order by StartDate for query efficiency.
// 5. Map every persistence record to a Domain Enrolment.
// 6. Return an empty list when no records match.

public class EfEnrolmentGateway : IEnrolmentGateway
{
    private readonly StudentManagementDbContext _context;
    private readonly EnrolmentPersistenceMapper _mapper;

    public EfEnrolmentGateway(StudentManagementDbContext context, EnrolmentPersistenceMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IReadOnlyList<Enrolment>> GetForStudentAsync(string studentNumber, CancellationToken cancellationToken)
    {
        var records = await _context.Enrolments
            .AsNoTracking()
            .Where(record => record.StudentNumber == studentNumber)
                    .OrderBy(record => record.StartDate)
                    .ToListAsync(cancellationToken);

        return records
            .Select(_mapper.ToDomain)
            .ToList();
    }

}
