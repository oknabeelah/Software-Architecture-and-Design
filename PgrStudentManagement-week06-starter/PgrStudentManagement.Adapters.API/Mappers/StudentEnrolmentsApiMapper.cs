using PgrStudentManagement.Adapters.API.Responses;
using PgrStudentManagement.Business.Students.ViewStudentEnrolments;

namespace PgrStudentManagement.Adapters.API.Mappers;

public sealed class StudentEnrolmentsApiMapper
{
    public StudentEnrolmentsResponse ToResponse(StudentEnrolmentsOutput output)
    {
        ArgumentNullException.ThrowIfNull(output);

        return new StudentEnrolmentsResponse(
            output.StudentNumber,
            output.Enrolments
                .Select(enrolment => new EnrolmentResponse(
                    enrolment.ProgrammeCode,
                    enrolment.StartDate,
                    enrolment.ExpectedEndDate,
                    enrolment.ModeOfStudy,
                    enrolment.Status))
                .ToList());
    }
}
