using Microsoft.EntityFrameworkCore;
using PgrStudentManagement.Adapters.Persistence.Relational.Models;

namespace PgrStudentManagement.Adapters.Persistence.Relational;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(StudentManagementDbContext context)
    {
        if (await context.Students.AnyAsync())
        {
            return;
        }

        var students = new List<StudentRecord>
        {
            new() { StudentNumber = "S100001", FirstName = "John", LastName = "Doe", EmailAddress = "john.doe@example.test" },
            new() { StudentNumber = "S100002", FirstName = "Jane", LastName = "Smith", EmailAddress = "jane.smith@example.test" },
            new() { StudentNumber = "S100003", FirstName = "Casey", LastName = "Wilson", EmailAddress = "casey.wilson@example.test" },
            new() { StudentNumber = "S100004", FirstName = "Morgan", LastName = "Lee", EmailAddress = "morgan.lee@example.test" }
        };

        await context.Students.AddRangeAsync(students);
        await context.SaveChangesAsync();

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
}
