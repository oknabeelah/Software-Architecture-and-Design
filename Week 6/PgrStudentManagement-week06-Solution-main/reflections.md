# Week 6 Lab 1: Clean Architecture Reflections

This document provides answers to the architectural reflection questions posed in [week06-lab1-instructions.md](docs/week06-lab1-instructions.md) based on the completed Clean Architecture refactoring of **W5-UC01: View Student Enrolments**.

---

## 1. Which project owns `IEnrolmentGateway`, and why?

**Project:** `PgrStudentManagement.Business`

**Rationale:**
- In Clean Architecture and the Dependency Inversion Principle (DIP), high-level business logic must not depend on low-level implementation details (such as Entity Framework Core, SQLite, or MongoDB).
- Instead, the use case owns and defines the abstraction for whatever external persistence operations it requires.
- Placing `IEnrolmentGateway` in `PgrStudentManagement.Business.Students.Gateway` ensures that the data access requirements are dictated by business use cases rather than by the structure of a database or ORM framework.

---

## 2. What is the difference between the input port and the output port?

| Port Type | Interface | Primary Purpose | Invocation Flow |
|---|---|---|---|
| **Input Port** | `IViewStudentEnrolmentsUseCase` | Defines the API through which incoming drivers (e.g., MVC controller, REST API controller) request business operations. | **Incoming Adapter &rarr; Business** (Driver &rarr; Application core) |
| **Output Port** | `IEnrolmentGateway` | Defines the contract that the use case requires from external driven components (e.g., database, message bus, third-party service). | **Business &rarr; Outgoing Adapter** (Application core &rarr; Driven adapter) |

- **Input Port:** The use case *implements* the input port interface.
- **Output Port:** The persistence adapter *implements* the output port interface, while the use case *calls* it.

---

## 3. How can runtime control reach the EF gateway when Business does not reference Persistence?

Control reaches the persistence gateway through **Dependency Inversion** configured in the **Composition Root**:

1. **Compile-time decoupling:**
   `PgrStudentManagement.Business` has no reference to `PgrStudentManagement.Adapters.Persistence`. At compile time, `Business` only knows about the interface `IEnrolmentGateway`.
2. **Composition Root binding:**
   The executable host (`PgrStudentManagement.Application`) references both `Business` and `Adapters.Persistence`. During startup in `Program.cs`, the dependency injection container registers the implementation:
   ```csharp
   builder.Services.AddScoped<IEnrolmentGateway, EfEnrolmentGateway>();
   ```
3. **Runtime dependency injection:**
   When a controller invokes `ViewStudentEnrolmentsUseCase`, the DI container instantiates `EfEnrolmentGateway` and passes it into the use case constructor as `IEnrolmentGateway`. Control passes dynamically through polymorphism.

---

## 4. Why are Domain entities separate from EF records?

1. **Separation of Concerns:**
   - **Domain entities** (`Enrolment`, `Student`) represent core business entities, business invariants, and domain logic. They must remain pure and free from framework-specific annotations.
   - **Persistence records** (`EnrolmentRecord`, `StudentRecord`) represent relational schema details, database column mappings, foreign keys, table relationships, and ORM tracking configurations.
2. **Preventing Framework Leakage:**
   Embedding EF Core attributes (e.g., `[Key]`, `[ForeignKey]`, `[Table]`, `[Index]`) or navigation collections into domain models couples the domain to a specific ORM and storage mechanism.
3. **Independent Evolution:**
   A schema change (e.g., partitioning, denormalization, column renaming, or migrating to NoSQL) should only affect persistence records and mappers without rippling into business rules or domain models.

---

## 5. Why do MVC and REST use different boundary models?

MVC and REST serve fundamentally different consumers:

1. **MVC View Models (`StudentEnrolmentsViewModel`, `EnrolmentViewModel`):**
   - Designed for human presentation in HTML templates.
   - Values are pre-formatted for rendering (e.g., dates as `"01 October 2024"`, missing values as `"Not recorded"`).
   - May contain UI-specific state, display helper properties (e.g., `HasEnrolments`), and validation tags for forms.
2. **REST Response Models (`StudentEnrolmentsResponse`, `EnrolmentResponse`):**
   - Designed for programmatic consumption and machine-to-machine integrations.
   - Preserve typed data representations (e.g., ISO-8601 strings or `DateTime` properties, nullable fields instead of presentation strings like `"Not recorded"`).
   - Adhere to strict API versioning and serialization contracts.

Sharing a single model across MVC and REST would compromise either presentation usability or API contract stability.

---

## 6. Which classes and behaviour are shared by the MVC and REST paths?

