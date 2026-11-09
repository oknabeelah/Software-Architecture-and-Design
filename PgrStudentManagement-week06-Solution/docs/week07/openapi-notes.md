# OpenAPI Notes

## What OpenAPI is for

OpenAPI is a machine-readable description of the HTTP API. It records the
available routes, HTTP methods, parameters, request bodies, response schemas,
and status codes. Consumers can use the document to understand and test the
API, generate client code, and check that the implementation remains
consistent with the documented contract.

This solution uses the built-in ASP.NET Core OpenAPI support for .NET 10.
`AddOpenApi()` registers document generation and `MapOpenApi()` exposes the
generated document at `/openapi/v1.json` in Development.

## Documented operations

### Students lookup

- **Owning capability:** Students
- **HTTP method:** `GET`
- **Route:** `/api/students/{studentNumber}/lookup`
- **Route parameters:** `studentNumber`
- **Query parameters:** None
- **Request body:** N/A
- **Successful response:** `200 OK` with `StudentLookupResponse`
- **Not found response:** `404 Not Found` when the Student does not exist

The response contains only `StudentNumber` and `DisplayName`, which are the
fields required by the Academic Supervisors consumer. Internal Student
properties and persistence details are not exposed across the HTTP boundary.

### Academic Supervisors assignment

- **Owning capability:** Academic Supervisors
- **HTTP method:** `POST`
- **Route:** `/api/students/{studentNumber}/academic-supervisors`
- **Route parameters:** `studentNumber`
- **Query parameters:** None
- **Request body:** `AssignAcademicSupervisorRequest`
- **Successful response:** `201 Created` with `AssignAcademicSupervisorResponse`
- **Not found response:** `404 Not Found` for an unknown Student or Academic Supervisor
- **Conflict responses:** `409 Conflict` for duplicate assignments, filled roles,
  maximum-supervisor limits, or missing exception authorisation
- **Validation responses:** `400 Bad Request` for invalid input and
  `422 Unprocessable Entity` for an ineligible or disallowed supervisor

## HTTP contract and `IStudentLookup`

`IStudentLookup` is an Academic Supervisors Business output port. It returns a
Business `StudentSummary` and does not depend on HTTP, JSON, or ASP.NET Core.
`HttpStudentLookupAdapter` implements that port and translates it to the
Students HTTP contract.

The Students endpoint returns `200 OK` with `StudentLookupResponse`. The
adapter maps that response to `StudentSummary` before returning it to the
Academic Supervisors use case. This keeps the Business layer independent from
the transport format.

When the Students endpoint returns `404 Not Found`, the adapter returns the
existing not-found result (`null`) so the Academic Supervisors use case can
produce its documented `StudentNotFound` outcome. Other unsuccessful responses,
including an unavailable or misconfigured Students service, raise
`HttpRequestException`; they are not treated as an unknown Student.
