using PgrStudentManagement.Business.AcademicSupervisors.AcademicSupervisorAssignment;
using PgrStudentManagement.Business.AcademicSupervisors.Gateway;
using PgrStudentManagement.Domain.AcademicSupervisors;
using PgrStudentManagement.Tests.TestDoubles;

namespace PgrStudentManagement.Tests.Business.AcademicSupervisors;

public class AcademicSupervisorAssignmentUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WhenValid_ReturnsCreated()
    {
        var gateway = CreateGateway();
        var result = await CreateUseCase(gateway, CreateLookup()).ExecuteAsync(
            CreateInput(),
            CancellationToken.None);

        Assert.Equal(AcademicSupervisorAssignmentOutcome.Created, result.Outcome);
        Assert.NotNull(gateway.AddedAssignment);
    }

    [Fact]
    public async Task ExecuteAsync_WhenStudentDoesNotExist_ReturnsStudentNotFound()
    {
        var result = await CreateUseCase(
            CreateGateway(),
            new FakeStudentLookup()).ExecuteAsync(
                CreateInput(),
                CancellationToken.None);

        Assert.Equal(
            AcademicSupervisorAssignmentOutcome.StudentNotFound,
            result.Outcome);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSupervisorDoesNotExist_ReturnsSupervisorNotFound()
    {
        var result = await CreateUseCase(
            new FakeAcademicSupervisorAssignmentGateway(),
            CreateLookup()).ExecuteAsync(
                CreateInput(),
                CancellationToken.None);

        Assert.Equal(
            AcademicSupervisorAssignmentOutcome.AcademicSupervisorNotFound,
            result.Outcome);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAssignmentAlreadyExists_ReturnsDuplicate()
    {
        var gateway = CreateGateway();
        gateway.AddAssignment(new SupervisoryAssignment
        {
            StudentNumber = "S100001",
            SupervisorStaffNumber = "SUP001",
            Role = SupervisoryRole.SecondSupervisor
        });

        var result = await CreateUseCase(gateway, CreateLookup()).ExecuteAsync(
            CreateInput(),
            CancellationToken.None);

        Assert.Equal(
            AcademicSupervisorAssignmentOutcome.DuplicateAssignment,
            result.Outcome);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUniqueRoleIsFilled_ReturnsRoleAlreadyFilled()
    {
        var gateway = CreateGateway();
        gateway.AddAssignment(new SupervisoryAssignment
        {
            StudentNumber = "S100001",
            SupervisorStaffNumber = "OTHER",
            Role = SupervisoryRole.DirectorOfStudies
        });

        var input = CreateInput() with { Role = SupervisoryRole.DirectorOfStudies };
        var result = await CreateUseCase(gateway, CreateLookup()).ExecuteAsync(
            input,
            CancellationToken.None);

        Assert.Equal(
            AcademicSupervisorAssignmentOutcome.RoleAlreadyFilled,
            result.Outcome);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRoleIsNotEligible_ReturnsNotEligible()
    {
        var gateway = CreateGateway(SupervisoryRole.Adviser);
        var result = await CreateUseCase(gateway, CreateLookup()).ExecuteAsync(
            CreateInput(),
            CancellationToken.None);

        Assert.Equal(
            AcademicSupervisorAssignmentOutcome.SupervisorNotEligible,
            result.Outcome);
    }

    [Fact]
    public async Task ExecuteAsync_WhenExternalSupervisorHasSeniorRole_ReturnsEmploymentTypeNotPermitted()
    {
        var gateway = CreateGateway(
            SupervisoryRole.FirstSupervisor,
            SupervisorEmploymentType.External);
        var input = CreateInput() with { Role = SupervisoryRole.FirstSupervisor };

        var result = await CreateUseCase(gateway, CreateLookup()).ExecuteAsync(
            input,
            CancellationToken.None);

        Assert.Equal(
            AcademicSupervisorAssignmentOutcome.EmploymentTypeNotPermitted,
            result.Outcome);
    }

    [Fact]
    public async Task ExecuteAsync_WhenFourthSupervisorNeedsApproval_ReturnsExceptionRequired()
    {
        var gateway = CreateGateway();
        gateway.AddAssignment(new SupervisoryAssignment
        {
            StudentNumber = "S100001",
            SupervisorStaffNumber = "SUP002",
            Role = SupervisoryRole.FirstSupervisor
        });
        gateway.AddAssignment(new SupervisoryAssignment
        {
            StudentNumber = "S100001",
            SupervisorStaffNumber = "SUP003",
            Role = SupervisoryRole.SecondSupervisor
        });
        gateway.AddAssignment(new SupervisoryAssignment
        {
            StudentNumber = "S100001",
            SupervisorStaffNumber = "SUP004",
            Role = SupervisoryRole.SecondSupervisor
        });

        var result = await CreateUseCase(gateway, CreateLookup()).ExecuteAsync(
            CreateInput(),
            CancellationToken.None);

        Assert.Equal(
            AcademicSupervisorAssignmentOutcome.MaximumSupervisorsReached,
            result.Outcome);
    }

    [Fact]
    public async Task ExecuteAsync_WhenInputIsInvalid_ReturnsInvalidInput()
    {
        var result = await CreateUseCase(
            CreateGateway(),
            CreateLookup()).ExecuteAsync(
                CreateInput() with { StudentNumber = " " },
                CancellationToken.None);

        Assert.Equal(
            AcademicSupervisorAssignmentOutcome.InvalidInput,
            result.Outcome);
    }

    private static AcademicSupervisorAssignmentUseCase CreateUseCase(
        FakeAcademicSupervisorAssignmentGateway gateway,
        FakeStudentLookup lookup) =>
        new(gateway, lookup);

    private static FakeStudentLookup CreateLookup()
    {
        var lookup = new FakeStudentLookup();
        lookup.Add(new StudentSummary("S100001", "Alex Student"));
        return lookup;
    }

    private static FakeAcademicSupervisorAssignmentGateway CreateGateway(
        SupervisoryRole role = SupervisoryRole.SecondSupervisor,
        SupervisorEmploymentType employmentType =
            SupervisorEmploymentType.InstitutionEmployed)
    {
        var gateway = new FakeAcademicSupervisorAssignmentGateway();
        gateway.AddSupervisor(new AcademicSupervisor
        {
            StaffNumber = "SUP001",
            EmploymentType = employmentType,
            EligibleRoleEntries = [role]
        });
        return gateway;
    }

    private static AcademicSupervisorAssignmentInput CreateInput() =>
        new(
            "S100001",
            "SUP001",
            SupervisoryRole.SecondSupervisor,
            new DateTime(2026, 11, 9),
            false);
}
