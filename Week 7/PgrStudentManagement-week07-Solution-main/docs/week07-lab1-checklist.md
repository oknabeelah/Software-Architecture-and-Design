# Week 7 Lab 1 Check List: Codebase Changes Summary

A quick-reference guide to every file **added** or **modified** when evolving the Week 6 Clean Architecture solution into the Week 7 Academic Supervisors REST & Cross-Module HTTP solution.

---

## 1. Quick Overview by Clean Architecture Layer

| Layer / Project | Added Files | Modified Files | Primary Responsibility Added |
|---|---|---|---|
| **`PgrStudentManagement.Domain`** | 4 | 0 | Pure `AcademicSupervisor` & `SupervisoryAssignment` domain models and enums |
| **`PgrStudentManagement.Business`** | 19 | 0 | Academic Supervisors use cases, `IStudentLookup` output port, `LocalStudentLookupAdapter` (Part 1), and Students `GetStudentSummaryUseCase` (Part 2) |
| **`PgrStudentManagement.Adapters.Persistence`** | 13 | 1 | EF Core records, mappers, gateways, migrations, and idempotent `AcademicSupervisorDataSeeder` |
| **`PgrStudentManagement.Adapters.API`** | 10 | 1 | `AcademicSupervisorsApiController`, `StudentsApiController` lookup route, API DTOs/mappers, and `HttpStudentLookupAdapter` (Part 2) |
| **`PgrStudentManagement.Adapters.Web`** | 0 | 1 | Home dashboard links for new Week 7 REST endpoints |
| **`PgrStudentManagement.Application`** | 1 | 3 | Composition root DI wiring (`AddHttpClient`), `Services:Students:BaseUrl` config, startup migration/seeding, and `.http` test file |
| **`PgrStudentManagement.Tests`** | 10 | 2 | Unit, persistence, controller, architecture, and cross-module HTTP integration tests |

---

## 2. Step-by-Step Changes by Lab Task

### Part 1: Academic Supervisors REST API with Direct In-Process Student Lookup

#### Task 1 — Add Academic Supervisors to `PgrStudentManagement.Domain`
Pure C# domain classes and enums (zero ORM, HTTP, or framework dependencies):

| Status | File | Key Types & Behaviour |
|---|---|---|
| **Added** | [`Domain/AcademicSupervisors/SupervisoryRole.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Domain/AcademicSupervisors/SupervisoryRole.cs) | Enum: `DirectorOfStudies`, `FirstSupervisor`, `SecondSupervisor`, `Adviser` |
| **Added** | [`Domain/AcademicSupervisors/SupervisorEmploymentType.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Domain/AcademicSupervisors/SupervisorEmploymentType.cs) | Enum: `InstitutionEmployed`, `External` |
| **Added** | [`Domain/AcademicSupervisors/AcademicSupervisor.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Domain/AcademicSupervisors/AcademicSupervisor.cs) | Entity with `StaffNumber`, `FirstName`, `LastName`, `FullName`, `EmploymentType`, `EligibleRoles`, and `IsEligibleFor(SupervisoryRole)` |
| **Added** | [`Domain/AcademicSupervisors/SupervisoryAssignment.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Domain/AcademicSupervisors/SupervisoryAssignment.cs) | Entity with `StudentNumber`, `SupervisorStaffNumber`, `Role`, `StartDate`, `EndDate`, `ExceptionAuthorised`, and `Current` (`EndDate is null`) |

---

#### Task 2 — Add Academic Supervisors Use Cases & Ports to `PgrStudentManagement.Business`
Business interactors, input ports, boundary DTOs, and consumer-owned output ports:

