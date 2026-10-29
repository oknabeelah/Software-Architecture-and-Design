using PgrStudentManagement.Domain.Students;
using PgrStudentManagement.Adapters.Persistence.Relational.Models;

namespace PgrStudentManagement.Adapters.Persistence.Relational.Mappers;
public class EnrolmentPersistenceMapper
{
    public Enrolment ToDomain(EnrolmentRecord record)
    {
        return new Enrolment
        {
            EnrolmentId = record.EnrolmentId,
            StudentNumber = record.StudentNumber,
            ProgrammeCode = record.ProgrammeCode,
            StartDate = record.StartDate,
            ExpectedEndDate = record.ExpectedEndDate,
            ModeOfStudy = record.ModeOfStudy,
            Status = record.Status
        };
    }

}