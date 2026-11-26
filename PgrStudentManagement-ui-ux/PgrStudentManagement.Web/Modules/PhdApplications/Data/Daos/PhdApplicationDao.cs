using Microsoft.EntityFrameworkCore;
using PgrStudentManagement.Web.Modules.PhdApplications.Models;
using PgrStudentManagement.Web.Modules.Students.Data;

namespace PgrStudentManagement.Web.Modules.PhdApplications.Data.Daos;

public class PhdApplicationDao
{
    private readonly StudentManagementDbContext _context;

    public PhdApplicationDao(StudentManagementDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<PhdApplication>> GetApplicationsAsync(
        ApplicationStatus? statusFilter = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.PhdApplications.AsNoTracking();

        if (statusFilter.HasValue)
        {
            query = query.Where(application =>
                application.Status == statusFilter.Value);
        }

        return await query
            .OrderByDescending(application => application.SubmittedDate)
            .ThenBy(application => application.ApplicationReference)
            .ToListAsync(cancellationToken);
    }

    public async Task<PhdApplication?> GetApplicationByReferenceAsync(
        string applicationReference,
        CancellationToken cancellationToken = default)
    {
        return await _context.PhdApplications
            .AsNoTracking()
            .FirstOrDefaultAsync(
                application =>
                    application.ApplicationReference == applicationReference,
                cancellationToken);
    }

    public async Task<bool> ApplicationExistsAsync(
        string applicationReference,
        CancellationToken cancellationToken = default)
    {
        return await _context.PhdApplications
            .AnyAsync(
                application =>
                    application.ApplicationReference == applicationReference,
                cancellationToken);
    }

    public async Task AddApplicationAsync(
        PhdApplication application,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(application);

        _context.PhdApplications.Add(application);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateApplicationAsync(
        PhdApplication application,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(application);

        var existing = await _context.PhdApplications
            .FirstOrDefaultAsync(
                item =>
                    item.ApplicationReference == application.ApplicationReference,
                cancellationToken);

        if (existing is null)
        {
            return false;
        }

        _context.Entry(existing)
            .CurrentValues
            .SetValues(application);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

