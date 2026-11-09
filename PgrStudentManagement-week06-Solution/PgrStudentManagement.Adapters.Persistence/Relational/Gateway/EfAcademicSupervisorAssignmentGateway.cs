using Microsoft.EntityFrameworkCore;
using PgrStudentManagement.Adapters.Persistence.Relational.Mappers;
using PgrStudentManagement.Business.AcademicSupervisors.Gateway;
using PgrStudentManagement.Domain.AcademicSupervisors;

namespace PgrStudentManagement.Adapters.Persistence.Relational.Gateway;

public sealed class EfAcademicSupervisorAssignmentGateway
    : IAcademicSupervisorAssignmentGateway
{
    private readonly StudentManagementDbContext _context;
    private readonly AcademicSupervisorPersistenceMapper _mapper;

    public EfAcademicSupervisorAssignmentGateway(
        StudentManagementDbContext context,
        AcademicSupervisorPersistenceMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<AcademicSupervisor?> GetSupervisorAsync(
        string staffNumber,
        CancellationToken cancellationToken = default)
    {
        var record = await _context.AcademicSupervisors
            .AsNoTracking()
            .Include(supervisor => supervisor.EligibleRoles)
            .SingleOrDefaultAsync(
                supervisor => supervisor.StaffNumber == staffNumber,
                cancellationToken);

        return record is null ? null : _mapper.ToDomain(record);
    }

    public async Task<IReadOnlyList<SupervisoryAssignment>> GetForStudentAsync(
        string studentNumber,
        CancellationToken cancellationToken = default)
    {
        var records = await _context.SupervisoryAssignments
            .AsNoTracking()
            .Where(assignment => assignment.StudentNumber == studentNumber)
            .OrderBy(assignment => assignment.StartDate)
            .ToListAsync(cancellationToken);

        return records.Select(_mapper.ToDomain).ToList();
    }

    public async Task AddAsync(
        SupervisoryAssignment assignment,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(assignment);

        _context.SupervisoryAssignments.Add(_mapper.ToRecord(assignment));
        await _context.SaveChangesAsync(cancellationToken);
    }
}