Both incoming adapter paths share:

- **Input Port Interface:** `IViewStudentEnrolmentsUseCase`
- **Use Case Interactor:** `ViewStudentEnrolmentsUseCase`
- **Use Case DTOs:** `ViewStudentEnrolmentsInput`, `StudentEnrolmentsOutput`, `EnrolmentOutput`
- **Output Port Interface:** `IEnrolmentGateway`
- **Persistence Gateway Implementation:** `EfEnrolmentGateway`
- **Domain Entities:** `Enrolment`, `EnrolmentStatus`
- **Shared Business Behaviour:**
  - Enrolment retrieval for a specific student number.
  - Filtering to exclude unrelated students' enrolments.
  - Strict chronological ordering (earliest to latest by `StartDate`).
  - Read-only execution with zero mutation of persistent data.
  - Preserving an empty collection for students with no recorded enrolments.

---

## 7. What would have to change if SQLite were replaced?

If SQLite were replaced with another database technology (such as PostgreSQL, Microsoft SQL Server, or MongoDB):

1. **Inside `PgrStudentManagement.Adapters.Persistence`:**
   - Update the EF provider package (e.g., replace `Microsoft.EntityFrameworkCore.Sqlite` with `Npgsql.EntityFrameworkCore.PostgreSQL` or `Microsoft.EntityFrameworkCore.SqlServer`).
   - Adjust column data types, value generators, or migrations if required by the new dialect.
   - If migrating to a document database like MongoDB, implement `IEnrolmentGateway` with a Mongo gateway and document mapper.
2. **Inside `PgrStudentManagement.Application`:**
   - Update the connection string and DI configuration in `Program.cs` / `appsettings.json` (e.g., `options.UseNpgsql(...)` or `options.UseSqlServer(...)`).
3. **No changes required in:**
   - `PgrStudentManagement.Domain`
   - `PgrStudentManagement.Business`
   - `PgrStudentManagement.Adapters.Web`
   - `PgrStudentManagement.Adapters.API`
   - `PgrStudentManagement.Tests` (business use case tests remain 100% valid because they use `FakeEnrolmentGateway`).

---

## 8. How do project references help enforce the Dependency Rule?

In .NET, source code dependencies are strictly constrained by MSBuild project references (`<ProjectReference>` in `.csproj` files):

1. **Compile-Time Enforcement:**
   - If a developer in `PgrStudentManagement.Business` attempts to import `using Microsoft.EntityFrameworkCore;` or `using PgrStudentManagement.Adapters.Persistence;`, the compiler throws a CS0246 error because `Business.csproj` only references `Domain.csproj`.
   - Incoming adapters (`Adapters.Web` and `Adapters.API`) do not reference `Adapters.Persistence`, preventing controllers from bypassing the use case and directly injecting `DbContext`.
2. **Prevention of Circular Dependencies:**
   MSBuild disallows circular project references, preventing bad architectural drift and keeping the dependency hierarchy clear and unidirectional.
3. **Complementary Automated Architecture Tests:**
   Unit tests like [`DomainDependencyTests.cs`](PgrStudentManagement.Tests/Architecture/DomainDependencyTests.cs) inspect assembly metadata at test execution time to guarantee that the compiled assemblies do not inadvertently pull in prohibited dependencies.

---

# Extension Tasks Documentation & Reflections

## Extension 1: Independently Refactoring W5-UC04 (Record Research Activity)

### 1. Architectural Distribution Across Layers
- **Domain (`PgrStudentManagement.Domain/ResearchActivities`):**
  - [`ResearchActivity.cs`](PgrStudentManagement.Domain/ResearchActivities/ResearchActivity.cs): Pure domain entity containing business attributes (`Id`, `StudentNumber`, `ActivityType`, `Title`, `ActivityDate`, `CreatedOn`, `Tags`, `Details`). Free from BSON, Mongo, or EF Core annotations.
  - [`ResearchActivityType.cs`](PgrStudentManagement.Domain/ResearchActivities/ResearchActivityType.cs): Strongly typed enum defining supported activity types (`ResearchReflection`, `SupervisionMeeting`, `ConferenceAttendance`, `Publication`, `Experiment`, `TrainingEvent`).
