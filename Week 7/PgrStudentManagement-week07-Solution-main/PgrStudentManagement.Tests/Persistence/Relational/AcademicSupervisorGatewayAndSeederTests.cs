using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PgrStudentManagement.Adapters.Persistence.Relational;
using PgrStudentManagement.Adapters.Persistence.Relational.Gateway;
using PgrStudentManagement.Adapters.Persistence.Relational.Mappers;
using PgrStudentManagement.Domain.AcademicSupervisors;

namespace PgrStudentManagement.Tests.Persistence.Relational;

public class AcademicSupervisorGatewayAndSeederTests
{
    [Fact]
    public async Task SeedAsync_WhenCalledTwice_IsIdempotentAndSeedsSupervisorsRolesAndAssignments()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<StudentManagementDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new StudentManagementDbContext(options);
        await context.Database.MigrateAsync();
        await DatabaseSeeder.SeedAsync(context);

        var studentGateway = new EfStudentGateway(context, new StudentPersistenceMapper());
        var supervisorGateway = new EfAcademicSupervisorGateway(context, new AcademicSupervisorPersistenceMapper());
        var seeder = new AcademicSupervisorDataSeeder(supervisorGateway, studentGateway);

        await seeder.SeedAsync();

        var supervisorCount = await context.AcademicSupervisors.CountAsync();
        var roleCount = await context.AcademicSupervisorEligibleRoles.CountAsync();
        var assignmentCount = await context.SupervisoryAssignments.CountAsync();

        Assert.Equal(4, supervisorCount);
        Assert.True(roleCount > 0);
        Assert.Equal(6, assignmentCount);

        context.ChangeTracker.Clear();

        // Second call must not duplicate rows or throw constraint errors
        await seeder.SeedAsync();

        Assert.Equal(supervisorCount, await context.AcademicSupervisors.CountAsync());
        Assert.Equal(roleCount, await context.AcademicSupervisorEligibleRoles.CountAsync());
        Assert.Equal(assignmentCount, await context.SupervisoryAssignments.CountAsync());

        var supervisor = await supervisorGateway.GetByStaffNumberAsync("AS1001");
        Assert.NotNull(supervisor);
        Assert.True(supervisor.IsEligibleFor(SupervisoryRole.DirectorOfStudies));
        Assert.False(supervisor.IsEligibleFor(SupervisoryRole.Adviser));
    }
}

