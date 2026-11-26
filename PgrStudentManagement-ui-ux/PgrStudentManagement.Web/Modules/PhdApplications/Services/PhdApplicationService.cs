using PgrStudentManagement.Web.Modules.AcademicSupervisors.Contracts;
using PgrStudentManagement.Web.Modules.PhdApplications.Data.Daos;
using PgrStudentManagement.Web.Modules.PhdApplications.Models;
using PgrStudentManagement.Web.Modules.Students.Contracts;

namespace PgrStudentManagement.Web.Modules.PhdApplications.Services;

public class PhdApplicationService
{
    private readonly PhdApplicationDao _phdApplicationDao;
    private readonly ISupervisorLookup _supervisorLookup;
    private readonly IStudentLookup _studentLookup;

    public PhdApplicationService(
        PhdApplicationDao phdApplicationDao,
        ISupervisorLookup supervisorLookup,
        IStudentLookup studentLookup)
    {
        _phdApplicationDao = phdApplicationDao;
        _supervisorLookup = supervisorLookup;
        _studentLookup = studentLookup;
    }

    public Task<IReadOnlyList<PhdApplication>> GetApplicationsAsync(
        ApplicationStatus? statusFilter = null,
        CancellationToken cancellationToken = default)
    {
        return _phdApplicationDao.GetApplicationsAsync(
            statusFilter,
            cancellationToken);
    }

    public Task<PhdApplication?> GetApplicationByReferenceAsync(
        string applicationReference,
        CancellationToken cancellationToken = default)
    {
        return _phdApplicationDao.GetApplicationByReferenceAsync(
            applicationReference,
            cancellationToken);
    }

    public Task<IReadOnlyList<SupervisorSummary>> GetAvailableSupervisorsAsync(
        CancellationToken cancellationToken = default)
    {
        return _supervisorLookup.GetSupervisorSummariesAsync(cancellationToken);
    }

    /// <summary>
    /// Helper for Lab Part 2: Returns only supervisors who are eligible
    /// to act as a Main Supervisor (Director of Studies or First Supervisor).
    /// </summary>
    public async Task<IReadOnlyList<SupervisorSummary>>
        GetEligibleMainSupervisorsAsync(
            CancellationToken cancellationToken = default)
    {
        var allSupervisors =
            await _supervisorLookup.GetSupervisorSummariesAsync(
                cancellationToken);

        return allSupervisors
            .Where(supervisor => supervisor.IsEligibleMainSupervisor)
            .ToList();
    }

    /// <summary>
    /// Helper for Lab Part 1 &amp; Part 7: Generates the next sequential
    /// application reference (e.g., APP-2026-005) on the server so the
    /// user does not have to invent one.
    /// </summary>
    public async Task<string> GenerateNextApplicationReferenceAsync(
        CancellationToken cancellationToken = default)
    {
        var existing = await _phdApplicationDao.GetApplicationsAsync(
            statusFilter: null,
            cancellationToken);

        var nextNumber = existing.Count + 1;
        var year = DateTime.UtcNow.Year;
        var candidate = $"APP-{year}-{nextNumber:D3}";

        while (await _phdApplicationDao.ApplicationExistsAsync(
                   candidate,
                   cancellationToken))
        {
            nextNumber++;
            candidate = $"APP-{year}-{nextNumber:D3}";
        }

        return candidate;
    }

    public async Task<ApplicationOutcome> CreateApplicationAsync(
        PhdApplication application,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(application);

        var exists = await _phdApplicationDao.ApplicationExistsAsync(
            application.ApplicationReference,
            cancellationToken);

        if (exists)
        {
            return ApplicationOutcome.DuplicateReference;
        }

        if (!string.IsNullOrWhiteSpace(
                application.ProposedSupervisorStaffNumber))
        {
            var supervisor =
                await _supervisorLookup.GetSupervisorSummaryAsync(
                    application.ProposedSupervisorStaffNumber,
                    cancellationToken);

            if (supervisor is null)
            {
                return ApplicationOutcome.SupervisorNotFound;
            }

            if (!supervisor.IsEligibleMainSupervisor)
            {
                return ApplicationOutcome.SupervisorNotEligible;
            }
        }
        else
        {
            application.ProposedSupervisorStaffNumber = null;
        }

        await _phdApplicationDao.AddApplicationAsync(
            application,
            cancellationToken);

        return ApplicationOutcome.Created;
    }

