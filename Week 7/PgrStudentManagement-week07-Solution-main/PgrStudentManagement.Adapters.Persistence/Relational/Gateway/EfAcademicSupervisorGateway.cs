using Microsoft.EntityFrameworkCore;
using PgrStudentManagement.Adapters.Persistence.Relational.Mappers;
using PgrStudentManagement.Business.AcademicSupervisors.Gateway;
using PgrStudentManagement.Domain.AcademicSupervisors;

namespace PgrStudentManagement.Adapters.Persistence.Relational.Gateway;

public class EfAcademicSupervisorGateway : IAcademicSupervisorGateway
{
    private readonly StudentManagementDbContext _context;
    private readonly AcademicSupervisorPersistenceMapper _mapper;

    public EfAcademicSupervisorGateway(
        StudentManagementDbContext context,
        AcademicSupervisorPersistenceMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<AcademicSupervisor>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var records = await _context.AcademicSupervisors
            .AsNoTracking()
            .Include(supervisor => supervisor.EligibleRoleEntries)
            .OrderBy(supervisor => supervisor.LastName)
            .ThenBy(supervisor => supervisor.FirstName)
            .ThenBy(supervisor => supervisor.StaffNumber)
            .ToListAsync(cancellationToken);

        return records
            .Select(_mapper.ToDomain)
            .ToList();
    }

    public async Task<AcademicSupervisor?> GetByStaffNumberAsync(
        string staffNumber,
        CancellationToken cancellationToken = default)
    {
        var record = await _context.AcademicSupervisors
            .AsNoTracking()
            .Include(supervisor => supervisor.EligibleRoleEntries)
            .FirstOrDefaultAsync(
                supervisor => supervisor.StaffNumber == staffNumber,
                cancellationToken);

        return record is null
            ? null
            : _mapper.ToDomain(record);
    }

    public async Task<bool> StaffNumberExistsAsync(
        string staffNumber,
        CancellationToken cancellationToken = default)
    {
        return await _context.AcademicSupervisors
            .AnyAsync(
                supervisor => supervisor.StaffNumber == staffNumber,
                cancellationToken);
    }

    public async Task AddSupervisorAsync(
        AcademicSupervisor supervisor,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(supervisor);

        var record = _mapper.ToRecord(supervisor);
        _context.AcademicSupervisors.Add(record);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SupervisoryAssignment>> GetAssignmentsForStudentAsync(
        string studentNumber,
        CancellationToken cancellationToken = default)
    {
        var records = await _context.SupervisoryAssignments
            .AsNoTracking()
            .Where(assignment => assignment.StudentNumber == studentNumber)
            .OrderBy(assignment => assignment.StartDate)
            .ToListAsync(cancellationToken);

        return records
            .Select(_mapper.ToDomain)
            .ToList();
    }

    public async Task<bool> AssignmentExistsAsync(
        string studentNumber,
        string staffNumber,
        SupervisoryRole role,
        DateTime startDate,
        CancellationToken cancellationToken = default)
    {
        var roleString = role.ToString();
        var normalizedStartDate = startDate.Date;

        return await _context.SupervisoryAssignments
            .AnyAsync(
                assignment =>
                    assignment.StudentNumber == studentNumber
                    && assignment.SupervisorStaffNumber == staffNumber
                    && assignment.Role == roleString
                    && assignment.StartDate == normalizedStartDate,
                cancellationToken);
    }

    public async Task AddAssignmentAsync(
        SupervisoryAssignment assignment,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(assignment);

        var record = _mapper.ToRecord(assignment);
        _context.SupervisoryAssignments.Add(record);

        await _context.SaveChangesAsync(cancellationToken);
    }
}

