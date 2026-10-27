using PgrStudentManagement.Business.Students.ViewStudentEnrolments;
using PgrStudentManagement.Domain.Students;
using PgrStudentManagement.Tests.TestDoubles;

namespace PgrStudentManagement.Tests.Business;

public class ViewStudentEnrolmentsUseCaseTests
{
    private static readonly DateTime EarlierStartDate = new(2024, 9, 1);
    private static readonly DateTime LaterStartDate = new(2025, 9, 1);

    [Fact] // mark test methods with [Fact] because it identifies a test with a fixed set of inputs.
    public async Task MatchingEnrolmentsAreReturned()
    {
        // arrange - create a fake gateway with some enrolments, including one for the student we want to test
        var gateway = new FakeEnrolmentGateway(
        [
            CreateEnrolment("S001", "COMP", EarlierStartDate)
        ]);
        var useCase = new ViewStudentEnrolmentsUseCase(gateway);

        // act - call the use case with the student number we want to test
        var output = await useCase.ExecuteAsync(new ViewStudentEnrolmentsInput("S001"), CancellationToken.None);

        // assert - check that the output contains the expected enrolment
        var enrolment = Assert.Single(output.Enrolments);
        Assert.Equal("S001", output.StudentNumber);
        Assert.Equal("COMP", enrolment.ProgrammeCode);
        Assert.Equal(EarlierStartDate, enrolment.StartDate);
    }

    [Fact]
    public async Task UnrelatedEnrolmentsAreExcluded()
    {
        var gateway = new FakeEnrolmentGateway(
        [
            CreateEnrolment("S001", "COMP", EarlierStartDate),
            CreateEnrolment("S002", "MATH", EarlierStartDate)
        ]);
        var useCase = new ViewStudentEnrolmentsUseCase(gateway);

        var output = await useCase.ExecuteAsync(new ViewStudentEnrolmentsInput("S001"), CancellationToken.None);

        var enrolment = Assert.Single(output.Enrolments);
        Assert.Equal("COMP", enrolment.ProgrammeCode);
    }

    [Fact]
    public async Task ResultsAreOrderedFromEarliestToLatestStartDate()
    {
        var gateway = new FakeEnrolmentGateway(
        [
            CreateEnrolment("S001", "LATER", LaterStartDate),
            CreateEnrolment("S001", "EARLIER", EarlierStartDate)
        ]);
        var useCase = new ViewStudentEnrolmentsUseCase(gateway);

        var output = await useCase.ExecuteAsync(new ViewStudentEnrolmentsInput("S001"), CancellationToken.None);

        Assert.Equal(
            ["EARLIER", "LATER"],
            output.Enrolments.Select(enrolment => enrolment.ProgrammeCode));
    }

    [Fact]
    public async Task StudentWithNoEnrolmentsProducesAnEmptyCollection()
    {
        var gateway = new FakeEnrolmentGateway(
        [
            CreateEnrolment("S002", "MATH", EarlierStartDate)
        ]);
        var useCase = new ViewStudentEnrolmentsUseCase(gateway);

        var output = await useCase.ExecuteAsync(new ViewStudentEnrolmentsInput("S001"), CancellationToken.None);

        Assert.Empty(output.Enrolments);
    }

    [Fact]
    public async Task ViewingEnrolmentsDoesNotModifyFakeData()
    {
        var originalEnrolments = new[]
        {
            CreateEnrolment("S001", "LATER", LaterStartDate),
            CreateEnrolment("S001", "EARLIER", EarlierStartDate)
        };
        var gateway = new FakeEnrolmentGateway(originalEnrolments);
        var useCase = new ViewStudentEnrolmentsUseCase(gateway);

        await useCase.ExecuteAsync(new ViewStudentEnrolmentsInput("S001"), CancellationToken.None);

        Assert.Equal(originalEnrolments, gateway.Enrolments);
        Assert.Equal(
            ["LATER", "EARLIER"],
            gateway.Enrolments.Select(enrolment => enrolment.ProgrammeCode));
    }

    private static Enrolment CreateEnrolment(string studentNumber, string programmeCode, DateTime startDate)
    {
        return new Enrolment
        {
            StudentNumber = studentNumber,
            ProgrammeCode = programmeCode,
            StartDate = startDate,
            ExpectedEndDate = startDate.AddYears(3),
            ModeOfStudy = "Full-time",
            Status = EnrolmentStatus.Active
        };
    }
}
// Add test cases for:
// Matching Enrolments are returned.
// Unrelated Enrolments are excluded.
// Results are ordered from earliest to latest even when the fake returns an unsorted collection.
// A Student with no Enrolments produces an empty output collection.
// Viewing Enrolments does not modify the fake data.