| Status | File | Key Types & Behaviour |
|---|---|---|
| **Added** | [`Business/AcademicSupervisors/Gateway/StudentSummary.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Business/AcademicSupervisors/Gateway/StudentSummary.cs) | `record StudentSummary(string StudentNumber, string DisplayName)` |
| **Added** | [`Business/AcademicSupervisors/Gateway/IStudentLookup.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Business/AcademicSupervisors/Gateway/IStudentLookup.cs) | Consumer-owned output port: `GetStudentSummaryAsync(studentNumber, ct)` |
| **Added** | [`Business/AcademicSupervisors/Gateway/IAcademicSupervisorGateway.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Business/AcademicSupervisors/Gateway/IAcademicSupervisorGateway.cs) | Persistence output port for supervisors and supervisory assignments |
| **Added** | [`Business/AcademicSupervisors/AssignAcademicSupervisor/AssignmentOutcome.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Business/AcademicSupervisors/AssignAcademicSupervisor/AssignmentOutcome.cs) | Outcomes: `Created`, `StudentNotFound`, `SupervisorNotFound`, `DuplicateAssignment`, `RoleAlreadyFilled`, `SupervisorNotEligible`, `EmploymentTypeNotPermitted`, `ExceptionRequired` |
| **Added** | [`Business/AcademicSupervisors/AssignAcademicSupervisor/AssignAcademicSupervisorInput.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Business/AcademicSupervisors/AssignAcademicSupervisor/AssignAcademicSupervisorInput.cs) | Input DTO (`StudentNumber`, `StaffNumber`, `Role`, `StartDate`, `ExceptionAuthorised`) |
| **Added** | [`Business/AcademicSupervisors/AssignAcademicSupervisor/SupervisoryAssignmentOutput.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Business/AcademicSupervisors/AssignAcademicSupervisor/SupervisoryAssignmentOutput.cs) | Created assignment DTO |
| **Added** | [`Business/AcademicSupervisors/AssignAcademicSupervisor/AssignAcademicSupervisorResult.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Business/AcademicSupervisors/AssignAcademicSupervisor/AssignAcademicSupervisorResult.cs) | Wraps `AssignmentOutcome` and optional `SupervisoryAssignmentOutput` |
| **Added** | [`Business/AcademicSupervisors/AssignAcademicSupervisor/IAssignAcademicSupervisorUseCase.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Business/AcademicSupervisors/AssignAcademicSupervisor/IAssignAcademicSupervisorUseCase.cs) | Input port interface |
| **Added** | [`Business/AcademicSupervisors/AssignAcademicSupervisor/AssignAcademicSupervisorUseCase.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Business/AcademicSupervisors/AssignAcademicSupervisor/AssignAcademicSupervisorUseCase.cs) | Enforces all supervisory assignment business rules using `IStudentLookup` and `IAcademicSupervisorGateway` |
| **Added** | [`Business/AcademicSupervisors/ViewSupervisoryTeam/IViewSupervisoryTeamUseCase.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Business/AcademicSupervisors/ViewSupervisoryTeam/IViewSupervisoryTeamUseCase.cs)<br/>[`ViewSupervisoryTeamUseCase.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Business/AcademicSupervisors/ViewSupervisoryTeam/ViewSupervisoryTeamUseCase.cs)<br/>[`ViewSupervisoryTeamInput.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Business/AcademicSupervisors/ViewSupervisoryTeam/ViewSupervisoryTeamInput.cs)<br/>[`SupervisoryTeamOutput.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Business/AcademicSupervisors/ViewSupervisoryTeam/SupervisoryTeamOutput.cs)<br/>[`SupervisoryTeamMemberOutput.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Business/AcademicSupervisors/ViewSupervisoryTeam/SupervisoryTeamMemberOutput.cs) | Use case & DTOs for viewing a student's current and historical supervisory team |

---

#### Task 3 — Add Academic Supervisors to `PgrStudentManagement.Adapters.Persistence`
Relational EF Core models, mappers, gateway, migrations, and seeder:

