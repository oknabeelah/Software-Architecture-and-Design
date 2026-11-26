using PgrStudentManagement.Web.Modules.AcademicSupervisors.Contracts;
using PgrStudentManagement.Web.Modules.PhdApplications.Data.Daos;
using PgrStudentManagement.Web.Modules.PhdApplications.Models;
using PgrStudentManagement.Web.Modules.Students.Contracts;

namespace PgrStudentManagement.Web.Modules.PhdApplications.Data;

public sealed class PhdApplicationDataSeeder
{
    private readonly PhdApplicationDao _phdApplicationDao;
    private readonly ISupervisorLookup _supervisorLookup;
    private readonly IStudentLookup _studentLookup;

    public PhdApplicationDataSeeder(
        PhdApplicationDao phdApplicationDao,
        ISupervisorLookup supervisorLookup,
        IStudentLookup studentLookup)
    {
        _phdApplicationDao = phdApplicationDao;
        _supervisorLookup = supervisorLookup;
        _studentLookup = studentLookup;
    }

    public async Task SeedAsync(
        CancellationToken cancellationToken = default)
    {
        await SeedApplicationIfMissingAsync(
            new PhdApplication
            {
                ApplicationReference = "APP-2026-001",
                FirstName = "Elena",
                LastName = "Rostova",
                EmailAddress = "elena.rostova@example.test",
                ProgrammeCode = "PHD-COMP",
                ModeOfStudy = "Full-time",
                ProposedStartDate = new DateTime(2027, 10, 1),
                ProposedThesisTitle =
                    "Federated Learning Privacy Guarantees in Edge Networks",
                ResearchSummary =
                    "Investigating differential privacy bounds for decentralized edge model training.",
                FundingSource = FundingType.Studentship,
                ProposedSupervisorStaffNumber = "AS1001",
                Status = ApplicationStatus.UnderReview,
                SubmittedDate = new DateTime(2026, 9, 15)
            },
            cancellationToken);

        await SeedApplicationIfMissingAsync(
            new PhdApplication
            {
                ApplicationReference = "APP-2026-002",
                FirstName = "Marcus",
                LastName = "Vance",
                EmailAddress = "marcus.vance@example.test",
                ProgrammeCode = "PHD-DESIGN",
                ModeOfStudy = "Part-time",
                ProposedStartDate = new DateTime(2027, 10, 1),
                ProposedThesisTitle =
                    "Accessible Tactile Interfaces for Neurodiverse Learners",
                ResearchSummary =
                    "Co-designing multi-sensory learning tools with inclusive participatory methods.",
                FundingSource = FundingType.ResearchCouncil,
                ProposedSupervisorStaffNumber = "AS1002",
                Status = ApplicationStatus.OfferMade,
                SubmittedDate = new DateTime(2026, 8, 20),
                DecisionDate = new DateTime(2026, 10, 10)
            },
            cancellationToken);

        await SeedApplicationIfMissingAsync(
            new PhdApplication
            {
                ApplicationReference = "APP-2026-003",
                FirstName = "Priya",
                LastName = "Nair",
                EmailAddress = "priya.nair@example.test",
                ProgrammeCode = "PHD-COMP",
                ModeOfStudy = "Full-time",
                ProposedStartDate = new DateTime(2027, 1, 15),
                ProposedThesisTitle =
                    "Automated Formal Verification of Smart Contracts",
                ResearchSummary =
                    "Applying theorem proving and symbolic execution to distributed ledger protocols.",
                FundingSource = FundingType.SelfFunded,
                ProposedSupervisorStaffNumber = null,
                Status = ApplicationStatus.Submitted,
                SubmittedDate = new DateTime(2026, 10, 25)
            },
            cancellationToken);

        await SeedApplicationIfMissingAsync(
            new PhdApplication
            {
                ApplicationReference = "APP-2026-004",
                FirstName = "Casey",
                LastName = "Wilson",
                EmailAddress = "casey.wilson@example.test",
                ProgrammeCode = "PHD-DESIGN",
                ModeOfStudy = "Full-time",
                ProposedStartDate = new DateTime(2026, 10, 1),
                ProposedThesisTitle =
                    "Sustainable Material Workflows in Circular Industrial Design",
                ResearchSummary =
                    "Lifecycle assessment and digital twin modelling for circular product design.",
                FundingSource = FundingType.ExternalSponsor,
                ProposedSupervisorStaffNumber = "AS1001",
                Status = ApplicationStatus.Enrolled,
                SubmittedDate = new DateTime(2026, 3, 10),
                DecisionDate = new DateTime(2026, 5, 18),
                EnrolledStudentNumber = "S100003"
            },
            cancellationToken);
    }

    private async Task SeedApplicationIfMissingAsync(
        PhdApplication application,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(
                application.ProposedSupervisorStaffNumber))
        {
            var supervisor =
                await _supervisorLookup.GetSupervisorSummaryAsync(
                    application.ProposedSupervisorStaffNumber,
                    cancellationToken);

            if (supervisor is null)
            {
                throw new InvalidOperationException(
                    $"Cannot seed PhD application {application.ApplicationReference}: "
                    + $"Academic Supervisor {application.ProposedSupervisorStaffNumber} "
                    + "does not exist.");
            }
        }

        if (!string.IsNullOrWhiteSpace(application.EnrolledStudentNumber))
        {
            var student =
                await _studentLookup.GetStudentSummaryAsync(
                    application.EnrolledStudentNumber,
                    cancellationToken);

            if (student is null)
            {
                throw new InvalidOperationException(
                    $"Cannot seed PhD application {application.ApplicationReference}: "
                    + $"student {application.EnrolledStudentNumber} "
                    + "does not exist.");
            }
        }

        var exists = await _phdApplicationDao.ApplicationExistsAsync(
            application.ApplicationReference,
            cancellationToken);

        if (exists)
        {
            return;
        }

        await _phdApplicationDao.AddApplicationAsync(
            application,
            cancellationToken);
    }
}

