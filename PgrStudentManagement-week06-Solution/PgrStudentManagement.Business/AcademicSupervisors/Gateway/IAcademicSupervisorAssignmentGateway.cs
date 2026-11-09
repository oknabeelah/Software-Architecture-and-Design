using PgrStudentManagement.Domain.AcademicSupervisors;

namespace PgrStudentManagement.Business.AcademicSupervisors.Gateway;

public interface IAcademicSupervisorAssignmentGateway
{
    Task AddAsync(SupervisoryAssignment assignment, CancellationToken cancellationToken);
    Task<IReadOnlyList<SupervisoryAssignment>> GetForStudentAsync(string studentNumber, CancellationToken cancellationToken);
}

// I..Gateway

//...UseCase
//I...UseCase
//...Result
//...Input