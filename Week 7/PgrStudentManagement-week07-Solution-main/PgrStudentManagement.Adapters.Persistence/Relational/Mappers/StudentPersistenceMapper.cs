using PgrStudentManagement.Adapters.Persistence.Relational.Models;
using PgrStudentManagement.Domain.Students;

namespace PgrStudentManagement.Adapters.Persistence.Relational.Mappers;

public class StudentPersistenceMapper
{
    public Student ToDomain(StudentRecord record)
    {
        return new Student
        {
            StudentNumber = record.StudentNumber,
            FirstName = record.FirstName,
            LastName = record.LastName,
            EmailAddress = record.EmailAddress
        };
    }
}

