03/11/2026

Architectures so far
Monolith -> Modular Monolith -> Layered -> Services

In this laboratory, you will use the Week 6 solution as your starting point and the Week 5 Academic Supervisors implementation as the functional reference.

You will complete the work in two parts:
- Add Academic Supervisors to the Week 6 Clean Architecture and expose its use cases through REST. At this stage, Academic Supervisors must continue to obtain Student information through a direct, in-process call to IStudentLookup.
- Expose the Students implementation of IStudentLookup through REST, then replace the direct Academic Supervisors lookup with an HTTP adapter.

The aim is to observe what changes when an existing module interaction becomes a service interaction. Clean Architecture should remain visible on both sides of the HTTP boundary.

**Important Architectural Constraints**
Your implementation must preserve the following rules:
- Students owns Student information and the Student lookup capability.
- Academic Supervisors consumes Student information through IStudentLookup.
- Academic Supervisors Business code must not query Students persistence directly.
- REST controllers are incoming adapters. They must not contain Business rules or access DbContext directly.
- HTTP-specific code belongs in an outer adapter, not in Business or Domain.
- The first version of the Academic Supervisors module may continue to use the relational database and StudentManagementDbContext pattern shown in the Week 5 reference solution

In Week 5 solution, Use the Academic Supervisors module to identify:
- the Domain concepts 
- the Academic Supervisor entities and enums 
- supervisory assignment rules
- controller behaviour
- relational table requirements
- data-seeding values
- Student lookup through IStudentLookup  
- the EF Core implementation using StudentManagementDbContext.

Use the Week 6 solution to determine:  
- project responsibilities
- dependency direction  
- input-port and output-port placement  
- adapter organisation
- dependency-injection registration
- API adapter conventions  
- where the new Academic Supervisors classes should be placed.

Organize the Academic Supervisors capability across the existing Week 6 projects.

PgrStudentManagement.Domain  
	Academic Supervisor and Supervisory Assignment concepts
	`Modules/AcademicSupervisor/Models`

PgrStudentManagement.Business  
	Academic Supervisor use cases `view supervisors, view supervisor details, add supervisor, add assignment, edit supervisor details, edit assignment details, view supervisory team`
	input ports `...Usecase/UsecaseInput`
	Business DTOs and outcomes  `...Usecase/AcademicSupervisorOutput`
	IStudentLookup  `...Usecase/I...Usecase` ? or gateway
	persistence output ports  `Gateway/IAcademicSupervisor`

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

## Week 7 Task 6 testing notes

The Academic Supervisors assignment endpoint is:

```text
POST http://localhost:5066/api/students/{studentNumber}/academic-supervisors
```

The reusable requests are in [AcademicSupervisors.http](../PgrStudentManagement-week06-Solution/PgrStudentManagement.Application/AcademicSupervisors.http). Start the application from the solution directory:

```bash
dotnet run --project PgrStudentManagement.Application --launch-profile http
```

Test the successful request with curl:

```bash
curl -i -X POST http://localhost:5066/api/students/S100003/academic-supervisors \
  -H "Content-Type: application/json" \
  -d "{\"supervisorStaffNumber\":\"AS1003\",\"role\":\"Adviser\",\"startDate\":\"2026-11-09\",\"exceptionAuthorised\":false}"
```

Expected result: `201 Created` with an `AssignAcademicSupervisorResponse` whose `succeeded` value is `true` and whose `outcome` is `Created`.

Test an unknown Student:

```bash
curl -i -X POST http://localhost:5066/api/students/S999999/academic-supervisors \
  -H "Content-Type: application/json" \
  -d "{\"supervisorStaffNumber\":\"AS1003\",\"role\":\"Adviser\",\"startDate\":\"2026-11-09\",\"exceptionAuthorised\":false}"
```

Expected result: `404 Not Found` with outcome `StudentNotFound`.

Test a Business-rule failure using the seeded Director of Studies assignment:

```bash
curl -i -X POST http://localhost:5066/api/students/S100001/academic-supervisors \
  -H "Content-Type: application/json" \
  -d "{\"supervisorStaffNumber\":\"AS1002\",\"role\":\"DirectorOfStudies\",\"startDate\":\"2026-11-09\",\"exceptionAuthorised\":false}"
```

Expected result: `409 Conflict` with outcome `RoleAlreadyFilled`.

Observed with curl on 09/11/2026:

```text
Known Student assignment: 201 Created
{"succeeded":true,"outcome":"Created","assignmentId":"assignment-185a7"}

Unknown Student: 404 Not Found
{"succeeded":false,"outcome":"StudentNotFound","assignmentId":null}

Filled Director of Studies role: 409 Conflict
{"succeeded":false,"outcome":"RoleAlreadyFilled","assignmentId":null}
```

The direct lookup registration is in `PgrStudentManagement.Application/Program.cs`:

```csharp
builder.Services.AddScoped<IStudentLookup, LocalStudentLookupAdapter>();
```

The runtime path is:

```text
Academic Supervisors controller
-> Academic Supervisor assignment use case
-> IStudentLookup
-> LocalStudentLookupAdapter
-> ViewStudentSummaryUseCase
-> EfStudentGateway
-> StudentManagementDbContext
-> SQLite
```

No HTTP call is made for Student lookup in Task 6.