- **Business (`PgrStudentManagement.Business/ResearchActivities`):**
  - **Output Port:** [`IResearchActivityGateway.cs`](PgrStudentManagement.Business/ResearchActivities/Gateway/IResearchActivityGateway.cs) defining asynchronous persistence contracts `AddAsync` and `GetForStudentAsync`.
  - **Input Port:** `IRecordResearchActivityUseCase.cs`.
  - **Interactor:** [`RecordResearchActivityUseCase.cs`](PgrStudentManagement.Business/ResearchActivities/RecordResearchActivity/RecordResearchActivityUseCase.cs):
    - Validates required inputs (non-empty student number and title, valid non-default date).
    - Rejects unsupported activity types.
    - Generates application-defined string identifiers (`activity-{guid}`).
    - Assigns UTC creation timestamp (`DateTime.UtcNow`).
    - Calls `IResearchActivityGateway.AddAsync` **only** after all validations pass.
  - **Boundary DTOs:** `RecordResearchActivityInput.cs` and `RecordResearchActivityResult.cs`.
- **Persistence Adapter (`PgrStudentManagement.Adapters.Persistence/Documents`):**
  - **Document Model:** [`ResearchActivityDocument.cs`](PgrStudentManagement.Adapters.Persistence/Documents/Models/ResearchActivityDocument.cs) containing MongoDB-specific BSON attributes (`[BsonId]`, `[BsonRepresentation]`, `[BsonElement]`).
  - **Document Mapper:** [`ResearchActivityDocumentMapper.cs`](PgrStudentManagement.Adapters.Persistence/Documents/Mappers/ResearchActivityDocumentMapper.cs) converting bidirectionally between `ResearchActivityDocument` and `ResearchActivity`.
  - **Document Gateway:** [`MongoResearchActivityGateway.cs`](PgrStudentManagement.Adapters.Persistence/Documents/Gateway/MongoResearchActivityGateway.cs) implementing `IResearchActivityGateway`. Interacts with MongoDB `IMongoCollection<ResearchActivityDocument>` when available, with a resilient in-memory fallback store for offline testing.
- **Incoming Web Adapter (`PgrStudentManagement.Adapters.Web`):**
  - **Controller:** [`ResearchActivitiesController.cs`](PgrStudentManagement.Adapters.Web/Controllers/ResearchActivitiesController.cs) (`[HttpGet] Record`, `[HttpPost] Record`).
  - **View Model:** `RecordResearchActivityViewModel.cs` with validation attributes, dropdown selections, and error display.
  - **Web Mapper:** `ResearchActivityWebMapper.cs` converting form data to `RecordResearchActivityInput`.
  - **Razor View:** [`Record.cshtml`](PgrStudentManagement.Adapters.Web/Views/ResearchActivities/Record.cshtml) with accessible Bootstrap inputs and Clean Architecture explanations.
- **Incoming API Adapter (`PgrStudentManagement.Adapters.API`):**
  - **Controller:** [`ResearchActivitiesApiController.cs`](PgrStudentManagement.Adapters.API/Controllers/ResearchActivitiesApiController.cs) (`POST api/students/{studentNumber}/research-activities`).
  - **Request / Response DTOs:** `RecordResearchActivityRequest.cs` and `RecordResearchActivityResponse.cs`.
  - **API Mapper:** `ResearchActivityApiMapper.cs`.
- **Unit Testing:**
  - Tested in [`RecordResearchActivityUseCaseTests.cs`](PgrStudentManagement.Tests/Business/ResearchActivities/RecordResearchActivityUseCaseTests.cs) using [`FakeResearchActivityGateway.cs`](PgrStudentManagement.Tests/TestDoubles/FakeResearchActivityGateway.cs), verifying valid creation, required field validation, unsupported type rejection, and zero persistence calls on validation failure.

---

## Extension 2: Refactoring W5-UC03 (View Student Research Activities)

### 1. Architectural Distribution Across Layers
- **Business (`PgrStudentManagement.Business/ResearchActivities/ViewStudentResearchActivities`):**
  - **Input Port:** `IViewStudentResearchActivitiesUseCase.cs`.
  - **Interactor:** [`ViewStudentResearchActivitiesUseCase.cs`](PgrStudentManagement.Business/ResearchActivities/ViewStudentResearchActivities/ViewStudentResearchActivitiesUseCase.cs).
  - **Business Responsibility:** Retrieves activities from `IResearchActivityGateway.GetForStudentAsync` and enforces strict **chronological ordering** (earliest to latest by `ActivityDate`) at the Business level rather than relying on external database sort order.
  - **Boundary DTOs:** `ViewStudentResearchActivitiesInput.cs`, `StudentResearchActivitiesOutput.cs`, and `ResearchActivityItemOutput.cs`.
