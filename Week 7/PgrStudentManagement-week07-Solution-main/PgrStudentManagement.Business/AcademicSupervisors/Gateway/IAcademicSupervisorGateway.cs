using PgrStudentManagement.Domain.AcademicSupervisors;

namespace PgrStudentManagement.Business.AcademicSupervisors.Gateway;

public interface IAcademicSupervisorGateway
{
    Task<IReadOnlyList<AcademicSupervisor>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<AcademicSupervisor?> GetByStaffNumberAsync(
        string staffNumber,
        CancellationToken cancellationToken = default);

    Task<bool> StaffNumberExistsAsync(
        string staffNumber,
        CancellationToken cancellationToken = default);

    Task AddSupervisorAsync(
        AcademicSupervisor supervisor,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SupervisoryAssignment>> GetAssignmentsForStudentAsync(
        string studentNumber,
        CancellationToken cancellationToken = default);

    Task<bool> AssignmentExistsAsync(
        string studentNumber,
        string staffNumber,
        SupervisoryRole role,
        DateTime startDate,
        CancellationToken cancellationToken = default);

    Task AddAssignmentAsync(
        SupervisoryAssignment assignment,
        CancellationToken cancellationToken = default);
}

