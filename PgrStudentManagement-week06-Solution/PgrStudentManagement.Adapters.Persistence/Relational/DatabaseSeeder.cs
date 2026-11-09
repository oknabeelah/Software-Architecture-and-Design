using Microsoft.EntityFrameworkCore;
using PgrStudentManagement.Adapters.Persistence.Relational.Models;
using PgrStudentManagement.Domain.AcademicSupervisors;

namespace PgrStudentManagement.Adapters.Persistence.Relational;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(StudentManagementDbContext context)
    {
        if (!await context.Students.AnyAsync())
        {
            var students = new List<StudentRecord>
            {
                new() { StudentNumber = "S100001", FirstName = "John", LastName = "Doe", EmailAddress = "john.doe@example.test" },
                new() { StudentNumber = "S100002", FirstName = "Jane", LastName = "Smith", EmailAddress = "jane.smith@example.test" },
                new() { StudentNumber = "S100003", FirstName = "Casey", LastName = "Wilson", EmailAddress = "casey.wilson@example.test" },
                new() { StudentNumber = "S100004", FirstName = "Morgan", LastName = "Lee", EmailAddress = "morgan.lee@example.test" }
            };

            await context.Students.AddRangeAsync(students);

            var enrolments = new List<EnrolmentRecord>
            {
                new()
                {
                    EnrolmentId = 1,
                    StudentNumber = "S100001",
                    ProgrammeCode = "PHD-COMP",
                    StartDate = new DateTime(2024, 10, 1),
                    ExpectedEndDate = new DateTime(2027, 9, 30),
                    ModeOfStudy = "Full-time",
                    Status = "Active"
                },
                new()
                {
                    EnrolmentId = 2,
                    StudentNumber = "S100002",
                    ProgrammeCode = "MPHIL-COMP",
                    StartDate = new DateTime(2022, 10, 1),
                    ExpectedEndDate = new DateTime(2024, 9, 30),
                    ModeOfStudy = "Full-time",
                    Status = "Completed"
                },
                new()
                {
                    EnrolmentId = 3,
                    StudentNumber = "S100002",
                    ProgrammeCode = "PHD-COMP",
                    StartDate = new DateTime(2024, 10, 1),
                    ExpectedEndDate = new DateTime(2030, 9, 30),
                    ModeOfStudy = "Part-time",
                    Status = "Active"
                },
                new()
                {
                    EnrolmentId = 4,
                    StudentNumber = "S100003",
                    ProgrammeCode = "PHD-DESIGN",
                    StartDate = new DateTime(2025, 10, 1),
                    ExpectedEndDate = null,
                    ModeOfStudy = "Full-time",
                    Status = "Suspended"
                }
            };

            await context.Enrolments.AddRangeAsync(enrolments);
            await context.SaveChangesAsync();
        }

        if (!await context.AcademicSupervisors.AnyAsync())
        {
            await context.AcademicSupervisors.AddRangeAsync(
                CreateSupervisor(
                    "AS1001",
                    "Robin",
                    "Carter",
                    SupervisorEmploymentType.InstitutionEmployed,
                    SupervisoryRole.DirectorOfStudies,
                    SupervisoryRole.FirstSupervisor,
                    SupervisoryRole.SecondSupervisor),
                CreateSupervisor(
                    "AS1002",
                    "Jamie",
                    "Ahmed",
                    SupervisorEmploymentType.InstitutionEmployed,
                    SupervisoryRole.DirectorOfStudies,
                    SupervisoryRole.FirstSupervisor,
                    SupervisoryRole.SecondSupervisor),
                CreateSupervisor(
                    "AS1003",
                    "Taylor",
                    "Green",
                    SupervisorEmploymentType.External,
                    SupervisoryRole.SecondSupervisor,
                    SupervisoryRole.Adviser),
                CreateSupervisor(
                    "AS1004",
                    "Avery",
                    "Brown",
                    SupervisorEmploymentType.InstitutionEmployed,
                    SupervisoryRole.FirstSupervisor,
                    SupervisoryRole.SecondSupervisor,
                    SupervisoryRole.Adviser));
            await context.SaveChangesAsync();
        }

        if (!await context.SupervisoryAssignments.AnyAsync())
        {
            await context.SupervisoryAssignments.AddRangeAsync(
                new SupervisoryAssignmentRecord
                {
                    StudentNumber = "S100001",
                    SupervisorStaffNumber = "AS1001",
                    Role = SupervisoryRole.DirectorOfStudies,
                    StartDate = new DateTime(2025, 10, 1)
                },
                new SupervisoryAssignmentRecord
                {
                    StudentNumber = "S100001",
                    SupervisorStaffNumber = "AS1002",
                    Role = SupervisoryRole.FirstSupervisor,
                    StartDate = new DateTime(2025, 10, 1)
                },
                new SupervisoryAssignmentRecord
                {
                    StudentNumber = "S100001",
                    SupervisorStaffNumber = "AS1003",
                    Role = SupervisoryRole.SecondSupervisor,
                    StartDate = new DateTime(2024, 10, 1),
                    EndDate = new DateTime(2025, 9, 30)
                },
                new SupervisoryAssignmentRecord
                {
                    StudentNumber = "S100002",
                    SupervisorStaffNumber = "AS1001",
                    Role = SupervisoryRole.FirstSupervisor,
                    StartDate = new DateTime(2026, 1, 15)
                },
                new SupervisoryAssignmentRecord
                {
                    StudentNumber = "S100002",
                    SupervisorStaffNumber = "AS1002",
                    Role = SupervisoryRole.DirectorOfStudies,
                    StartDate = new DateTime(2025, 10, 1)
                },
                new SupervisoryAssignmentRecord
                {
                    StudentNumber = "S100002",
                    SupervisorStaffNumber = "AS1003",
                    Role = SupervisoryRole.Adviser,
                    StartDate = new DateTime(2025, 10, 1)
                });
            await context.SaveChangesAsync();
        }
    }

    private static AcademicSupervisorRecord CreateSupervisor(
        string staffNumber,
        string firstName,
        string lastName,
        SupervisorEmploymentType employmentType,
        params SupervisoryRole[] eligibleRoles)
    {
        return new AcademicSupervisorRecord
        {
            StaffNumber = staffNumber,
            FirstName = firstName,
            LastName = lastName,
            EmailAddress = $"{firstName}.{lastName}@example.test".ToLowerInvariant(),
            EmploymentType = employmentType,
            TrainingCompleted = true,
            EligibleRoles = eligibleRoles
                .Distinct()
                .Select(role => new AcademicSupervisorEligibleRoleRecord
                {
                    SupervisorStaffNumber = staffNumber,
                    Role = role
                })
                .ToList()
        };
    }
}
