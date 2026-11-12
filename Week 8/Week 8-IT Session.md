10/11//2026


## Students Authentication Boundary

| Capability | Adapter | Route or action | Current access | New Access |
|---|---|---|---|
| List Students | MVC | `StudentsController.Index` | Anonymous | Anonymous | 
| View Student details | MVC | `StudentsController.Details` | Anonymous | Student, Tutor |
| Create Student | MVC | Create action | Anonymous | Tutor |
| Update status | MVC | Status action | Anonymous | Tutor |
| Update expected submission date | MVC | Submission-date action | Anonymous | Student |
| Record submission | MVC | Submission action | Anonymous | Student |
| View Enrolments | MVC | Enrolments action | Anonymous | Student, Tutor |
| Student lookup | REST | `/api/students/{studentNumber}/lookup` | Anonymous | Student, Tutor |

If `HttpStudentLookupAdapter` calls a now-protected Students endpoint, an unauthenticated internal `HttpClient` call will receive `401 Unauthorized`.

For the core Laboratory 1 completion, choose and document one of these approaches:

**Recommended approach**
Keep the Students lookup endpoint anonymous temporarily:

```csharp
[AllowAnonymous]
[HttpGet("{studentNumber}/lookup")]
public async Task<IActionResult> Lookup(...)
```

Document that this is a temporary exception used to preserve the Week 7 module interaction while user authentication is introduced. Revisit service-to-service authentication as an extension.

### Alternative if the solution does not use the HTTP adapter

If Academic Supervisors still uses a local `IStudentLookup` adapter, the internal call does not cross the protected REST endpoint and no extra change is required.
