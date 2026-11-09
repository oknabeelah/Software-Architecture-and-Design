using PgrStudentManagement.Business.Students.GetStudentSummary;
using PgrStudentManagement.Domain.Students;
using PgrStudentManagement.Tests.TestDoubles;

namespace PgrStudentManagement.Tests.Business.Students;

public class GetStudentSummaryUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WhenStudentExists_ReturnsFoundWithDisplayName()
    {
        var gateway = new FakeStudentGateway();
        gateway.Add(new Student
        {
            StudentNumber = "S100002",
            FirstName = "Jane",
            LastName = "Smith",
            EmailAddress = "jane.smith@example.test"
        });

        var useCase = new GetStudentSummaryUseCase(gateway);

        var output = await useCase.ExecuteAsync(
            new GetStudentSummaryInput("S100002"),
            CancellationToken.None);

        Assert.True(output.Found);
        Assert.Equal("S100002", output.StudentNumber);
        Assert.Equal("Jane Smith", output.DisplayName);
        Assert.Equal(1, gateway.NumberOfCalls);
    }

    [Fact]
    public async Task ExecuteAsync_WhenStudentDoesNotExist_ReturnsNotFound()
    {
        var gateway = new FakeStudentGateway();
        var useCase = new GetStudentSummaryUseCase(gateway);

        var output = await useCase.ExecuteAsync(
            new GetStudentSummaryInput("UNKNOWN"),
            CancellationToken.None);

        Assert.False(output.Found);
        Assert.Equal("UNKNOWN", output.StudentNumber);
        Assert.Null(output.DisplayName);
        Assert.Equal(1, gateway.NumberOfCalls);
    }

    [Fact]
    public async Task ExecuteAsync_WhenStudentNumberIsBlank_ReturnsNotFoundWithoutCallingGateway()
    {
        var gateway = new FakeStudentGateway();
        var useCase = new GetStudentSummaryUseCase(gateway);

        var output = await useCase.ExecuteAsync(
            new GetStudentSummaryInput("   "),
            CancellationToken.None);

        Assert.False(output.Found);
        Assert.Null(output.DisplayName);
        Assert.Equal(0, gateway.NumberOfCalls);
    }
}

