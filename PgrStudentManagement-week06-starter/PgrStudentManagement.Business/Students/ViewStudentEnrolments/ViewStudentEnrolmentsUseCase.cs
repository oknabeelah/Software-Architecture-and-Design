using PgrStudentManagement.Business.Students.Gateway;

namespace PgrStudentManagement.Business.Students.ViewStudentEnrolments;

public class ViewStudentEnrolmentsUseCase : IViewStudentEnrolmentsUseCase
{
    private readonly IEnrolmentGateway _enrolmentGateway;

    public ViewStudentEnrolmentsUseCase(IEnrolmentGateway enrolmentGateway)
    {
        _enrolmentGateway = enrolmentGateway;
    }

    public async Task<StudentEnrolmentsOutput> ExecuteAsync(ViewStudentEnrolmentsInput input, CancellationToken cancellationToken)
    {
        var enrolments = await _enrolmentGateway.GetForStudentAsync(input.StudentNumber, cancellationToken);

        var orderedEnrolments = enrolments
            .OrderBy(e => e.StartDate)
            .Select(e => new EnrolmentOutput(
                e.ProgrammeCode,
                e.StartDate,
                e.ExpectedEndDate,
                e.ModeOfStudy,
                e.Status.ToString()))
            .ToList();

        return new StudentEnrolmentsOutput(input.StudentNumber, orderedEnrolments);
    }
}
//Implement ViewStudentEnrolmentsUseCase using constructor injection of IEnrolmentGateway.
// The use case should:
// 1. Receive ViewStudentEnrolmentsInput.
// 2. Call IEnrolmentGateway.
// 3. Order matching Enrolments from earliest to latest StartDate.
// 4. Map Domain Enrolments to Business output DTOs.
// 5. Return an empty Enrolments collection for the supplied No Enrolments Found flow.
// 6. Avoid modifying any Student or Enrolment information.