## Week 8: Manual Verification for REST Endpoints

### Purpose

Use this guide with:

- **Week 8 Laboratory 1** to verify authentication for protected Students REST endpoints; and
- **Week 8 Laboratory 2** to verify Role-Based Access Control outcomes for different authenticated identities.

The core teaching application uses cookie authentication. Visual Studio Code must retain the authentication cookie returned by the login request and resend it with later API requests.

This guide uses example routes, identities, passwords, and ports. Replace them with the values configured in the current solution.

### Security Notice

Use this guide only with:

- the local development application;
- synthetic development identities;
- synthetic Student data; and
- a private or controlled teaching environment.

Do not record or commit:

- authentication cookies;
- genuine usernames or passwords;
- bearer tokens;
- client secrets; or
- real Student or staff data.

## 1. Start the Application

From the solution directory, run:

```bash
dotnet run --project PgrStudentManagement.Application
```

Note the HTTP base address displayed in the terminal, for example:

```text
http://localhost:5000
```

Use the exact same scheme, host, and port for sign-in and all later requests.

## 2. Create the Verification Record

Record results in:

```text
docs/week08/rbac-verification-evidence.md
```

For each request, record:

```text
Test ID:
Identity:
Role:
HTTP method:
Route:
Expected status:
Actual status:
Relevant response:
Result matches the authentication or permission design: Yes or No
Notes:
```

Do not paste authentication-cookie values into the evidence file.

## 3. Install and Configure REST Client

### 3.1 Install the Extension

In Visual Studio Code, install **REST Client** by Huachao Mao:

```text
Extensions
  -> search for REST Client
    -> install REST Client by Huachao Mao
```

The extension sends requests from `.http` files and displays responses inside Visual Studio Code.

### 3.2 Enable Cookie Persistence

Open the Visual Studio Code settings JSON:

```text
Command Palette
  -> Preferences: Open User Settings (JSON)
```

Add:

```json
{
  "rest-client.rememberCookiesForSubsequentRequests": true
}
```

If the settings file already contains other properties, add the property without removing the existing settings.

REST Client associates cookies with the scheme, host, and port. A cookie received from:

```text
http://localhost:5000
```

is not sent to:

```text
http://localhost:5001
https://localhost:5000
```

## 4. Create the HTTP Request Files

Store the request files under:

```text
PgrStudentManagement.Application/HttpRequests/
```

Create:

```text
PgrStudentManagement.Application/
└── HttpRequests/
    ├── anonymous-students.http
    ├── authenticated-students.http
    ├── college-administrator.http
    ├── pgr-tutor.http
    ├── academic-supervisor.http
    ├── student.http
    └── student-lookup-exception.http
```

Use one file for each identity or role. Each authenticated file must contain the sign-in request before the REST requests that depend on the resulting authentication cookie.

## 5. Laboratory 1 Verification

Laboratory 1 verifies authentication only.

### 5.1 Create the Anonymous Request File

**File path:** `PgrStudentManagement.Application/HttpRequests/anonymous-students.http`

```http
@baseUrl = http://localhost:5000

### Anonymous request to a protected Students operation
GET {{baseUrl}}/api/students
Accept: application/json
```

Replace the base address with the HTTP address displayed by the application.

Select **Send Request** above the request.

Expected result:

```text
401 Unauthorized
```

If the request returns `200 OK`, check whether:

- the controller or action has `[Authorize]`;
- authentication and authorisation middleware are enabled;
- the request targeted the intended Application instance; or
- the operation is deliberately marked `[AllowAnonymous]`.

### 5.2 Create the Authenticated Request File

**File path:** `PgrStudentManagement.Application/HttpRequests/authenticated-students.http`

```http
@baseUrl = http://localhost:5000

### Sign in as the development user
POST {{baseUrl}}/account/login
Content-Type: application/x-www-form-urlencoded

Username=demo.user&Password=ChangeThis-Demo-Password

### Authenticated request to a protected Students operation
GET {{baseUrl}}/api/students
Accept: application/json
```

Run the requests in this order:

1. **Sign in as the development user**
2. **Authenticated request to a protected Students operation**

The login response should contain a `Set-Cookie` header. REST Client stores the cookie and sends it with the later request in the same file and to the same base address.

