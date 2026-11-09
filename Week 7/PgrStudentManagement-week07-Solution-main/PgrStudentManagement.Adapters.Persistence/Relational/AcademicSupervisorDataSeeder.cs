using PgrStudentManagement.Business.AcademicSupervisors.Gateway;
using PgrStudentManagement.Business.Students.Gateway;
using PgrStudentManagement.Domain.AcademicSupervisors;

namespace PgrStudentManagement.Adapters.Persistence.Relational;

public sealed class AcademicSupervisorDataSeeder
{
    private readonly IAcademicSupervisorGateway _supervisorGateway;
    private readonly IStudentGateway _studentGateway;

    public AcademicSupervisorDataSeeder(
        IAcademicSupervisorGateway supervisorGateway,
        IStudentGateway studentGateway)
    {
        _supervisorGateway = supervisorGateway;
        _studentGateway = studentGateway;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedAcademicSupervisorsAsync(cancellationToken);

        await SeedAssignmentIfPossibleAsync(
            studentNumber: "S100001",
            staffNumber: "AS1001",
            role: SupervisoryRole.DirectorOfStudies,
            startDate: new DateTime(2025, 10, 1),
            cancellationToken: cancellationToken);

        await SeedAssignmentIfPossibleAsync(
            studentNumber: "S100001",
            staffNumber: "AS1002",
            role: SupervisoryRole.FirstSupervisor,
            startDate: new DateTime(2025, 10, 1),
            cancellationToken: cancellationToken);

        await SeedAssignmentIfPossibleAsync(
            studentNumber: "S100001",
            staffNumber: "AS1003",
            role: SupervisoryRole.SecondSupervisor,
            startDate: new DateTime(2024, 10, 1),
            cancellationToken: cancellationToken,
            endDate: new DateTime(2025, 9, 30));

        await SeedAssignmentIfPossibleAsync(
            studentNumber: "S100002",
            staffNumber: "AS1001",
            role: SupervisoryRole.FirstSupervisor,
            startDate: new DateTime(2026, 1, 15),
            cancellationToken: cancellationToken);

        await SeedAssignmentIfPossibleAsync(
            studentNumber: "S100002",
            staffNumber: "AS1002",
            role: SupervisoryRole.DirectorOfStudies,
            startDate: new DateTime(2025, 10, 1),
            cancellationToken: cancellationToken);

        await SeedAssignmentIfPossibleAsync(
            studentNumber: "S100002",
            staffNumber: "AS1003",
            role: SupervisoryRole.Adviser,
            startDate: new DateTime(2025, 10, 1),
            cancellationToken: cancellationToken);
    }

    private async Task SeedAcademicSupervisorsAsync(CancellationToken cancellationToken)
    {
        await AddAcademicSupervisorIfMissingAsync(
            new AcademicSupervisor
            {
                StaffNumber = "AS1001",
                FirstName = "Robin",
                LastName = "Carter",
                EmailAddress = "robin.carter@example.test",
                EmploymentType = SupervisorEmploymentType.InstitutionEmployed,
                TrainingCompleted = true,
                EligibleRoles = new HashSet<SupervisoryRole>
                {
                    SupervisoryRole.DirectorOfStudies,
                    SupervisoryRole.FirstSupervisor,
                    SupervisoryRole.SecondSupervisor
                }
            },
            cancellationToken);

        await AddAcademicSupervisorIfMissingAsync(
            new AcademicSupervisor
            {
                StaffNumber = "AS1002",
                FirstName = "Jamie",
                LastName = "Ahmed",
                EmailAddress = "jamie.ahmed@example.test",
                EmploymentType = SupervisorEmploymentType.InstitutionEmployed,
                TrainingCompleted = true,
                EligibleRoles = new HashSet<SupervisoryRole>
                {
                    SupervisoryRole.DirectorOfStudies,
                    SupervisoryRole.FirstSupervisor,
                    SupervisoryRole.SecondSupervisor
                }
            },
            cancellationToken);

        await AddAcademicSupervisorIfMissingAsync(
            new AcademicSupervisor
            {
                StaffNumber = "AS1003",
                FirstName = "Taylor",
                LastName = "Green",
                EmailAddress = "taylor.green@example.test",
                EmploymentType = SupervisorEmploymentType.External,
                TrainingCompleted = true,
                EligibleRoles = new HashSet<SupervisoryRole>
                {
                    SupervisoryRole.SecondSupervisor,
                    SupervisoryRole.Adviser
                }
            },
            cancellationToken);

        await AddAcademicSupervisorIfMissingAsync(
            new AcademicSupervisor
            {
                StaffNumber = "AS1004",
                FirstName = "Avery",
                LastName = "Brown",
                EmailAddress = "avery.brown@example.test",
                EmploymentType = SupervisorEmploymentType.InstitutionEmployed,
                TrainingCompleted = true,
                EligibleRoles = new HashSet<SupervisoryRole>
                {
                    SupervisoryRole.FirstSupervisor,
                    SupervisoryRole.SecondSupervisor,
                    SupervisoryRole.Adviser
                }
            },
            cancellationToken);
    }

    private async Task AddAcademicSupervisorIfMissingAsync(
        AcademicSupervisor supervisor,
        CancellationToken cancellationToken)
    {
        var exists = await _supervisorGateway.StaffNumberExistsAsync(
            supervisor.StaffNumber,
            cancellationToken);

        if (!exists)
        {
            await _supervisorGateway.AddSupervisorAsync(supervisor, cancellationToken);
        }
    }

    private async Task SeedAssignmentIfPossibleAsync(
        string studentNumber,
        string staffNumber,
        SupervisoryRole role,
        DateTime startDate,
        CancellationToken cancellationToken,
        DateTime? endDate = null)
    {
        startDate = startDate.Date;
        endDate = endDate?.Date;

        var student = await _studentGateway.GetByStudentNumberAsync(
            studentNumber,
            cancellationToken);

        if (student is null)
        {
            throw new InvalidOperationException(
                $"Cannot seed supervisory assignment: student {studentNumber} does not exist.");
        }

        var supervisor = await _supervisorGateway.GetByStaffNumberAsync(
            staffNumber,
            cancellationToken);

        if (supervisor is null)
        {
            throw new InvalidOperationException(
                $"Cannot seed supervisory assignment: Academic Supervisor {staffNumber} does not exist.");
        }

        var assignmentAlreadyExists = await _supervisorGateway.AssignmentExistsAsync(
            studentNumber,
            staffNumber,
            role,
            startDate,
            cancellationToken);

        if (assignmentAlreadyExists)
        {
            return;
        }

        await _supervisorGateway.AddAssignmentAsync(
            new SupervisoryAssignment
            {
                StudentNumber = studentNumber,
                SupervisorStaffNumber = staffNumber,
                Role = role,
                StartDate = startDate,
                EndDate = endDate,
                ExceptionAuthorised = false
            },
            cancellationToken);
    }
}

