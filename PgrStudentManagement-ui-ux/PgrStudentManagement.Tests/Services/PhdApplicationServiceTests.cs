using Microsoft.EntityFrameworkCore;
using PgrStudentManagement.Tests.Fixtures;
using PgrStudentManagement.Tests.TestDoubles;
using PgrStudentManagement.Web.Modules.PhdApplications.Data.Daos;
using PgrStudentManagement.Web.Modules.PhdApplications.Models;
using PgrStudentManagement.Web.Modules.PhdApplications.Services;
using Xunit;

namespace PgrStudentManagement.Tests.Services;

public sealed class PhdApplicationServiceTests
{
    [Fact]
    public async Task CreateApplicationAsync_WithValidApplication_ReturnsCreated()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();

        var dao = new PhdApplicationDao(database.Context);
        var supervisorLookup = new FakeSupervisorLookup();
        var studentLookup = new FakeStudentLookup();

        var service = new PhdApplicationService(
            dao,
            supervisorLookup,
            studentLookup);

        var application = CreateApplication("APP-2026-100");

        var outcome = await service.CreateApplicationAsync(application);

        Assert.Equal(ApplicationOutcome.Created, outcome);

        var saved = await dao.GetApplicationByReferenceAsync("APP-2026-100");
        Assert.NotNull(saved);
    }

    [Fact]
    public async Task CreateApplicationAsync_WithDuplicateReference_ReturnsDuplicateReference()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();

        var dao = new PhdApplicationDao(database.Context);
        var supervisorLookup = new FakeSupervisorLookup();
        var studentLookup = new FakeStudentLookup();

        var service = new PhdApplicationService(
            dao,
            supervisorLookup,
            studentLookup);

        await service.CreateApplicationAsync(CreateApplication("APP-2026-100"));

        var outcome = await service.CreateApplicationAsync(
            CreateApplication("APP-2026-100"));

        Assert.Equal(ApplicationOutcome.DuplicateReference, outcome);
        Assert.Single(await database.Context.PhdApplications.ToListAsync());
    }

    [Fact]
    public async Task CreateApplicationAsync_WhenSupervisorDoesNotExist_ReturnsSupervisorNotFound()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();

        var dao = new PhdApplicationDao(database.Context);
        var supervisorLookup = new FakeSupervisorLookup();
        var studentLookup = new FakeStudentLookup();

        var service = new PhdApplicationService(
            dao,
            supervisorLookup,
            studentLookup);

        var application = CreateApplication(
            "APP-2026-101",
            proposedSupervisorStaffNumber: "UNKNOWN");

        var outcome = await service.CreateApplicationAsync(application);

        Assert.Equal(ApplicationOutcome.SupervisorNotFound, outcome);
    }

    [Fact]
    public async Task CreateApplicationAsync_WhenSupervisorNotEligibleMain_ReturnsSupervisorNotEligible()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();

        var dao = new PhdApplicationDao(database.Context);
        var supervisorLookup = new FakeSupervisorLookup();
        supervisorLookup.AddSupervisor(
            "AS1003",
            "Taylor Green",
            isEligibleMainSupervisor: false);

        var studentLookup = new FakeStudentLookup();

        var service = new PhdApplicationService(
            dao,
            supervisorLookup,
            studentLookup);

        var application = CreateApplication(
            "APP-2026-102",
            proposedSupervisorStaffNumber: "AS1003");

        var outcome = await service.CreateApplicationAsync(application);

        Assert.Equal(ApplicationOutcome.SupervisorNotEligible, outcome);
    }

    [Fact]
    public async Task UpdateApplicationStatusAsync_ToOfferMade_SetsDecisionDate()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();

        var dao = new PhdApplicationDao(database.Context);
        var supervisorLookup = new FakeSupervisorLookup();
        var studentLookup = new FakeStudentLookup();

        var service = new PhdApplicationService(
            dao,
            supervisorLookup,
            studentLookup);

        await service.CreateApplicationAsync(CreateApplication("APP-2026-103"));

        var decisionDate = new DateTime(2026, 11, 4);

        var outcome = await service.UpdateApplicationStatusAsync(
            "APP-2026-103",
            ApplicationStatus.OfferMade,
            decisionDate);

        Assert.Equal(ApplicationOutcome.Updated, outcome);

        var updated = await dao.GetApplicationByReferenceAsync("APP-2026-103");
        Assert.NotNull(updated);
        Assert.Equal(ApplicationStatus.OfferMade, updated.Status);
        Assert.Equal(decisionDate, updated.DecisionDate);
    }

    [Fact]
    public async Task UpdateApplicationStatusAsync_FromRejectedToOfferMade_ReturnsInvalidStatusTransition()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();

        var dao = new PhdApplicationDao(database.Context);
        var supervisorLookup = new FakeSupervisorLookup();
        var studentLookup = new FakeStudentLookup();

        var service = new PhdApplicationService(
            dao,
            supervisorLookup,
            studentLookup);

        await service.CreateApplicationAsync(
            CreateApplication(
                "APP-2026-104",
                status: ApplicationStatus.Rejected));

        var outcome = await service.UpdateApplicationStatusAsync(
            "APP-2026-104",
            ApplicationStatus.OfferMade);

        Assert.Equal(ApplicationOutcome.InvalidStatusTransition, outcome);
    }

    private static PhdApplication CreateApplication(
        string reference,
        string? proposedSupervisorStaffNumber = null,
        ApplicationStatus status = ApplicationStatus.Submitted)
    {
        return new PhdApplication
        {
            ApplicationReference = reference,
            FirstName = "Elena",
            LastName = "Rostova",
            EmailAddress = $"{reference.ToLowerInvariant()}@example.test",
            ProgrammeCode = "PHD-COMP",
            ModeOfStudy = "Full-time",
            ProposedStartDate = new DateTime(2027, 10, 1),
            ProposedThesisTitle = "Federated Learning Privacy",
            ResearchSummary = "Privacy guarantees in edge networks.",
            FundingSource = FundingType.Studentship,
            ProposedSupervisorStaffNumber = proposedSupervisorStaffNumber,
            Status = status,
            SubmittedDate = new DateTime(2026, 9, 15)
        };
    }
}