The login response may be a redirect or contain no JSON body. The important result is that the response creates the authentication cookie used by the later REST requests.

Expected result for the second request:

```text
200 OK
```

Use the actual successful status implemented by the operation where it differs from `200 OK`.

## 6. Laboratory 2 Verification

Laboratory 2 verifies role and permission outcomes. Run the sign-in request at the top of each role file before running the protected REST requests in that file.

### 6.1 College Administrator

**File path:** `PgrStudentManagement.Application/HttpRequests/college-administrator.http`

```http
@baseUrl = http://localhost:5000

### Sign in as College Administrator
POST {{baseUrl}}/account/login
Content-Type: application/x-www-form-urlencoded

Username=college.admin&Password=Demo-College-Password

### List Students as College Administrator
GET {{baseUrl}}/api/students
Accept: application/json

### College Administrator-only operation
# Replace this example with the actual method, route, and body.
POST {{baseUrl}}/api/students
Content-Type: application/json

{
  "studentNumber": "S100010",
  "givenName": "Development",
  "familyName": "Student"
}
```

Expected outcomes:

```text
List Students
  -> success

College Administrator-only operation
  -> success
```

Use the actual request schema implemented by the current API. Do not simulate a request body that the application does not support.

### 6.2 PgR Tutor

**File path:** `PgrStudentManagement.Application/HttpRequests/pgr-tutor.http`

```http
@baseUrl = http://localhost:5000

### Sign in as PgR Tutor
POST {{baseUrl}}/account/login
Content-Type: application/x-www-form-urlencoded

Username=pgr.tutor&Password=Demo-Tutor-Password

### List Students as PgR Tutor
GET {{baseUrl}}/api/students
Accept: application/json

### College Administrator-only operation as PgR Tutor
# Replace this example with the actual protected operation.
POST {{baseUrl}}/api/students
Content-Type: application/json

{
  "studentNumber": "S100011",
  "givenName": "Forbidden",
  "familyName": "Example"
}
```

Expected outcomes:

```text
List Students
  -> success

College Administrator-only operation
  -> 403 Forbidden
```

A `403 Forbidden` response means:

- authentication succeeded;
- ASP.NET Core established an identity;
- the identity did not satisfy the operation's authorisation policy; and
- access control prevented the operation from executing.

If the response is `401 Unauthorized`, run the sign-in request at the top of `pgr-tutor.http` again and confirm that the response contains `Set-Cookie`.

### 6.3 Academic Supervisor

**File path:** `PgrStudentManagement.Application/HttpRequests/academic-supervisor.http`

```http
@baseUrl = http://localhost:5000

### Sign in as Academic Supervisor
POST {{baseUrl}}/account/login
Content-Type: application/x-www-form-urlencoded

Username=academic.supervisor&Password=Demo-Supervisor-Password

### Attempt to list all Students as Academic Supervisor
GET {{baseUrl}}/api/students
Accept: application/json
```

Expected result:

```text
403 Forbidden
```

### 6.4 Student

**File path:** `PgrStudentManagement.Application/HttpRequests/student.http`

```http
@baseUrl = http://localhost:5000

### Sign in as Student
POST {{baseUrl}}/account/login
Content-Type: application/x-www-form-urlencoded

Username=student.user&Password=Demo-Student-Password

### Attempt to list all Students as Student
GET {{baseUrl}}/api/students
Accept: application/json
```

Expected result:

```text
403 Forbidden
```

### 6.5 Compare the Results with the Permission Matrix

| Request file | Operation | Expected result |
|---|---|---:|
| `anonymous-students.http` | Protected Students GET | `401` |
| `college-administrator.http` | List Students | Success |
| `college-administrator.http` | Create Student | Success |
| `pgr-tutor.http` | List Students | Success |
| `pgr-tutor.http` | Create Student | `403` |
| `academic-supervisor.http` | List all Students | `403` |
| `student.http` | List all Students | `403` |

Compare the results with:

```text
docs/week08/permission-matrix.md
```

If a result differs from the matrix, determine whether:

- the matrix is wrong;
- the named policy is wrong;
- the wrong policy is applied to the route;
- the identity has the wrong role claim;
- the sign-in request in the selected role file was not run first; or
- the request targeted a different operation.

## 7. Supply a Cookie for One Temporary Request

