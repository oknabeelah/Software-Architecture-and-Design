using Microsoft.AspNetCore.Mvc;
using PgrStudentManagement.Adapters.API.Controllers;
using PgrStudentManagement.Adapters.API.Mappers;
using PgrStudentManagement.Adapters.API.Responses;
using PgrStudentManagement.Tests.TestDoubles;

namespace PgrStudentManagement.Tests.API.Controllers;

public class StudentsApiControllerTests
{
    [Fact]
    public async Task GetEnrolments_WhenNoneExist_ReturnsOkWithEmptyCollection()
    {
        var useCase = new StubViewStudentEnrolmentsUseCase();

        var mapper = new StudentEnrolmentsApiMapper();

        var controller = new StudentsApiController(
            useCase,
            mapper);

        var actionResult = await controller.GetEnrolments(
            "S100003",
            CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);

        var response = Assert.IsType<StudentEnrolmentsResponse>(okResult.Value);

        Assert.Equal(200, okResult.StatusCode);
        Assert.Empty(response.Enrolments);
    }
}
