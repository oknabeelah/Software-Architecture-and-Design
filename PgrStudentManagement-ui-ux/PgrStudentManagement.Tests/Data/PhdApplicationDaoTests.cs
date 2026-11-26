using Microsoft.EntityFrameworkCore;
using PgrStudentManagement.Tests.Fixtures;
using PgrStudentManagement.Web.Modules.PhdApplications.Data.Daos;
using PgrStudentManagement.Web.Modules.PhdApplications.Models;
using Xunit;

namespace PgrStudentManagement.Tests.Data;

public sealed class PhdApplicationDaoTests
{
    [Fact]
    public async Task AddApplicationAsync_SavesApplication()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();

        var dao = new PhdApplicationDao(database.Context);

        var application = CreateApplication("APP-2026-001");

        await dao.AddApplicationAsync(application);

        database.Context.ChangeTracker.Clear();

        var saved = await database.Context
            .PhdApplications
            .SingleAsync(existing =>
                existing.ApplicationReference == "APP-2026-001");

        Assert.Equal("APP-2026-001", saved.ApplicationReference);
        Assert.Equal("Elena", saved.FirstName);
        Assert.Equal("Rostova", saved.LastName);
        Assert.Equal(FundingType.Studentship, saved.FundingSource);
        Assert.Equal(ApplicationStatus.Submitted, saved.Status);
    }

    [Fact]
    public async Task GetApplicationsAsync_WithStatusFilter_ReturnsMatchingApplications()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();

        var dao = new PhdApplicationDao(database.Context);

        await dao.AddApplicationAsync(
            CreateApplication(
                "APP-2026-001",
                status: ApplicationStatus.Submitted));

        await dao.AddApplicationAsync(
            CreateApplication(
                "APP-2026-002",
                status: ApplicationStatus.OfferMade));

        database.Context.ChangeTracker.Clear();

        var offers = await dao.GetApplicationsAsync(ApplicationStatus.OfferMade);

        var single = Assert.Single(offers);
        Assert.Equal("APP-2026-002", single.ApplicationReference);
    }

    [Fact]
    public async Task UpdateApplicationAsync_UpdatesExistingApplication()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();

        var dao = new PhdApplicationDao(database.Context);

        var application = CreateApplication("APP-2026-001");
        await dao.AddApplicationAsync(application);

        database.Context.ChangeTracker.Clear();

        application.Status = ApplicationStatus.OfferMade;
        application.DecisionDate = new DateTime(2026, 11, 1);

        var updated = await dao.UpdateApplicationAsync(application);

        Assert.True(updated);

        database.Context.ChangeTracker.Clear();

        var saved = await dao.GetApplicationByReferenceAsync("APP-2026-001");

        Assert.NotNull(saved);
        Assert.Equal(ApplicationStatus.OfferMade, saved.Status);
        Assert.Equal(new DateTime(2026, 11, 1), saved.DecisionDate);
    }

    private static PhdApplication CreateApplication(
        string reference,
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
            Status = status,
            SubmittedDate = new DateTime(2026, 9, 15)
        };
    }
}