Use this only if automatic cookie persistence is unclear or when testing one isolated request.

Keep the temporary request in the relevant role file.

1. Run the relevant login request.
2. Inspect the response's `Set-Cookie` header.
3. Identify the authentication cookie name and value before the first semicolon.
4. Add a prompted variable to the temporary request:

```http
# @prompt authCookie

### Temporary authenticated request
GET {{baseUrl}}/api/students
Accept: application/json
Cookie: PgrStudentManagement.Auth={{authCookie}}
```

When Visual Studio Code prompts for `authCookie`, paste only the cookie value.

The cookie name must match the name configured by the application. It may differ from:

```text
PgrStudentManagement.Auth
```

Remove the `Cookie` header after the request. Never commit an active cookie value to the repository.

## 8. Verify the Student Lookup Exception

Create a separate request file because the lookup exception does not depend on an authenticated cookie.

**File path:** `PgrStudentManagement.Application/HttpRequests/student-lookup-exception.http`

```http
@baseUrl = http://localhost:5000
@studentNumber = S100001

### Anonymous Student lookup integration request
GET {{baseUrl}}/api/students/{{studentNumber}}/lookup
Accept: application/json
```

Expected results may include:

```text
200 OK         Student exists
404 Not Found  Student does not exist
```

The result should not be `401 Unauthorized` if the action is deliberately marked `[AllowAnonymous]`.

Record the exception in:

```text
docs/week08/rbac-implementation-notes.md
```

Suggested wording:

```text
The Student lookup endpoint remains anonymous as a temporary
module-integration exception. End-user RBAC roles do not represent
an Academic Supervisors service identity. Service-to-service
authentication and authorisation are deferred.
```

## 9. Request-File Summary

Double-check your own files against this table and update it as appropriate:

| File | Purpose | Depends on an authentication cookie |
|---|---|---:|
| `PgrStudentManagement.Application/HttpRequests/anonymous-students.http` | Verify anonymous `401` behaviour | No |
| `PgrStudentManagement.Application/HttpRequests/authenticated-students.http` | Verify authenticated access in Laboratory 1 | Yes |
| `PgrStudentManagement.Application/HttpRequests/college-administrator.http` | Verify College Administrator permissions | Yes |
| `PgrStudentManagement.Application/HttpRequests/pgr-tutor.http` | Verify PgR Tutor permissions | Yes |
| `PgrStudentManagement.Application/HttpRequests/academic-supervisor.http` | Verify Academic Supervisor permissions | Yes |
| `PgrStudentManagement.Application/HttpRequests/student.http` | Verify Student permissions | Yes |
| `PgrStudentManagement.Application/HttpRequests/student-lookup-exception.http` | Verify the anonymous module-integration exception | No |

Every authenticated file must contain the sign-in request before the protected requests that depend on that identity.

## 10. Troubleshooting

### The Login Request Returns HTML Rather Than JSON

This can be expected from the login endpoint. Check whether the response sets the authentication cookie and whether REST Client retains the cookie.

### A Protected API Request Returns 302 Found

Check that:

- the endpoint is recognised as an API endpoint;
- the controller has `[ApiController]` where appropriate;
- the request targeted the intended REST route; and
- the ASP.NET Core cookie-authentication configuration has not overridden the API behaviour.

### Every Authenticated Request Returns 401 Unauthorized

Check that:

- the sign-in request at the top of the selected file succeeded;
- the response set the cookie;
- REST Client retained the cookie;
- the later request uses the same HTTP scheme, host, and port;
- the cookie has not expired; and
- the application was not restarted after the cookie was issued.

### An Authenticated but Disallowed Role Receives 200 OK

Check that:

- the action uses the intended named policy;
- the policy is registered in `PgrStudentManagement.Application/Program.cs`;
- the identity contains the expected role claim;
- the action uses the named policy rather than only `[Authorize]`; and
- the request targeted the intended route.

### An Allowed Role Receives 403 Forbidden

Check that:

- the role claim exactly matches the role constant;
- the policy includes the intended role;
- the cookie was created after the role configuration changed; and
- the policy name matches the policy applied to the action.

### A Different User Still Appears After Sign-In

Run the sign-in request at the top of the intended role file again, then rerun the protected request in that same file.
