using Microsoft.AspNetCore.Mvc;
using PgrStudentManagement.Adapters.API.Controllers;
using PgrStudentManagement.Adapters.API.Mappers;
using PgrStudentManagement.Adapters.API.Requests;
using PgrStudentManagement.Adapters.API.Responses;
using PgrStudentManagement.Business.AcademicSupervisors.AssignAcademicSupervisor;
using PgrStudentManagement.Business.AcademicSupervisors.ViewSupervisoryTeam;
using PgrStudentManagement.Domain.AcademicSupervisors;
using PgrStudentManagement.Tests.TestDoubles;

namespace PgrStudentManagement.Tests.API.Controllers;

public class AcademicSupervisorsApiControllerTests
{
    [Fact]
    public async Task AssignSupervisor_WhenValid_Returns201Created()
    {
        var gateway = new FakeAcademicSupervisorGateway();
        await gateway.AddSupervisorAsync(new AcademicSupervisor
        {
            StaffNumber = "AS1004",
            FirstName = "Avery",
            LastName = "Brown",
            EmailAddress = "avery.brown@example.test",
            EmploymentType = SupervisorEmploymentType.InstitutionEmployed,
            TrainingCompleted = true,
            EligibleRoles = new HashSet<SupervisoryRole> { SupervisoryRole.SecondSupervisor }
        });

        var studentLookup = new FakeStudentLookup();
        studentLookup.AddStudent("S100001", "John Doe");

        var assignUseCase = new AssignAcademicSupervisorUseCase(gateway, studentLookup);
        var viewUseCase = new ViewSupervisoryTeamUseCase(gateway, studentLookup);
        var mapper = new AcademicSupervisorsApiMapper();
        var controller = new AcademicSupervisorsApiController(assignUseCase, mapper, viewUseCase);

        var actionResult = await controller.AssignSupervisor(
            "S100001",
            new AssignAcademicSupervisorRequest
            {
                StaffNumber = "AS1004",
                Role = SupervisoryRole.SecondSupervisor,
                StartDate = new DateTime(2026, 10, 1),
                ExceptionAuthorised = false
            },
            CancellationToken.None);

        var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
        Assert.Equal(201, objectResult.StatusCode);

        var response = Assert.IsType<AssignAcademicSupervisorResponse>(objectResult.Value);
        Assert.Equal("Created", response.Outcome);
        Assert.NotNull(response.Assignment);
        Assert.Equal("AS1004", response.Assignment.SupervisorStaffNumber);
    }

    [Fact]
    public async Task AssignSupervisor_WhenStudentNotFound_Returns404NotFound()
    {
        var gateway = new FakeAcademicSupervisorGateway();
        var studentLookup = new FakeStudentLookup();

        var assignUseCase = new AssignAcademicSupervisorUseCase(gateway, studentLookup);
        var mapper = new AcademicSupervisorsApiMapper();
        var controller = new AcademicSupervisorsApiController(assignUseCase, mapper);

        var actionResult = await controller.AssignSupervisor(
            "UNKNOWN",
            new AssignAcademicSupervisorRequest
            {
                StaffNumber = "AS1004",
                Role = SupervisoryRole.SecondSupervisor,
                StartDate = new DateTime(2026, 10, 1)
            },
            CancellationToken.None);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal(404, notFoundResult.StatusCode);

        var response = Assert.IsType<AssignAcademicSupervisorResponse>(notFoundResult.Value);
        Assert.Equal("StudentNotFound", response.Outcome);
    }

    [Fact]
    public async Task AssignSupervisor_WhenRoleAlreadyFilled_Returns409Conflict()
    {
        var gateway = new FakeAcademicSupervisorGateway();
        await gateway.AddSupervisorAsync(new AcademicSupervisor
        {
            StaffNumber = "AS1001",
            FirstName = "Robin",
            LastName = "Carter",
            EmailAddress = "robin.carter@example.test",
            EmploymentType = SupervisorEmploymentType.InstitutionEmployed,
            TrainingCompleted = true,
            EligibleRoles = new HashSet<SupervisoryRole> { SupervisoryRole.FirstSupervisor }
        });
        await gateway.AddSupervisorAsync(new AcademicSupervisor
        {
            StaffNumber = "AS1004",
            FirstName = "Avery",
            LastName = "Brown",
            EmailAddress = "avery.brown@example.test",
            EmploymentType = SupervisorEmploymentType.InstitutionEmployed,
            TrainingCompleted = true,
            EligibleRoles = new HashSet<SupervisoryRole> { SupervisoryRole.FirstSupervisor }
        });
        await gateway.AddAssignmentAsync(new SupervisoryAssignment
        {
            StudentNumber = "S100001",
            SupervisorStaffNumber = "AS1001",
            Role = SupervisoryRole.FirstSupervisor,
            StartDate = new DateTime(2025, 10, 1)
        });

        var studentLookup = new FakeStudentLookup();
        studentLookup.AddStudent("S100001", "John Doe");

        var assignUseCase = new AssignAcademicSupervisorUseCase(gateway, studentLookup);
        var mapper = new AcademicSupervisorsApiMapper();
        var controller = new AcademicSupervisorsApiController(assignUseCase, mapper);

        var actionResult = await controller.AssignSupervisor(
            "S100001",
            new AssignAcademicSupervisorRequest
            {
                StaffNumber = "AS1004",
                Role = SupervisoryRole.FirstSupervisor,
                StartDate = new DateTime(2026, 10, 1)
            },
            CancellationToken.None);

        var conflictResult = Assert.IsType<ConflictObjectResult>(actionResult.Result);
        Assert.Equal(409, conflictResult.StatusCode);

        var response = Assert.IsType<AssignAcademicSupervisorResponse>(conflictResult.Value);
        Assert.Equal("RoleAlreadyFilled", response.Outcome);
    }

    [Fact]
    public async Task AssignSupervisor_WhenSupervisorNotEligible_Returns422UnprocessableEntity()
    {
        var gateway = new FakeAcademicSupervisorGateway();
        await gateway.AddSupervisorAsync(new AcademicSupervisor
        {
            StaffNumber = "AS1004",
            FirstName = "Avery",
            LastName = "Brown",
            EmailAddress = "avery.brown@example.test",
            EmploymentType = SupervisorEmploymentType.InstitutionEmployed,
            TrainingCompleted = true,
            EligibleRoles = new HashSet<SupervisoryRole> { SupervisoryRole.Adviser }
        });

        var studentLookup = new FakeStudentLookup();
        studentLookup.AddStudent("S100001", "John Doe");

        var assignUseCase = new AssignAcademicSupervisorUseCase(gateway, studentLookup);
        var mapper = new AcademicSupervisorsApiMapper();
        var controller = new AcademicSupervisorsApiController(assignUseCase, mapper);

        var actionResult = await controller.AssignSupervisor(
            "S100001",
            new AssignAcademicSupervisorRequest
            {
                StaffNumber = "AS1004",
                Role = SupervisoryRole.DirectorOfStudies,
                StartDate = new DateTime(2026, 10, 1)
            },
            CancellationToken.None);

        var unprocessableResult = Assert.IsType<UnprocessableEntityObjectResult>(actionResult.Result);
        Assert.Equal(422, unprocessableResult.StatusCode);

        var response = Assert.IsType<AssignAcademicSupervisorResponse>(unprocessableResult.Value);
        Assert.Equal("SupervisorNotEligible", response.Outcome);
    }
}
