using PgrStudentManagement.Domain.AcademicSupervisors;

namespace PgrStudentManagement.Business.AcademicSupervisors.Gateway;

public interface IAcademicSupervisorAssignmentGateway
{
    Task<AcademicSupervisor?> GetSupervisorAsync(
        string staffNumber,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SupervisoryAssignment>> GetForStudentAsync(
        string studentNumber,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        SupervisoryAssignment assignment,
        CancellationToken cancellationToken = default);
}

// I..Gateway

//...UseCase
//I...UseCase
//...Result
//...Input