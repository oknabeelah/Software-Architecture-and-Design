using PgrStudentManagement.Business.AcademicSupervisors.ViewSupervisoryTeam;
using PgrStudentManagement.Domain.AcademicSupervisors;
using PgrStudentManagement.Tests.TestDoubles;

namespace PgrStudentManagement.Tests.Business.AcademicSupervisors;

public class ViewSupervisoryTeamUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WhenStudentDoesNotExist_ReturnsStudentFoundFalse()
    {
        var gateway = new FakeAcademicSupervisorGateway();
        var studentLookup = new FakeStudentLookup();
        var useCase = new ViewSupervisoryTeamUseCase(gateway, studentLookup);

        var output = await useCase.ExecuteAsync(
            new ViewSupervisoryTeamInput("UNKNOWN"),
            CancellationToken.None);

        Assert.False(output.StudentFound);
        Assert.Null(output.Student);
        Assert.Empty(output.CurrentAssignments);
        Assert.Empty(output.HistoricalAssignments);
    }

    [Fact]
    public async Task ExecuteAsync_SeparatesCurrentAndHistoricalAssignments()
    {
        var gateway = new FakeAcademicSupervisorGateway();
        var studentLookup = new FakeStudentLookup();
        studentLookup.AddStudent("S100001", "John Doe");

        await gateway.AddSupervisorAsync(new AcademicSupervisor
        {
            StaffNumber = "AS1001",
            FirstName = "Robin",
            LastName = "Carter",
            EmailAddress = "robin.carter@example.test",
            EmploymentType = SupervisorEmploymentType.InstitutionEmployed,
            TrainingCompleted = true,
            EligibleRoles = new HashSet<SupervisoryRole> { SupervisoryRole.DirectorOfStudies }
        });

        await gateway.AddSupervisorAsync(new AcademicSupervisor
        {
            StaffNumber = "AS1003",
            FirstName = "Taylor",
            LastName = "Green",
            EmailAddress = "taylor.green@example.test",
            EmploymentType = SupervisorEmploymentType.External,
            TrainingCompleted = true,
            EligibleRoles = new HashSet<SupervisoryRole> { SupervisoryRole.SecondSupervisor }
        });

        await gateway.AddAssignmentAsync(new SupervisoryAssignment
        {
            StudentNumber = "S100001",
            SupervisorStaffNumber = "AS1001",
            Role = SupervisoryRole.DirectorOfStudies,
            StartDate = new DateTime(2025, 10, 1),
            EndDate = null
        });

        await gateway.AddAssignmentAsync(new SupervisoryAssignment
        {
            StudentNumber = "S100001",
            SupervisorStaffNumber = "AS1003",
            Role = SupervisoryRole.SecondSupervisor,
            StartDate = new DateTime(2024, 10, 1),
            EndDate = new DateTime(2025, 9, 30)
        });

        var useCase = new ViewSupervisoryTeamUseCase(gateway, studentLookup);

        var output = await useCase.ExecuteAsync(
            new ViewSupervisoryTeamInput("S100001"),
            CancellationToken.None);

        Assert.True(output.StudentFound);
        Assert.NotNull(output.Student);
        Assert.Equal("John Doe", output.Student.DisplayName);
        Assert.Single(output.CurrentAssignments);
        Assert.Single(output.HistoricalAssignments);
        Assert.Equal("AS1001", output.CurrentAssignments[0].SupervisorStaffNumber);
        Assert.Equal("AS1003", output.HistoricalAssignments[0].SupervisorStaffNumber);
    }
}

