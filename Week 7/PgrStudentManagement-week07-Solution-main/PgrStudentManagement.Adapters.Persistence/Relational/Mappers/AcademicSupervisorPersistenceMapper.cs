using PgrStudentManagement.Adapters.Persistence.Relational.Models;
using PgrStudentManagement.Domain.AcademicSupervisors;

namespace PgrStudentManagement.Adapters.Persistence.Relational.Mappers;

public class AcademicSupervisorPersistenceMapper
{
    public AcademicSupervisor ToDomain(AcademicSupervisorRecord record)
    {
        var eligibleRoles = record.EligibleRoleEntries
            .Select(entry => Enum.Parse<SupervisoryRole>(entry.Role, ignoreCase: true))
            .ToHashSet();

        var employmentType = Enum.TryParse<SupervisorEmploymentType>(
            record.EmploymentType,
            ignoreCase: true,
            out var parsedEmploymentType)
            ? parsedEmploymentType
            : SupervisorEmploymentType.InstitutionEmployed;

        return new AcademicSupervisor
        {
            StaffNumber = record.StaffNumber,
            FirstName = record.FirstName,
            LastName = record.LastName,
            EmailAddress = record.EmailAddress,
            EmploymentType = employmentType,
            TrainingCompleted = record.TrainingCompleted,
            EligibleRoles = eligibleRoles
        };
    }

    public AcademicSupervisorRecord ToRecord(AcademicSupervisor domain)
    {
        return new AcademicSupervisorRecord
        {
            StaffNumber = domain.StaffNumber,
            FirstName = domain.FirstName,
            LastName = domain.LastName,
            EmailAddress = domain.EmailAddress,
            EmploymentType = domain.EmploymentType.ToString(),
            TrainingCompleted = domain.TrainingCompleted,
            EligibleRoleEntries = domain.EligibleRoles
                .Distinct()
                .Select(role => new AcademicSupervisorEligibleRoleRecord
                {
                    SupervisorStaffNumber = domain.StaffNumber,
                    Role = role.ToString()
                })
                .ToList()
        };
    }

    public SupervisoryAssignment ToDomain(SupervisoryAssignmentRecord record)
    {
        return new SupervisoryAssignment
        {
            StudentNumber = record.StudentNumber,
            SupervisorStaffNumber = record.SupervisorStaffNumber,
            Role = Enum.Parse<SupervisoryRole>(record.Role, ignoreCase: true),
            StartDate = record.StartDate,
            EndDate = record.EndDate,
            ExceptionAuthorised = record.ExceptionAuthorised
        };
    }

    public SupervisoryAssignmentRecord ToRecord(SupervisoryAssignment domain)
    {
        return new SupervisoryAssignmentRecord
        {
            StudentNumber = domain.StudentNumber,
            SupervisorStaffNumber = domain.SupervisorStaffNumber,
            Role = domain.Role.ToString(),
            StartDate = domain.StartDate.Date,
            EndDate = domain.EndDate?.Date,
            ExceptionAuthorised = domain.ExceptionAuthorised
        };
    }
}

