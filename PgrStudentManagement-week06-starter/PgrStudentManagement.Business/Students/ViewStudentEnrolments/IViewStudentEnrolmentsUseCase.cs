// Use-case DTOs contain simple data crossing the Business boundary.
namespace PgrStudentManagement.Business.Students.ViewStudentEnrolments;

public interface IViewStudentEnrolmentsUseCase
{
    Task<StudentEnrolmentsOutput> ExecuteAsync(ViewStudentEnrolmentsInput input, CancellationToken cancellationToken);
}

// IViewStudentEnrolmentsUseCase is an input port. 
// It defines how MVC and REST adapters invoke the use case.