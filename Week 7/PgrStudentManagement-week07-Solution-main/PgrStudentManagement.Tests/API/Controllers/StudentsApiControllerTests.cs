using Microsoft.AspNetCore.Mvc;
using PgrStudentManagement.Adapters.API.Controllers;
using PgrStudentManagement.Adapters.API.Mappers;
using PgrStudentManagement.Adapters.API.Responses;
using PgrStudentManagement.Business.Students.GetStudentSummary;
using PgrStudentManagement.Domain.Students;
using PgrStudentManagement.Tests.TestDoubles;

namespace PgrStudentManagement.Tests.API.Controllers;

public class StudentsApiControllerTests
{
    [Fact]
    public async Task GetEnrolments_WhenNoneExist_ReturnsOkWithEmptyCollection()
    {
        var useCase = new StubViewStudentEnrolmentsUseCase();
        var mapper = new StudentEnrolmentsApiMapper();
        var controller = new StudentsApiController(useCase, mapper);

        var actionResult = await controller.GetEnrolments(
            "S100003",
            CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<StudentEnrolmentsResponse>(okResult.Value);

        Assert.Equal(200, okResult.StatusCode);
        Assert.Empty(response.Enrolments);
    }

    [Fact]
    public async Task GetStudentLookup_WhenStudentExists_ReturnsOkWithLookupResponse()
    {
        var studentGateway = new FakeStudentGateway();
        studentGateway.Add(new Student
        {
            StudentNumber = "S100002",
            FirstName = "Jane",
            LastName = "Smith",
            EmailAddress = "jane.smith@example.test"
        });

        var controller = new StudentsApiController(
            new StubViewStudentEnrolmentsUseCase(),
            new StudentEnrolmentsApiMapper(),
            new GetStudentSummaryUseCase(studentGateway),
            new StudentLookupApiMapper());

        var actionResult = await controller.GetStudentLookup(
            "S100002",
            CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<StudentLookupResponse>(okResult.Value);

        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal("S100002", response.StudentNumber);
        Assert.Equal("Jane Smith", response.DisplayName);
    }

    [Fact]
    public async Task GetStudentLookup_WhenStudentDoesNotExist_ReturnsNotFound()
    {
        var studentGateway = new FakeStudentGateway();

        var controller = new StudentsApiController(
            new StubViewStudentEnrolmentsUseCase(),
            new StudentEnrolmentsApiMapper(),
            new GetStudentSummaryUseCase(studentGateway),
            new StudentLookupApiMapper());

        var actionResult = await controller.GetStudentLookup(
            "UNKNOWN",
            CancellationToken.None);

        Assert.IsType<NotFoundResult>(actionResult.Result);
    }
}
