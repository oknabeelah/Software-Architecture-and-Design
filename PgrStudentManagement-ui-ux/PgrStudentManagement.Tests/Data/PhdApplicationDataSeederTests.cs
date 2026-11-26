using Microsoft.EntityFrameworkCore;
using PgrStudentManagement.Tests.Fixtures;
using PgrStudentManagement.Web.Modules.AcademicSupervisors.Data;
using PgrStudentManagement.Web.Modules.AcademicSupervisors.Data.Daos;
using PgrStudentManagement.Web.Modules.AcademicSupervisors.Services;
using PgrStudentManagement.Web.Modules.PhdApplications.Data;
using PgrStudentManagement.Web.Modules.PhdApplications.Data.Daos;
using PgrStudentManagement.Web.Modules.Students.Data;
using PgrStudentManagement.Web.Modules.Students.Data.Daos;
using PgrStudentManagement.Web.Modules.Students.Services;
using Xunit;

namespace PgrStudentManagement.Tests.Data;

public sealed class PhdApplicationDataSeederTests
{
    [Fact]
    public async Task SeedAsync_AddsPhdApplications()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();

        var studentDao = new StudentDao(database.Context);
        var enrolmentDao = new EnrolmentDao(database.Context);
        var studentService = new StudentService(studentDao, enrolmentDao);

        await new StudentDataSeeder(studentDao).SeedAsync();

        var supervisorDao = new AcademicSupervisorDao(database.Context);
        var supervisorService =
            new AcademicSupervisorService(supervisorDao, studentService);

        await new AcademicSupervisorDataSeeder(
            supervisorDao,
            studentService).SeedAsync();

        var applicationDao = new PhdApplicationDao(database.Context);
        var seeder = new PhdApplicationDataSeeder(
            applicationDao,
            supervisorService,
            studentService);

        await seeder.SeedAsync();

        Assert.NotEmpty(
            await database.Context.PhdApplications.ToListAsync());
    }

    [Fact]
    public async Task SeedAsync_WhenCalledTwice_DoesNotDuplicateData()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();

        var studentDao = new StudentDao(database.Context);
        var enrolmentDao = new EnrolmentDao(database.Context);
        var studentService = new StudentService(studentDao, enrolmentDao);

        await new StudentDataSeeder(studentDao).SeedAsync();

        var supervisorDao = new AcademicSupervisorDao(database.Context);
        var supervisorService =
            new AcademicSupervisorService(supervisorDao, studentService);

        await new AcademicSupervisorDataSeeder(
            supervisorDao,
            studentService).SeedAsync();

        var applicationDao = new PhdApplicationDao(database.Context);
        var seeder = new PhdApplicationDataSeeder(
            applicationDao,
            supervisorService,
            studentService);

        await seeder.SeedAsync();

        int countAfterFirstSeed =
            await database.Context.PhdApplications.CountAsync();

        database.Context.ChangeTracker.Clear();

        await seeder.SeedAsync();

        Assert.Equal(
            countAfterFirstSeed,
            await database.Context.PhdApplications.CountAsync());
    }
}