| Status | File | Key Types & Behaviour |
|---|---|---|
| **Added** | [`Adapters.Persistence/Relational/Models/AcademicSupervisorRecord.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Adapters.Persistence/Relational/Models/AcademicSupervisorRecord.cs) | Maps to `AcademicSupervisors` table (PK: `StaffNumber`) |
| **Added** | [`Adapters.Persistence/Relational/Models/AcademicSupervisorEligibleRoleRecord.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Adapters.Persistence/Relational/Models/AcademicSupervisorEligibleRoleRecord.cs) | Maps to `AcademicSupervisorEligibleRoles` table (Composite PK: `SupervisorStaffNumber`, `Role`) |
| **Added** | [`Adapters.Persistence/Relational/Models/SupervisoryAssignmentRecord.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Adapters.Persistence/Relational/Models/SupervisoryAssignmentRecord.cs) | Maps to `SupervisoryAssignments` table (Composite PK: `StudentNumber`, `SupervisorStaffNumber`, `Role`, `StartDate`) |
| **Modified** | [`Adapters.Persistence/Relational/StudentManagementDbContext.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Adapters.Persistence/Relational/StudentManagementDbContext.cs) | Added `DbSet` properties and `OnModelCreating` configurations for the 3 Academic Supervisor tables |
| **Added** | [`Adapters.Persistence/Relational/Mappers/AcademicSupervisorPersistenceMapper.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Adapters.Persistence/Relational/Mappers/AcademicSupervisorPersistenceMapper.cs) | Bidirectional mapper between EF records and Domain entities |
| **Added** | [`Adapters.Persistence/Relational/Gateway/EfAcademicSupervisorGateway.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Adapters.Persistence/Relational/Gateway/EfAcademicSupervisorGateway.cs) | Implements `IAcademicSupervisorGateway` using EF Core (`Include(s => s.EligibleRoleEntries)`) |
| **Added** | [`Adapters.Persistence/Relational/AcademicSupervisorDataSeeder.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Adapters.Persistence/Relational/AcademicSupervisorDataSeeder.cs) | Asynchronous, idempotent seeder for `AS1001`–`AS1004` and initial assignments |
| **Added** | [`Adapters.Persistence/Relational/Migrations/20261103130000_AddAcademicSupervisors.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Adapters.Persistence/Relational/Migrations/20261103130000_AddAcademicSupervisors.cs) *(+ Designer & Snapshot)* | EF Core schema migrations for `Students`, `Enrolments`, and `AcademicSupervisors` tables |

---

#### Task 4 & Task 8 — Students Lookup Use Case & Direct In-Process Adapter
Provider-side Students lookup use case and Part 1's in-process `IStudentLookup` adapter:

| Status | File | Key Types & Behaviour |
|---|---|---|
| **Added** | [`Business/Students/Gateway/IStudentGateway.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Business/Students/Gateway/IStudentGateway.cs) | Students persistence output port (`GetByStudentNumberAsync`) |
| **Added** | [`Adapters.Persistence/Relational/Mappers/StudentPersistenceMapper.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Adapters.Persistence/Relational/Mappers/StudentPersistenceMapper.cs)<br/>[`Adapters.Persistence/Relational/Gateway/EfStudentGateway.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Adapters.Persistence/Relational/Gateway/EfStudentGateway.cs) | Implements `IStudentGateway` over `StudentManagementDbContext.Students` |
| **Added** | [`Business/Students/GetStudentSummary/IGetStudentSummaryUseCase.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Business/Students/GetStudentSummary/IGetStudentSummaryUseCase.cs)<br/>[`GetStudentSummaryUseCase.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Business/Students/GetStudentSummary/GetStudentSummaryUseCase.cs)<br/>[`GetStudentSummaryInput.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Business/Students/GetStudentSummary/GetStudentSummaryInput.cs)<br/>[`GetStudentSummaryOutput.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Business/Students/GetStudentSummary/GetStudentSummaryOutput.cs) | Provider-side Students use case returning `(Found, StudentNumber, DisplayName)` |
| **Added** | [`Business/AcademicSupervisors/Gateway/LocalStudentLookupAdapter.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Business/AcademicSupervisors/Gateway/LocalStudentLookupAdapter.cs) | **Part 1 Adapter:** Implements `IStudentLookup` via an in-process call to `IGetStudentSummaryUseCase` |

