# Week 7 Laboratory 1: From Modules to Services

## Software Architecture and Design

**Duration:** 2 hours  
**Starting point:** The supplied Week 6 reference solution  
**Focus:** Add the Academic Supervisors module, expose it through REST, trace its direct Student lookup, and then replace that local lookup with an HTTP interaction

## 1. Purpose

In Week 5, you implemented relational persistence for the Students module and MongoDB persistence for the Research Activities module. The Academic Supervisors module had originally been introduced in Week 4 using in-memory storage. Refactoring that module to use relational persistence was an extension task in Week 5.

The complete Week 5 reference solution includes an EF Core implementation of Academic Supervisors that deliberately uses the `StudentManagementDbContext` owned by the Students module:

[Week 5 complete reference solution](https://github.com/mustbebuilt/PgrStudentManagement-week05-Solution-complete)

The Week 6 reference solution reorganises the application using Clean Architecture, but it does not include a Clean Architecture implementation of Academic Supervisors because that work was left as an extension task:

[Week 6 reference solution](https://github.com/mustbebuilt/PgrStudentManagement-week06-Solution)

In this laboratory, you will use the Week 6 solution as your starting point and the Week 5 Academic Supervisors implementation as the functional reference.

You will complete the work in two parts:

1. Add Academic Supervisors to the Week 6 Clean Architecture and expose its use cases through REST. At this stage, Academic Supervisors must continue to obtain Student information through a direct, in-process call to `IStudentLookup`.
2. Expose the Students implementation of `IStudentLookup` through REST, then replace the direct Academic Supervisors lookup with an HTTP adapter.

The aim is to observe what changes when an existing module interaction becomes a service interaction. Clean Architecture should remain visible on both sides of the HTTP boundary.

## 2. Important Architectural Constraints

Your implementation must preserve the following rules:

- Students owns Student information and the Student lookup capability.
- Academic Supervisors consumes Student information through `IStudentLookup`.
- Academic Supervisors Business code must not query Students persistence directly.
- REST controllers are incoming adapters. They must not contain Business rules or access `DbContext` directly.
- HTTP-specific code belongs in an outer adapter, not in Business or Domain.
- The first version of the Academic Supervisors module may continue to use the relational database and `StudentManagementDbContext` pattern shown in the Week 5 reference solution.

## 3. Preparation

Before starting, confirm that you have:

- cloned or downloaded the Week 6 reference solution;
- access to the Week 5 complete reference solution;
- .NET 10 installed;
- the EF Core local or global tool available;
- a REST client such as Postman, an IDE `.http` client, or `curl`;
- a working SQLite database created from the Week 6 migrations; and
- the ability to run and test the application through a REST client.

From the Week 6 solution directory, run:

```bash
dotnet tool restore
dotnet restore
dotnet build
dotnet test
```

Run the application and verify that the Students functionality works before changing the solution.

```bash
dotnet run --project PgrStudentManagement.Application
```

Use the actual executable project path if it differs.

## 4. Reference Implementations

Use the two repositories for different purposes:

### Week 5 reference

Use the Academic Supervisors module in the Week 5 solution to identify:

- the Domain concepts;
- the Academic Supervisor entities and enums;
- supervisory assignment rules;
- controller behaviour;
- relational table requirements;
- data-seeding values;
- Student lookup through `IStudentLookup`; and
- the EF Core implementation using `StudentManagementDbContext`.

Do not copy the Week 5 MVC structure unchanged into Week 7. Reorganise the capability according to the Clean Architecture projects in the Week 6 solution.

### Week 6 reference

Use the Week 6 solution to determine:

- project responsibilities;
- dependency direction;
- input-port and output-port placement;
- adapter organisation;
- dependency-injection registration;
- API adapter conventions; and
- where the new Academic Supervisors classes should be placed.

## 5. Target Project Responsibilities

Organise the Academic Supervisors capability across the existing Week 6 projects.

```text
PgrStudentManagement.Domain
    Academic Supervisor and Supervisory Assignment concepts

PgrStudentManagement.Business
    Academic Supervisor use cases
    input ports
    Business DTOs and outcomes
    IStudentLookup
    persistence output ports

PgrStudentManagement.Adapters.Persistence
    EF Core Academic Supervisor gateways
    relational configuration
    seed data

PgrStudentManagement.Adapters.Web
    MVC adapter, where retained

PgrStudentManagement.Adapters.API
    Academic Supervisors REST controller
    request and response models
    API mappers
    HTTP Student lookup adapter, added in Part 2

PgrStudentManagement.Application
    dependency injection
    configuration
    migrations and startup composition
```

Use the actual namespaces and conventions in the Week 6 reference solution.

---

# Part 1: Academic Supervisors REST API with Direct Student Lookup

## Task 1: Add Academic Supervisors to `PgrStudentManagement.Domain`

### Objective

Move the Academic Supervisors business concepts from the Week 5 reference implementation into the Week 6 Domain project.

### Steps

1. Inspect the Academic Supervisors models and enums in the Week 5 reference solution.
2. Add the minimum Domain concepts needed by the selected use case, including:
   - `AcademicSupervisor`;
   - `SupervisoryAssignment`;
   - `SupervisoryRole`;
   - `SupervisorEmploymentType`; and
   - any Domain behaviour required by the assignment rules.
3. Keep the Domain free from ASP.NET Core, EF Core, HTTP, controller, and database concerns.
4. Preserve the business terminology and rules represented by the Week 5 implementation.
5. Avoid copying MVC view models, API models, DAO classes, or `DbContext` code into Domain.

### Checkpoint

`PgrStudentManagement.Domain` should compile without referencing Business or any adapter project.

## Task 2: Add Academic Supervisors to `PgrStudentManagement.Business`

### Objective

Add the Academic Supervisors use case, ports, DTOs, and outcomes to the Business project.

### Steps

1. Select the Academic Supervisor assignment use case from the Week 5 implementation.
2. Add an input port for the use case.
3. Add the Business input DTO required to request an assignment.
4. Add explicit Business outcomes for the relevant rules, such as:
   - Student not found;
   - Academic Supervisor not found;
   - duplicate assignment;
   - unique role already filled;
   - supervisor not eligible;
   - employment type not permitted;
   - exception required; and
   - assignment created.
5. Add Business-owned persistence output ports for Academic Supervisors and supervisory assignments.
6. Add or retain `IStudentLookup` as an Academic Supervisors Business-owned output port used to obtain a Student summary.
7. Implement the Academic Supervisors use case using only Domain types, Business DTOs, and Business-owned ports.
8. Keep EF Core, `DbContext`, controllers, HTTP, and JSON out of Business.

A Student lookup contract may resemble:

```csharp
public interface IStudentLookup
{
    Task<StudentSummary?> GetStudentSummaryAsync(
        string studentNumber,
        CancellationToken cancellationToken = default);
}
```

### Required Business Dependency

```text
Academic Supervisors use case
    -> IStudentLookup
```

At this stage, `IStudentLookup` is implemented by a local adapter that delegates to the Students capability in the same process. No HTTP call is involved.

### Checkpoint

The Business project should compile without referencing `PgrStudentManagement.Adapters.Persistence`, `PgrStudentManagement.Adapters.API`, or `PgrStudentManagement.Application`.

## Task 3: Add Academic Supervisors to `PgrStudentManagement.Adapters.Persistence`

### Objective

Implement the Business-owned persistence ports using EF Core and the Week 5 relational implementation as a reference.

### Steps

1. Add the Academic Supervisor and supervisory assignment persistence adapter classes.
2. Extend `StudentManagementDbContext` with the required Academic Supervisor tables.
3. Configure keys, relationships, and enum persistence.
4. Store eligible roles in a separate relational table.
5. Implement the Business-owned persistence output ports.
6. Add seed data based on the Week 5 reference solution.
7. Ensure the seeders are asynchronous and idempotent.
8. Register the persistence adapter and seeders in the Application composition root.

The relational model should contain tables equivalent to:

```text
AcademicSupervisors
AcademicSupervisorEligibleRoles
SupervisoryAssignments
```

The eligible-role table should have a composite key such as:

```text
SupervisorStaffNumber
Role
```

The supervisory assignment identity should prevent duplicate seed rows and assignments.

### Migration

Create and apply a migration:

```bash
dotnet tool run dotnet-ef migrations add AddAcademicSupervisors \
  --project PgrStudentManagement.Adapters.Persistence \
  --startup-project PgrStudentManagement.Application

dotnet tool run dotnet-ef database update \
  --project PgrStudentManagement.Adapters.Persistence \
  --startup-project PgrStudentManagement.Application
```

Adapt the paths to the supplied solution.

### Checkpoint

Run the application twice. The second startup must not create duplicate-key failures.

## Task 4: Connect `IStudentLookup` Through a Direct Call

### Objective

Complete the first implementation using the local Students implementation of `IStudentLookup`.

### Steps

1. Identify the existing Students input port or service that retrieves the required Student summary.
2. Add or identify a local `IStudentLookup` adapter that delegates to that Students capability in the same process.
3. Register the local adapter in `PgrStudentManagement.Application`.
4. Inject `IStudentLookup` into the Academic Supervisors use case.
5. Use the lookup when assigning an Academic Supervisor and when retrieving Student-related supervisory information.
6. Ensure an unknown Student is represented as a distinct Business outcome.

A reference registration is:

```csharp
builder.Services.AddScoped<IStudentLookup, LocalStudentLookupAdapter>();
```

Use the actual Students input port, adapter names, and registration conventions in the Week 6 solution. If the supplied implementation already allows `StudentService` to implement `IStudentLookup` directly, that arrangement may be retained for Part 1, but the dependency must remain in process and behind the interface.

### Required Runtime Path

```text
Academic Supervisors use case
    -> IStudentLookup
        -> local Students implementation
            -> Students persistence output port
                -> Students persistence adapter
                    -> StudentManagementDbContext
                        -> SQLite
```

This is an in-process call. It does not involve HTTP, serialisation, network latency, or remote failure.

## Task 5: Add Academic Supervisors to `PgrStudentManagement.Adapters.API`

### Objective

Expose the Academic Supervisor assignment use case through REST while retaining the direct Student lookup internally.

### Add API models

Create dedicated request and response types in the API adapter. They are transport models and must not be EF Core entities.

A request may contain:

```text
StudentNumber
StaffNumber
Role
StartDate
ExceptionAuthorised
```

Use the actual requirements represented by the Week 5 implementation.

### Add an API mapper

Map between the API request and the Business input DTO. Map the Business outcome to an API response without exposing Domain or persistence objects directly.

### Add the REST controller

The controller must:

- receive HTTP input;
- map the request to the Business input type;
- invoke the Academic Supervisors input port;
- map the Business outcome to an HTTP response;
- pass the request `CancellationToken`; and
- contain no assignment rules or persistence code.

A reference route is:

```http
POST /api/students/{studentNumber}/academic-supervisors
```

Other route designs are acceptable if they represent ownership and use-case semantics clearly.

### Map Business outcomes

| Business outcome | Possible HTTP response |
|---|---|
| Assignment created | `201 Created` or `200 OK` |
| Student not found | `404 Not Found` |
| Academic Supervisor not found | `404 Not Found` |
| Duplicate assignment | `409 Conflict` |
| Unique role already filled | `409 Conflict` |
| Supervisor not eligible | `422 Unprocessable Content` or a documented `400 Bad Request` |
| Employment type not permitted | `422 Unprocessable Content` or a documented `400 Bad Request` |
| Exception required | `409 Conflict` |

Use one consistent mapping and verify it using the REST client.

### Register the API adapter

Register the Academic Supervisors use case, API mapper, and related dependencies in `PgrStudentManagement.Application`. Ensure the API adapter assembly is loaded by the Application host.

## Task 6: Test the Academic Supervisors REST Operation

### Objective

Verify the REST operation while Student lookup remains a direct call.

### Steps

1. Start the application.
2. Use Postman, an IDE `.http` file, or `curl`.
3. Send a successful Academic Supervisors request.
4. Send a request for an unknown Student.
5. Send a request that triggers at least one Academic Supervisor Business rule.
6. Confirm that the status codes and response bodies match your documented mapping.
7. Use breakpoints, logging, or code tracing to confirm that `IStudentLookup` is handled by the local Students implementation.

### Evidence to retain

Record:

- the HTTP method and route;
- one successful request and response;
- one unsuccessful request and response; and
- the dependency-injection registration for `IStudentLookup`.

## Task 7: Create the Direct-Call Sequence Diagram

### Objective

Document the first implementation before replacing the local lookup.

Create a PlantUML or Mermaid sequence diagram showing:

```text
REST client
    -> Academic Supervisors REST controller
        -> Academic Supervisors input port
            -> Academic Supervisors use case
                -> IStudentLookup
                    -> local Students lookup implementation
                        -> Students persistence port
                            -> Students persistence adapter
                                -> StudentManagementDbContext
                                    -> SQLite
```

The diagram must show:

- the external REST request;
- the Academic Supervisors API adapter;
- the Academic Supervisors Business use case;
- `IStudentLookup`;
- the local Students implementation;
- the database interaction;
- the successful response; and
- an alternative for an unknown Student.

Label the `IStudentLookup` interaction as an in-process call. Keep this diagram for comparison with the HTTP-based interaction in Part 2.

---

# Part 2: Expose Student Lookup and Replace the Direct Call

Part 2 replaces the local adapter for the Academic Supervisors Business-owned `IStudentLookup` output port without changing the Academic Supervisors use case. First, Students exposes the lookup capability through its own Business input port and REST incoming adapter. Then an outgoing HTTP adapter replaces the local lookup adapter.

## Task 8: Define the Students Lookup Use Case in `PgrStudentManagement.Business`

### Objective

Provide a Students Business operation that can be invoked by an incoming REST adapter.

`IStudentLookup` is the consumer-side output port owned by Academic Supervisors Business. It is not itself an HTTP endpoint and does not cross the network. Students therefore needs a separate provider-side input port or use case for the same capability.

### Steps

1. Review the existing Students Business code and reuse an existing Student lookup use case if one already provides the required data.
2. If necessary, add a Students input port such as `IGetStudentSummaryUseCase`.
3. Add or reuse an input DTO containing the Student number.
4. Add or reuse an output DTO containing only the information required by Academic Supervisors, for example:
   - `StudentNumber`; and
   - `DisplayName`.
5. Represent a missing Student explicitly in the Business outcome.
6. Implement the use case through the existing Students persistence output port.
7. Keep HTTP methods, routes, status codes, JSON, and API response types out of Business.

A possible input port is:

```csharp
public interface IGetStudentSummaryUseCase
{
    Task<GetStudentSummaryOutput> ExecuteAsync(
        GetStudentSummaryInput input,
        CancellationToken cancellationToken = default);
}
```

The exact names should follow the conventions already used in the Week 6 Students implementation.

### Checkpoint

The Students lookup use case should be testable without an HTTP controller:

```text
Students input port
    -> Students lookup use case
        -> Students persistence output port
            -> Students persistence adapter
```

## Task 9: Expose Student Lookup in `PgrStudentManagement.Adapters.API`

### Objective

Add an incoming REST adapter for the Students lookup use case.

### Define the REST contract

Use a route owned by Students. A reference design is:

```http
GET /api/students/{studentNumber}/lookup
Accept: application/json
```

A simpler route is acceptable if it does not conflict with another Student operation:

```http
GET /api/students/{studentNumber}
```

A successful representation should contain only the fields required by Academic Supervisors:

```json
{
  "studentNumber": "S100002",
  "displayName": "Jane Smith"
}
```

Expected outcomes:

```text
200 OK          Student found
404 Not Found   Student does not exist
400 Bad Request Invalid Student number, where applicable
```

### Steps

1. Add an adapter-owned response model, such as `StudentLookupResponse`.
2. Add an API mapper from the Students Business output to the response model.
3. Add a controller operation for the selected route.
4. Map the route value to the Business input DTO.
5. Invoke the Students input port.
6. Return `200 OK` with `StudentLookupResponse` when the Student exists.
7. Return `404 Not Found` when the Student does not exist.
8. Pass the request `CancellationToken` to the Business input port.
9. Keep persistence records and EF Core entities out of the response contract.
10. Register the API mapper and any new Students Business components in the Application composition root.

### Required Runtime Path

```text
REST client
    -> Students REST controller
        -> Students input port
            -> Students lookup use case
                -> Students persistence output port
                    -> Students persistence adapter
                        -> SQLite
```

### Checkpoint

Test the Students endpoint independently before changing Academic Supervisors:

```http
GET /api/students/S100002/lookup
```

Also test an unknown Student number. Record the exact route, status codes, and response bodies.

## Task 10: Add the Outgoing HTTP Adapter to `PgrStudentManagement.Adapters.API`

### Objective

Implement an outgoing adapter that allows Academic Supervisors to satisfy `IStudentLookup` through the Students REST endpoint.

A suitable structure is:

```text
PgrStudentManagement.Adapters.API
└── AcademicSupervisors
    └── StudentLookupClient
        ├── HttpStudentLookupAdapter.cs
        └── StudentLookupApiResponse.cs
```

The Week 6 solution uses `PgrStudentManagement.Adapters.API` for HTTP-facing code, so this task places the outgoing adapter there. If the solution has a dedicated outgoing-adapters project, that project is also acceptable. In either case, the HTTP implementation must remain outside Business and Domain.

### Steps

1. Add an adapter-owned transport model that matches the Students JSON response.
2. Create `HttpStudentLookupAdapter` and implement `IStudentLookup`.
3. Inject `HttpClient` through the constructor.
4. Build the request URI from the Student number.
5. URL-encode the Student number where required.
6. Send the request asynchronously and pass the `CancellationToken`.
7. Translate `200 OK` into `StudentSummary`.
8. Translate `404 Not Found` into the existing Student-not-found result, such as `null`.
9. Treat other unsuccessful responses as communication or service failures, not as “Student not found”.
10. Keep Students API response types inside the adapter.

A simplified outline is:

```csharp
public sealed class HttpStudentLookupAdapter : IStudentLookup
{
    private readonly HttpClient _httpClient;

    public HttpStudentLookupAdapter(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<StudentSummary?> GetStudentSummaryAsync(
        string studentNumber,
        CancellationToken cancellationToken = default)
    {
        // Build and send the Students lookup request.
        // Return null only for 404 Not Found.
        // Handle other unsuccessful responses deliberately.
        // Map the successful response to StudentSummary.
    }
}
```

### Checkpoint

The adapter should compile against `IStudentLookup` without requiring any change to the Academic Supervisors use case.

## Task 11: Replace the Local Adapter in `PgrStudentManagement.Application`

### Objective

Use dependency injection to replace the local implementation of `IStudentLookup` with `HttpStudentLookupAdapter`.

### Configure the Students address

Add the base address to:

```text
PgrStudentManagement.Application/appsettings.json
```

or, for a development-only value:

```text
PgrStudentManagement.Application/appsettings.Development.json
```

Example:

```json
{
  "Services": {
    "Students": {
      "BaseUrl": "https://localhost:7001"
    }
  }
}
```

Confirm the actual listening address in:

```text
PgrStudentManagement.Application/Properties/launchSettings.json
```

### Replace the dependency-injection registration

In:

```text
PgrStudentManagement.Application/Program.cs
```

remove or replace the local `IStudentLookup` registration and register the typed HTTP client:

```csharp
var studentsBaseUrl = builder.Configuration[
    "Services:Students:BaseUrl"]
    ?? throw new InvalidOperationException(
        "The Students service base URL is not configured.");

builder.Services.AddHttpClient<
    IStudentLookup,
    HttpStudentLookupAdapter>(client =>
{
    client.BaseAddress = new Uri(studentsBaseUrl);
});
```

### Verify the composition

Confirm that:

- only the intended runtime implementation of `IStudentLookup` is registered;
- Academic Supervisors Business still depends only on `IStudentLookup`;
- no URL appears in Business or Domain;
- `HttpClient` is created through `AddHttpClient`; and
- configuration is read by the Application host.

### Checkpoint

The Academic Supervisors use case should not require source-code changes when the adapter is replaced.

## Task 12: Test the Cross-Module HTTP Interaction

### Objective

Verify the complete flow from the external Academic Supervisors request to the Students lookup endpoint.

### Steps

1. Start the application.
2. Verify the Students lookup endpoint independently.
3. Call the Academic Supervisors REST operation.
4. Use a breakpoint or logging to confirm that `HttpStudentLookupAdapter` is invoked.
5. Confirm that the adapter sends an HTTP request to the Students endpoint.
6. Verify a successful assignment request for a known Student.
7. Test an unknown Student and confirm that the Students `404 Not Found` becomes the existing Academic Supervisors Student-not-found outcome.
8. Temporarily configure an invalid Students base address.
9. Confirm that an unavailable endpoint is not reported as “Student not found”.
10. Restore the correct configuration and rerun the successful request.

### Expected Runtime Path

```text
REST client
    -> Academic Supervisors REST controller
        -> Academic Supervisors input port
            -> Academic Supervisors use case
                -> IStudentLookup
                    -> HttpStudentLookupAdapter
                        -> HTTP GET Students lookup endpoint
                            -> Students REST controller
                                -> Students input port
                                    -> Students lookup use case
                                        -> Students persistence output port
                                            -> Students persistence adapter
                                                -> SQLite
```

### Expected Dependency Structure

```text
Academic Supervisors Business
    -> IStudentLookup

HttpStudentLookupAdapter
    -> IStudentLookup
    -> HttpClient
    -> Students REST contract
```

The runtime interaction now crosses an HTTP boundary, while the source-code dependency of Academic Supervisors Business remains on `IStudentLookup`.

## Task 13: Compare the Direct and HTTP Interactions

### Objective

Make the architectural change explicit and retain evidence for the debrief.

### Update the sequence diagram

Create a second sequence diagram for the HTTP-based interaction. Do not overwrite the direct-call diagram from Part 1.

The HTTP diagram must show:

- the external REST client;
- the Academic Supervisors REST controller;
- the Academic Supervisors input port and use case;
- `IStudentLookup`;
- `HttpStudentLookupAdapter`;
- the HTTP request to the Students endpoint;
- the Students REST controller;
- the Students input port and use case;
- Students persistence; and
- success and Student-not-found alternatives.

### Compare both implementations

Record the differences:

| Direct lookup | HTTP lookup |
|---|---|
| In-process invocation | HTTP request and JSON response |
| Local adapter | `HttpStudentLookupAdapter` |
| No network failure | Endpoint, timeout, and protocol failure possible |
| No serialisation | Request and response serialisation |
| Local runtime path | Crosses the Students REST boundary |

Also record what remains unchanged:

- the Academic Supervisors use case;
- `IStudentLookup`;
- the Academic Supervisors Business rules; and
- the Students ownership of Student information.

### Evidence to retain

Keep:

- both sequence diagrams;
- the documented Students lookup REST contract;
- one successful Students lookup request and response;
- one successful Academic Supervisors request and response;
- one unknown-Student response;
- the original local `IStudentLookup` registration;
- the final HTTP adapter registration; and
- the location of `HttpStudentLookupAdapter`.

---

## 6. Required Completion Criteria

### Academic Supervisors implementation

- [ ] Academic Supervisors has been added to the Week 6 Clean Architecture.
- [ ] The implementation is based on the Week 5 capability and rules.
- [ ] Academic Supervisors relational persistence uses the shared `StudentManagementDbContext` for this laboratory.
- [ ] Eligible roles and supervisory assignments are stored relationally.
- [ ] Academic Supervisors Business owns and depends on the `IStudentLookup` output port.

### Part 1

- [ ] An Academic Supervisors use case is exposed through REST.
- [ ] The Academic Supervisors controller invokes a Business input port or use case.
- [ ] The first implementation supplies `IStudentLookup` through a local, in-process adapter.
- [ ] The Academic Supervisors operation can be invoked with a REST client.
- [ ] A sequence diagram shows the direct Student lookup.

### Part 2

- [ ] Students exposes the lookup capability through a Students Business input port and REST incoming adapter.
- [ ] A dedicated API response model is used.
- [ ] The outgoing `HttpStudentLookupAdapter` implements the Academic Supervisors Business-owned `IStudentLookup` output port.
- [ ] Academic Supervisors Business does not depend on `HttpClient`.
- [ ] The Students base URL is stored in Application configuration.
- [ ] Dependency injection supplies the HTTP adapter.
- [ ] A known Student produces the expected result.
- [ ] An unknown Student is distinct from an unavailable endpoint.
- [ ] The Academic Supervisors use case remains unchanged when the local adapter is replaced.
- [ ] A second sequence diagram shows the HTTP-based Student lookup.
- [ ] The direct-call and HTTP-call diagrams use consistent participant and port names.

## 7. Reflection

Record brief answers:

1. Which classes were added when Academic Supervisors was moved into the Week 6 Clean Architecture?
2. Which parts of the Week 5 Academic Supervisors implementation were reused?
3. Why does Academic Supervisors Business own `IStudentLookup` rather than depend on Students persistence?
4. What does the direct-call sequence diagram show that the HTTP API alone does not show?
5. What changed when the local `IStudentLookup` adapter was replaced by `HttpStudentLookupAdapter`?
6. Which parts of Academic Supervisors Business remained unchanged?
7. Why is an unknown Student different from an unavailable Students endpoint?
8. Does your final implementation contain one application container or two independently deployable service containers?
9. What evidence supports that answer?
10. Why is the shared `StudentManagementDbContext` a limitation for independent service deployment?

## 8. Troubleshooting

### Academic Supervisors classes cannot reference Business or Domain types

Check the project references against the Week 6 Dependency Rule. Outer adapters may reference Business and Domain. Business must not reference adapter projects.

### EF Core cannot create the Academic Supervisors tables

Check:

- the `DbSet` properties;
- entity keys;
- composite keys;
- foreign-key relationships;
- enum conversions;
- the generated migration; and
- the startup and migrations project arguments.

### Seeding fails with a unique-constraint error

Ensure the existence check uses the same identity as the database key. For supervisory assignments, this may include:

```text
StudentNumber
SupervisorStaffNumber
Role
StartDate
```

### Academic Supervisor eligible roles are empty

Ensure the persistence query explicitly loads the eligible-role collection with `Include(...)` where required.

### The Academic Supervisors endpoint cannot be reached

Check:

- controller discovery;
- route attributes;
- API service registration;
- endpoint mapping;
- that the API adapter assembly is loaded by the Application host; and
- the URL used by the REST client.

### The HTTP adapter calls the wrong endpoint or calls itself recursively

Check:

- `Services:Students:BaseUrl`;
- `launchSettings.json`;
- the Students route template;
- the URI built by the adapter; and
- whether the configured base address contains an unexpected path segment.

### Every Student lookup returns `404 Not Found`

Test the Students endpoint independently. Then compare:

- route parameter names;
- URL encoding;
- Student-number case;
- the exact route used by the adapter; and
- the base URL.

### Deserialisation fails

Compare the actual Students JSON response with the adapter-owned response model. Check property names, nullability, types, and JSON naming conventions.

### The request hangs or times out

Confirm that the application can accept the nested request to its own Students endpoint and that the configured address is reachable. Check the console logs for both the outgoing Academic Supervisors request and the incoming Students request.

### `IStudentLookup` has multiple registrations

Remove the local registration when switching to the HTTP adapter. There should be one intended runtime implementation for the final configuration.

## 9. Final Commit

After both parts work:

```bash
git status
git add .
git commit -m "Add Academic Supervisors REST API and HTTP Student lookup"
```

Keep the direct-call sequence diagram and request examples with your laboratory evidence. They will be used when modelling the implementation in Laboratory 2.
