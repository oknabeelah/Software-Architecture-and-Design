using PgrStudentManagement.Business.AcademicSupervisors.AssignAcademicSupervisor;
using PgrStudentManagement.Business.AcademicSupervisors.Gateway;
using PgrStudentManagement.Business.Students.GetStudentSummary;
using PgrStudentManagement.Domain.AcademicSupervisors;
using PgrStudentManagement.Domain.Students;
using PgrStudentManagement.Tests.TestDoubles;

namespace PgrStudentManagement.Tests.Business.AcademicSupervisors;

public class AssignAcademicSupervisorUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WhenStudentDoesNotExist_ReturnsStudentNotFound()
    {
        var gateway = new FakeAcademicSupervisorGateway();
        var studentLookup = new FakeStudentLookup();
        var useCase = new AssignAcademicSupervisorUseCase(gateway, studentLookup);

        var result = await useCase.ExecuteAsync(
            CreateInput("UNKNOWN", "AS1001"),
            CancellationToken.None);

        Assert.Equal(AssignmentOutcome.StudentNotFound, result.Outcome);
        Assert.False(result.Succeeded);
        Assert.Null(result.Assignment);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSupervisorDoesNotExist_ReturnsSupervisorNotFound()
    {
        var gateway = new FakeAcademicSupervisorGateway();
        var studentLookup = new FakeStudentLookup();
        studentLookup.AddStudent("S100001", "John Doe");

        var useCase = new AssignAcademicSupervisorUseCase(gateway, studentLookup);

        var result = await useCase.ExecuteAsync(
            CreateInput("S100001", "UNKNOWN"),
            CancellationToken.None);

        Assert.Equal(AssignmentOutcome.SupervisorNotFound, result.Outcome);
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task ExecuteAsync_WhenDuplicateCurrentAssignmentExists_ReturnsDuplicateAssignment()
    {
        var gateway = new FakeAcademicSupervisorGateway();
        var studentLookup = new FakeStudentLookup();
        studentLookup.AddStudent("S100001", "John Doe");

        await gateway.AddSupervisorAsync(CreateSupervisor(
            "AS1004",
            eligibleRoles: [SupervisoryRole.Adviser]));

        await gateway.AddAssignmentAsync(new SupervisoryAssignment
        {
            StudentNumber = "S100001",
            SupervisorStaffNumber = "AS1004",
            Role = SupervisoryRole.Adviser,
            StartDate = new DateTime(2025, 10, 1)
        });

        var useCase = new AssignAcademicSupervisorUseCase(gateway, studentLookup);

        var result = await useCase.ExecuteAsync(
            CreateInput("S100001", "AS1004", SupervisoryRole.Adviser),
            CancellationToken.None);

        Assert.Equal(AssignmentOutcome.DuplicateAssignment, result.Outcome);
    }

    [Theory]
    [InlineData(SupervisoryRole.DirectorOfStudies)]
    [InlineData(SupervisoryRole.FirstSupervisor)]
    public async Task ExecuteAsync_WhenUniqueRoleAlreadyFilled_ReturnsRoleAlreadyFilled(
        SupervisoryRole role)
    {
        var gateway = new FakeAcademicSupervisorGateway();
        var studentLookup = new FakeStudentLookup();
        studentLookup.AddStudent("S100001", "John Doe");

        await gateway.AddSupervisorAsync(CreateSupervisor("AS1001", eligibleRoles: [role]));
        await gateway.AddSupervisorAsync(CreateSupervisor("AS1002", eligibleRoles: [role]));

        await gateway.AddAssignmentAsync(new SupervisoryAssignment
        {
            StudentNumber = "S100001",
            SupervisorStaffNumber = "AS1001",
            Role = role,
            StartDate = new DateTime(2025, 10, 1)
        });

        var useCase = new AssignAcademicSupervisorUseCase(gateway, studentLookup);

        var result = await useCase.ExecuteAsync(
            CreateInput("S100001", "AS1002", role),
            CancellationToken.None);

        Assert.Equal(AssignmentOutcome.RoleAlreadyFilled, result.Outcome);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSupervisorNotEligible_ReturnsSupervisorNotEligible()
    {
        var gateway = new FakeAcademicSupervisorGateway();
        var studentLookup = new FakeStudentLookup();
        studentLookup.AddStudent("S100001", "John Doe");

        await gateway.AddSupervisorAsync(CreateSupervisor(
            "AS1004",
            eligibleRoles: [SupervisoryRole.Adviser]));

        var useCase = new AssignAcademicSupervisorUseCase(gateway, studentLookup);

        var result = await useCase.ExecuteAsync(
            CreateInput("S100001", "AS1004", SupervisoryRole.DirectorOfStudies),
            CancellationToken.None);

        Assert.Equal(AssignmentOutcome.SupervisorNotEligible, result.Outcome);
    }

    [Theory]
    [InlineData(SupervisoryRole.DirectorOfStudies)]
    [InlineData(SupervisoryRole.FirstSupervisor)]
    public async Task ExecuteAsync_WhenExternalSupervisorAssignedToRestrictedRole_ReturnsEmploymentTypeNotPermitted(
        SupervisoryRole role)
    {
        var gateway = new FakeAcademicSupervisorGateway();
        var studentLookup = new FakeStudentLookup();
        studentLookup.AddStudent("S100001", "John Doe");

        await gateway.AddSupervisorAsync(CreateSupervisor(
            "EXT001",
            employmentType: SupervisorEmploymentType.External,
            eligibleRoles: [role]));

        var useCase = new AssignAcademicSupervisorUseCase(gateway, studentLookup);

        var result = await useCase.ExecuteAsync(
            CreateInput("S100001", "EXT001", role),
            CancellationToken.None);

        Assert.Equal(AssignmentOutcome.EmploymentTypeNotPermitted, result.Outcome);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAddingFourthSupervisorWithoutException_ReturnsExceptionRequired()
    {
        var gateway = new FakeAcademicSupervisorGateway();
        var studentLookup = new FakeStudentLookup();
        studentLookup.AddStudent("S100001", "John Doe");

        await AddSupervisorAndAssignmentAsync(gateway, "AS1001", SupervisoryRole.DirectorOfStudies, new DateTime(2025, 1, 1));
        await AddSupervisorAndAssignmentAsync(gateway, "AS1002", SupervisoryRole.FirstSupervisor, new DateTime(2025, 2, 1));
        await AddSupervisorAndAssignmentAsync(gateway, "AS1003", SupervisoryRole.SecondSupervisor, new DateTime(2025, 3, 1));

        await gateway.AddSupervisorAsync(CreateSupervisor(
            "AS1004",
            eligibleRoles: [SupervisoryRole.SecondSupervisor]));

        var useCase = new AssignAcademicSupervisorUseCase(gateway, studentLookup);

        var result = await useCase.ExecuteAsync(
            CreateInput("S100001", "AS1004", SupervisoryRole.SecondSupervisor, exceptionAuthorised: false),
            CancellationToken.None);

        Assert.Equal(AssignmentOutcome.ExceptionRequired, result.Outcome);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAddingFourthSupervisorWithException_CreatesAssignment()
    {
        var gateway = new FakeAcademicSupervisorGateway();
        var studentLookup = new FakeStudentLookup();
        studentLookup.AddStudent("S100001", "John Doe");

        await AddSupervisorAndAssignmentAsync(gateway, "AS1001", SupervisoryRole.DirectorOfStudies, new DateTime(2025, 1, 1));
        await AddSupervisorAndAssignmentAsync(gateway, "AS1002", SupervisoryRole.FirstSupervisor, new DateTime(2025, 2, 1));
        await AddSupervisorAndAssignmentAsync(gateway, "AS1003", SupervisoryRole.SecondSupervisor, new DateTime(2025, 3, 1));

        await gateway.AddSupervisorAsync(CreateSupervisor(
            "AS1004",
            eligibleRoles: [SupervisoryRole.SecondSupervisor]));

        var useCase = new AssignAcademicSupervisorUseCase(gateway, studentLookup);
        var startDate = new DateTime(2026, 10, 1);

        var result = await useCase.ExecuteAsync(
            CreateInput("S100001", "AS1004", SupervisoryRole.SecondSupervisor, startDate, exceptionAuthorised: true),
            CancellationToken.None);

        Assert.Equal(AssignmentOutcome.Created, result.Outcome);
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Assignment);
        Assert.True(await gateway.AssignmentExistsAsync(
            "S100001",
            "AS1004",
            SupervisoryRole.SecondSupervisor,
            startDate));
    }

    [Fact]
    public async Task ExecuteAsync_WithLocalStudentLookupAdapter_ResolvesStudentInProcessAndCreatesAssignment()
    {
        var studentGateway = new FakeStudentGateway();
        studentGateway.Add(new Student
        {
            StudentNumber = "S100001",
            FirstName = "John",
            LastName = "Doe",
            EmailAddress = "john.doe@example.test"
        });

        var getStudentSummaryUseCase = new GetStudentSummaryUseCase(studentGateway);
        var localStudentLookup = new LocalStudentLookupAdapter(getStudentSummaryUseCase);

        var supervisorGateway = new FakeAcademicSupervisorGateway();
        await supervisorGateway.AddSupervisorAsync(CreateSupervisor(
            "AS1004",
            eligibleRoles: [SupervisoryRole.SecondSupervisor]));

        var useCase = new AssignAcademicSupervisorUseCase(supervisorGateway, localStudentLookup);
        var startDate = new DateTime(2026, 10, 1);

        var result = await useCase.ExecuteAsync(
            CreateInput("S100001", "AS1004", SupervisoryRole.SecondSupervisor, startDate),
            CancellationToken.None);

        Assert.Equal(AssignmentOutcome.Created, result.Outcome);
        Assert.Equal(1, studentGateway.NumberOfCalls);
    }

    private static AssignAcademicSupervisorInput CreateInput(
        string studentNumber,
        string staffNumber,
        SupervisoryRole role = SupervisoryRole.Adviser,
        DateTime? startDate = null,
        bool exceptionAuthorised = false)
    {
        return new AssignAcademicSupervisorInput(
            StudentNumber: studentNumber,
            StaffNumber: staffNumber,
            Role: role,
            StartDate: startDate ?? new DateTime(2026, 10, 1),
            ExceptionAuthorised: exceptionAuthorised);
    }

    private static AcademicSupervisor CreateSupervisor(
        string staffNumber,
        SupervisorEmploymentType employmentType = SupervisorEmploymentType.InstitutionEmployed,
        SupervisoryRole[]? eligibleRoles = null)
    {
        eligibleRoles ??=
        [
            SupervisoryRole.DirectorOfStudies,
            SupervisoryRole.FirstSupervisor,
            SupervisoryRole.SecondSupervisor,
            SupervisoryRole.Adviser
        ];

        return new AcademicSupervisor
        {
            StaffNumber = staffNumber,
            FirstName = "Robin",
            LastName = "Carter",
            EmailAddress = $"{staffNumber.ToLowerInvariant()}@example.test",
            EmploymentType = employmentType,
            TrainingCompleted = true,
            EligibleRoles = eligibleRoles.ToHashSet()
        };
    }

    private static async Task AddSupervisorAndAssignmentAsync(
        FakeAcademicSupervisorGateway gateway,
        string staffNumber,
        SupervisoryRole role,
        DateTime startDate)
    {
        await gateway.AddSupervisorAsync(CreateSupervisor(
            staffNumber,
            eligibleRoles: [role]));

        await gateway.AddAssignmentAsync(new SupervisoryAssignment
        {
            StudentNumber = "S100001",
            SupervisorStaffNumber = staffNumber,
            Role = role,
            StartDate = startDate
        });
    }
}