---

#### Task 5 — Expose Academic Supervisors in `PgrStudentManagement.Adapters.API`

| Status | File | Key Types & Behaviour |
|---|---|---|
| **Added** | [`Adapters.API/Requests/AssignAcademicSupervisorRequest.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Adapters.API/Requests/AssignAcademicSupervisorRequest.cs) | JSON request model (`StaffNumber`, `Role`, `StartDate`, `ExceptionAuthorised`) |
| **Added** | [`Adapters.API/Responses/AssignAcademicSupervisorResponse.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Adapters.API/Responses/AssignAcademicSupervisorResponse.cs)<br/>[`SupervisoryAssignmentResponse.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Adapters.API/Responses/SupervisoryAssignmentResponse.cs)<br/>[`SupervisoryTeamResponse.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Adapters.API/Responses/SupervisoryTeamResponse.cs) | REST transport response models |
| **Added** | [`Adapters.API/Mappers/AcademicSupervisorsApiMapper.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Adapters.API/Mappers/AcademicSupervisorsApiMapper.cs) | Maps requests to `AssignAcademicSupervisorInput` and `AssignmentOutcome` to HTTP response payloads |
| **Added** | [`Adapters.API/Controllers/AcademicSupervisorsApiController.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Adapters.API/Controllers/AcademicSupervisorsApiController.cs) | `GET` & `POST /api/students/{studentNumber}/academic-supervisors` (`201`, `404`, `409`, `422`) |

---

### Part 2: Expose Student Lookup via REST & Replace Direct Call with HTTP Adapter

#### Task 9 — Expose Student Lookup in `PgrStudentManagement.Adapters.API`

| Status | File | Key Types & Behaviour |
|---|---|---|
| **Added** | [`Adapters.API/Responses/StudentLookupResponse.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Adapters.API/Responses/StudentLookupResponse.cs) | `record StudentLookupResponse(string StudentNumber, string DisplayName)` |
| **Added** | [`Adapters.API/Mappers/StudentLookupApiMapper.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Adapters.API/Mappers/StudentLookupApiMapper.cs) | Maps `GetStudentSummaryOutput` to `StudentLookupResponse?` |
| **Modified** | [`Adapters.API/Controllers/StudentsApiController.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Adapters.API/Controllers/StudentsApiController.cs) | Added `[HttpGet("{studentNumber}/lookup")]` returning `200 OK` or `404 Not Found` |

---

#### Task 10 — Add Outgoing HTTP Adapter to `PgrStudentManagement.Adapters.API`

| Status | File | Key Types & Behaviour |
|---|---|---|
| **Added** | [`Adapters.API/AcademicSupervisors/StudentLookupClient/StudentLookupApiResponse.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Adapters.API/AcademicSupervisors/StudentLookupClient/StudentLookupApiResponse.cs) | Client-side JSON deserialisation record |
| **Added** | [`Adapters.API/AcademicSupervisors/StudentLookupClient/HttpStudentLookupAdapter.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Adapters.API/AcademicSupervisors/StudentLookupClient/HttpStudentLookupAdapter.cs) | **Part 2 Adapter:** Implements `IStudentLookup` over `HttpClient` (`GET api/students/{id}/lookup`); maps `200` &rarr; `StudentSummary`, `404` &rarr; `null`, other errors &rarr; `HttpRequestException` |

---

#### Task 11 — Composition Root & Configuration (`PgrStudentManagement.Application`)

| Status | File | Key Changes |
|---|---|---|
| **Modified** | [`Application/appsettings.json`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Application/appsettings.json)<br/>[`Application/appsettings.Development.json`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Application/appsettings.Development.json) | Added `"Services": { "Students": { "BaseUrl": "http://localhost:5066" } }` |
| **Modified** | [`Application/Program.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Application/Program.cs) | Registered new gateways, mappers, use cases, `AcademicSupervisorDataSeeder`, and switched `IStudentLookup` from `LocalStudentLookupAdapter` (Part 1) to `AddHttpClient<IStudentLookup, HttpStudentLookupAdapter>` (Part 2) |
| **Added** | [`Application/PgrStudentManagement.http`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Application/PgrStudentManagement.http) | Ready-to-run REST requests for testing lookup and supervisory assignment endpoints |

