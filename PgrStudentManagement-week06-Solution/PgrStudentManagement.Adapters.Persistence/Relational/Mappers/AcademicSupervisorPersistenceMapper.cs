using PgrStudentManagement.Adapters.Persistence.Relational.Models;
using PgrStudentManagement.Domain.AcademicSupervisors;

namespace PgrStudentManagement.Adapters.Persistence.Relational.Mappers;

public sealed class AcademicSupervisorPersistenceMapper
{
    // ToDomain : converts an EF database record into a Domain object.
    // ToRecord : converts a Domain object into an EF database record.
    // Database record -> ToDomain -> Business/Domain
    // Business/Domain -> ToRecord -> Database record
    public AcademicSupervisor ToDomain(
        AcademicSupervisorRecord record) =>
        new()
        {
            StaffNumber = record.StaffNumber,
            LastName = record.LastName,
            FirstName = record.FirstName,
            EmailAddress = record.EmailAddress,
            EmploymentType = (SupervisorEmploymentType)record.EmploymentType,
            TrainingCompleted = record.TrainingCompleted,
            EligibleRoleEntries = record.EligibleRoles
                .Select(role => (SupervisoryRole)role.Role)
                .ToList()
        };

    public SupervisoryAssignment ToDomain(
        SupervisoryAssignmentRecord record) =>
        new()
        {
            StudentNumber = record.StudentNumber,
            SupervisorStaffNumber = record.SupervisorStaffNumber,
            Role = (SupervisoryRole)record.Role,
            StartDate = record.StartDate,
            EndDate = record.EndDate,
            ExceptionAuthorised = record.ExceptionAuthorised
        };

    public AcademicSupervisorRecord ToRecord(
        AcademicSupervisor supervisor) =>
        new()
        {
            StaffNumber = supervisor.StaffNumber,
            LastName = supervisor.LastName,
            FirstName = supervisor.FirstName,
            EmailAddress = supervisor.EmailAddress,
            EmploymentType = supervisor.EmploymentType,
            TrainingCompleted = supervisor.TrainingCompleted,
            EligibleRoles = supervisor.EligibleRoleEntries
                .Distinct()
                .Select(role => new AcademicSupervisorEligibleRoleRecord
                {
                    SupervisorStaffNumber = supervisor.StaffNumber,
                    Role = role
                })
                .ToList()
        };

    public SupervisoryAssignmentRecord ToRecord(
        SupervisoryAssignment assignment) =>
        new()
        {
            StudentNumber = assignment.StudentNumber,
            SupervisorStaffNumber = assignment.SupervisorStaffNumber,
            Role = assignment.Role,
            StartDate = assignment.StartDate,
            EndDate = assignment.EndDate,
            ExceptionAuthorised = assignment.ExceptionAuthorised
        };
}