    public async Task<ApplicationOutcome> UpdateApplicationStatusAsync(
        string applicationReference,
        ApplicationStatus newStatus,
        DateTime? decisionDate = null,
        string? enrolledStudentNumber = null,
        CancellationToken cancellationToken = default)
    {
        var application =
            await _phdApplicationDao.GetApplicationByReferenceAsync(
                applicationReference,
                cancellationToken);

        if (application is null)
        {
            return ApplicationOutcome.ApplicationNotFound;
        }

        if (!IsValidStatusTransition(application.Status, newStatus))
        {
            return ApplicationOutcome.InvalidStatusTransition;
        }

        if (newStatus == ApplicationStatus.Enrolled)
        {
            if (string.IsNullOrWhiteSpace(enrolledStudentNumber))
            {
                return ApplicationOutcome.StudentNotFound;
            }

            var student = await _studentLookup.GetStudentSummaryAsync(
                enrolledStudentNumber.Trim(),
                cancellationToken);

            if (student is null)
            {
                return ApplicationOutcome.StudentNotFound;
            }

            application.EnrolledStudentNumber = student.StudentNumber;
        }

        if (newStatus is ApplicationStatus.OfferMade
            or ApplicationStatus.Rejected)
        {
            application.DecisionDate =
                (decisionDate ?? DateTime.UtcNow).Date;
        }
        else if (decisionDate.HasValue)
        {
            application.DecisionDate = decisionDate.Value.Date;
        }

        application.Status = newStatus;

        await _phdApplicationDao.UpdateApplicationAsync(
            application,
            cancellationToken);

        return ApplicationOutcome.Updated;
    }

    /// <summary>
    /// Helper for Lab Part 8: Returns the statuses that an application is
    /// allowed to transition to from its current status.
    /// </summary>
    public IReadOnlyList<ApplicationStatus> GetAllowedTransitions(
        ApplicationStatus currentStatus)
    {
        return currentStatus switch
        {
            ApplicationStatus.Submitted =>
            [
                ApplicationStatus.UnderReview,
                ApplicationStatus.Withdrawn
            ],
            ApplicationStatus.UnderReview =>
            [
                ApplicationStatus.InterviewScheduled,
                ApplicationStatus.OfferMade,
                ApplicationStatus.Rejected,
                ApplicationStatus.Withdrawn
            ],
            ApplicationStatus.InterviewScheduled =>
            [
                ApplicationStatus.OfferMade,
                ApplicationStatus.Rejected,
                ApplicationStatus.Withdrawn
            ],
            ApplicationStatus.OfferMade =>
            [
                ApplicationStatus.OfferAccepted,
                ApplicationStatus.Withdrawn
            ],
            ApplicationStatus.OfferAccepted =>
            [
                ApplicationStatus.Enrolled,
                ApplicationStatus.Withdrawn
            ],
            ApplicationStatus.Rejected => [],
            ApplicationStatus.Withdrawn => [],
            ApplicationStatus.Enrolled => [],
            _ => []
        };
    }

    private static bool IsValidStatusTransition(
        ApplicationStatus currentStatus,
        ApplicationStatus newStatus)
    {
        if (currentStatus == newStatus)
        {
            return true;
        }

        return currentStatus switch
        {
            ApplicationStatus.Rejected => false,
            ApplicationStatus.Withdrawn => false,
            ApplicationStatus.Enrolled => false,
            _ => true
        };
    }
}
