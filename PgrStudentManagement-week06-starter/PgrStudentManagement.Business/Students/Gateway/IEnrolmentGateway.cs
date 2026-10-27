using PgrStudentManagement.Domain.Students;

namespace PgrStudentManagement.Business.Students.Gateway;
public interface IEnrolmentGateway
{
    Task<IReadOnlyList<Enrolment>> GetForStudentAsync(string studentNumber, CancellationToken cancellationToken);
}

// IEnrolmentGateway is an output port.
// It expresses the external data capability required by Business. 
// It belongs to Business because the use case owns that requirement.
