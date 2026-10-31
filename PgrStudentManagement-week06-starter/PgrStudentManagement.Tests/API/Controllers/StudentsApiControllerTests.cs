using Microsoft.AspNetCore.Mvc;
using PgrStudentManagement.Adapters.API.Controllers;
using PgrStudentManagement.Adapters.API.Mappers;
using PgrStudentManagement.Adapters.API.Responses;
using PgrStudentManagement.Business.Students.ViewStudentEnrolments;

namespace PgrStudentManagement.Tests.API.Controllers;

public sealed class StudentsApiControllerTests
{
    [Fact]
    public async Task GetEnrolments_WhenNoneExist_ReturnsOkWithEmptyCollection()
    {
        var useCase = new StubViewStudentEnrolmentsUseCase();
        var controller = new StudentsApiController(useCase, new StudentEnrolmentsApiMapper());

        var actionResult = await controller.GetEnrolments(
            "S100003",
            CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<StudentEnrolmentsResponse>(okResult.Value);

        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal("S100003", response.StudentNumber);
        Assert.Empty(response.Enrolments);
    }

    private sealed class StubViewStudentEnrolmentsUseCase: IViewStudentEnrolmentsUseCase
    {
        public Task<StudentEnrolmentsOutput> ExecuteAsync(ViewStudentEnrolmentsInput input, CancellationToken cancellationToken)
        {
            return Task.FromResult(
                new StudentEnrolmentsOutput(input.StudentNumber, []));
        }
    }
}
