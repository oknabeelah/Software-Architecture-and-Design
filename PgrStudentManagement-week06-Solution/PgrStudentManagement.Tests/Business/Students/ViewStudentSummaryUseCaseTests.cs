using PgrStudentManagement.Business.Students.ViewStudentSummary;
using PgrStudentManagement.Domain.Students;
using PgrStudentManagement.Tests.TestDoubles;

namespace PgrStudentManagement.Tests.Business.Students;

public sealed class ViewStudentSummaryUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WhenStudentExists_ReturnsRequiredSummary()
    {
        var gateway = new FakeStudentGateway();
        gateway.Add(new Student
        {
            StudentNumber = "S100002",
            FirstName = "Jane",
            LastName = "Smith",
            EmailAddress = "jane.smith@example.test"
        });

        var output = await new ViewStudentSummaryUseCase(gateway)
            .ExecuteAsync(
                new ViewStudentSummaryInput(" S100002 "),
                CancellationToken.None);

        Assert.NotNull(output);
        Assert.Equal("S100002", output.StudentNumber);
        Assert.Equal("Jane Smith", output.DisplayName);
        Assert.Equal("S100002", gateway.LastRequestedStudentNumber);
        Assert.Equal(1, gateway.NumberOfCalls);
    }

    [Fact]
    public async Task ExecuteAsync_WhenStudentDoesNotExist_ReturnsMissingOutput()
    {
        var gateway = new FakeStudentGateway();

        var output = await new ViewStudentSummaryUseCase(gateway)
            .ExecuteAsync(
                new ViewStudentSummaryInput("S999999"),
                CancellationToken.None);

        Assert.Null(output);
        Assert.Equal("S999999", gateway.LastRequestedStudentNumber);
    }

    [Fact]
    public async Task ExecuteAsync_WhenStudentNumberIsBlank_ReturnsMissingOutputWithoutGatewayCall()
    {
        var gateway = new FakeStudentGateway();

        var output = await new ViewStudentSummaryUseCase(gateway)
            .ExecuteAsync(
                new ViewStudentSummaryInput(" "),
                CancellationToken.None);

        Assert.Null(output);
        Assert.Equal(0, gateway.NumberOfCalls);
    }
}