---

## 3. The Key DI Switch: Part 1 vs. Part 2 (`Program.cs`)

Notice that **zero lines of code changed in `AssignAcademicSupervisorUseCase`** between Part 1 and Part 2 — only the DI registration in [`Program.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Application/Program.cs#L73-L84):

```csharp
// Part 1 registration (direct in-process Student lookup):
// builder.Services.AddScoped<IStudentLookup, LocalStudentLookupAdapter>();

// Part 2 registration (cross-module HTTP Student lookup):
var studentsBaseUrl = builder.Configuration["Services:Students:BaseUrl"]
    ?? throw new InvalidOperationException(
        "The Students service base URL is not configured.");

builder.Services.AddHttpClient<IStudentLookup, HttpStudentLookupAdapter>(client =>
{
    client.BaseAddress = new Uri(studentsBaseUrl);
});
```

---

## 4. Automated Tests Added (`PgrStudentManagement.Tests`)

| Test File | What It Verifies |
|---|---|
| [`Business/AcademicSupervisors/AssignAcademicSupervisorUseCaseTests.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Tests/Business/AcademicSupervisors/AssignAcademicSupervisorUseCaseTests.cs) | All 8 `AssignmentOutcome` rules + in-process `LocalStudentLookupAdapter` integration |
| [`Business/AcademicSupervisors/ViewSupervisoryTeamUseCaseTests.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Tests/Business/AcademicSupervisors/ViewSupervisoryTeamUseCaseTests.cs) | Unknown student handling and current vs. historical assignment sorting |
| [`Business/Students/GetStudentSummaryUseCaseTests.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Tests/Business/Students/GetStudentSummaryUseCaseTests.cs) | Found, not-found, and blank student number handling |
| [`API/StudentLookupClient/HttpStudentLookupAdapterTests.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Tests/API/StudentLookupClient/HttpStudentLookupAdapterTests.cs) | `200 OK` deserialisation, `404 Not Found` &rarr; `null`, and `500` &rarr; `HttpRequestException` |
| [`API/Controllers/AcademicSupervisorsApiControllerTests.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Tests/API/Controllers/AcademicSupervisorsApiControllerTests.cs) | HTTP status code mapping (`201`, `404`, `409`, `422`) |
| [`API/Controllers/StudentsApiControllerTests.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Tests/API/Controllers/StudentsApiControllerTests.cs) | `GET /api/students/{studentNumber}/lookup` (`200 OK` and `404 Not Found`) |
| [`API/CrossModuleHttpIntegrationTests.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Tests/API/CrossModuleHttpIntegrationTests.cs) | End-to-end flow: `AcademicSupervisorsApiController` &rarr; `AssignAcademicSupervisorUseCase` &rarr; `HttpStudentLookupAdapter` &rarr; `StudentsApiController` |
| [`Persistence/Relational/AcademicSupervisorGatewayAndSeederTests.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Tests/Persistence/Relational/AcademicSupervisorGatewayAndSeederTests.cs) | SQLite migration, `EfAcademicSupervisorGateway`, and idempotent double-execution of `AcademicSupervisorDataSeeder` |
| [`Architecture/DomainDependencyTests.cs`](PgrStudentManagement-week07-Solution/PgrStudentManagement.Tests/Architecture/DomainDependencyTests.cs) | Verifies `Domain` and `Business` do not reference `EntityFrameworkCore`, `AspNetCore`, `System.Net.Http`, or outer adapter assemblies |