- **Web Adapter (`PgrStudentManagement.Adapters.Web`):**
  - **Controller Action:** `ResearchActivitiesController.Index(string studentNumber)`.
  - **View Models:** `StudentResearchActivitiesViewModel.cs` and `ResearchActivityItemViewModel.cs`.
  - **Razor View:** [`Index.cshtml`](PgrStudentManagement.Adapters.Web/Views/ResearchActivities/Index.cshtml) rendering a clean, responsive activities table, tag badges, and "No research activities found" empty state.
- **API Adapter (`PgrStudentManagement.Adapters.API`):**
  - **Controller Endpoint:** `GET api/students/{studentNumber}/research-activities`.
  - **Response Models:** `StudentResearchActivitiesResponse.cs` and `ResearchActivityResponse.cs`.
- **Unit Testing:**
  - Verified in [`ViewStudentResearchActivitiesUseCaseTests.cs`](PgrStudentManagement.Tests/Business/ResearchActivities/ViewStudentResearchActivitiesUseCaseTests.cs) confirming matching activities, business-enforced chronological ordering, isolation of unrelated students' activities, and empty list handling.

---

## Extension 3: Publish Inspection and Deployment Reflections

### 1. Publication Command & Artifacts
The application was published in `Release` configuration using:
```bash
dotnet publish PgrStudentManagement.Application --configuration Release
```
Output path: `PgrStudentManagement.Application/bin/Release/net10.0/publish/`

### 2. Assembly Identification
The published output folder contains the compiled assemblies representing each layer and adapter:

| Clean Architecture Layer | Assembly Name | Role & Contents |
|---|---|---|
| **Domain** | `PgrStudentManagement.Domain.dll` | Core business entities (`Enrolment`, `Student`, `ResearchActivity`) and domain enums. Contains zero framework or persistence dependencies. |
| **Business** | `PgrStudentManagement.Business.dll` | Application use cases and interactors (`ViewStudentEnrolmentsUseCase`, `RecordResearchActivityUseCase`, `ViewStudentResearchActivitiesUseCase`), input ports, and output port gateway interfaces (`IEnrolmentGateway`, `IResearchActivityGateway`). |
| **Persistence Adapter** | `PgrStudentManagement.Adapters.Persistence.dll` | Outgoing persistence implementations (`EfEnrolmentGateway`, `MongoResearchActivityGateway`), EF DbContext/migrations, and MongoDB document models/mappers. |
| **Web Adapter** | `PgrStudentManagement.Adapters.Web.dll` | Incoming Razor Class Library containing MVC controllers (`StudentsController`, `ResearchActivitiesController`), Razor views, view models, and presentation mappers. |
| **API Adapter** | `PgrStudentManagement.Adapters.API.dll` | Incoming REST API controllers (`StudentsApiController`, `ResearchActivitiesApiController`), request/response models, and API serialization mappers. |
| **Application Host** | `PgrStudentManagement.Application.dll` & `PgrStudentManagement.Application` | Composition Root (`Program.cs`), configuration files (`appsettings.json`), static web assets (`wwwroot/`), and executable binary that bootstraps the DI container and web server. |

### 3. Why Several Assemblies Form One Deployed Application

In Clean Architecture, modularity and deployment topology are distinct design concerns:

1. **Logical Modularity vs. Physical Topology (Modular Monolith):**
   - Structuring the system into separate projects/assemblies (`Domain`, `Business`, `Adapters.Persistence`, `Adapters.Web`, `Adapters.API`, `Application`) enforces strict compile-time boundaries, encapsulation, and the Dependency Rule. Developers cannot accidentally bypass business logic or tightly couple UI/API controllers to database schemas.
   - However, separating code into multiple assemblies does **not** mandate splitting them into distributed microservices or separate server processes.
2. **Single Process Execution:**
   - At runtime, `PgrStudentManagement.Application` is the sole entry point. When launched, the .NET runtime loads all referenced assemblies into a **single process (AppDomain / AssemblyLoadContext)**.
   - Inter-assembly calls (such as `StudentsController` invoking `IViewStudentEnrolmentsUseCase` and `RecordResearchActivityUseCase` invoking `IResearchActivityGateway`) execute as high-performance **in-process memory method calls**, without HTTP, network serialization, or socket latency.
3. **Inversion of Control via the Composition Root:**
   - The `Application` project serves as the Composition Root. It binds the independent assemblies together through Dependency Injection at startup, wiring incoming drivers to use cases and use cases to persistence gateways.
4. **Operational Simplicity:**
   - Packaging the multiple assemblies into a single deployment folder simplifies deployment, monitoring, transactional consistency, configuration management, and hosting (whether hosted in IIS, Kestrel on Linux, Docker, or Azure App Service), while retaining all the architectural rigor, testability, and maintainability of Clean Architecture.


