using PgrStudentManagement.Adapters.Web.ViewModels;
using PgrStudentManagement.Business.Students.ViewStudentEnrolments;

namespace PgrStudentManagement.Adapters.Web.Mappers;

public sealed class StudentEnrolmentsWebMapper
{
    public StudentEnrolmentsViewModel ToViewModel(StudentEnrolmentsOutput output)
    {
        ArgumentNullException.ThrowIfNull(output);

        return new StudentEnrolmentsViewModel
        {
            StudentNumber = output.StudentNumber,
            Enrolments = output.Enrolments
                .Select(enrolment => new EnrolmentViewModel
                {
                    ProgrammeCode = enrolment.ProgrammeCode,
                    StartDate = enrolment.StartDate,
                    ExpectedEndDate = enrolment.ExpectedEndDate?.ToString("yyyy-MM-dd") ?? "Not recorded",
                    ModeOfStudy = enrolment.ModeOfStudy,
                    Status = enrolment.Status
                })
                .ToList()
        };
    }
}