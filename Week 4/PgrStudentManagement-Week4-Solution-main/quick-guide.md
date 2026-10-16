# Quick Guide: Modular Monolith Refactoring & Academic Supervisors Module

## 1. Overview & Architectural Rationale

### Why Move from Layered to Modular Monolith?
In the Week 3 layered architecture, code was organized by technical concern (`Controllers/`, `Data/`, `Models/`, `Services/`). As domain complexity grows, this structure leads to high coupling and unclear domain ownership.

In Week 4, the architecture is refactored into a **Modular Monolith** organized around **Business Capabilities**:
1. **Students Module** (`Modules/Students/`): Maintains student records, enrolments, progression status, and thesis details.
2. **Academic Supervisor Management Module** (`Modules/AcademicSupervisors/`): Manages supervisor profiles, role eligibility, training status, and student supervisory assignments.

### Key Architectural Principles
- **Domain & Data Encapsulation**: Each module owns its domain entities and in-memory storage (`StudentStorage` vs `AcademicSupervisorStorage`). Neither module can directly read or mutate another module's internal collections.
- **Explicit Cross-Module Contracts**: Cross-module communication (e.g. validating a student during assignment creation) must occur solely via an explicit public contract (`IStudentLookup` / `StudentSummary`), not by exposing internal entity models or data stores.
- **Single Process / Shared Deployment**: The system remains a single ASP.NET Core MVC web application with in-memory persistence.
- **Standard View Discovery (Approach 4)**: Razor views remain under the root `Views/` directory (`Views/Students/`, `Views/AcademicSupervisors/`, `Views/Home/`, `Views/Shared/`) to preserve standard MVC view-location conventions without needing custom Razor view engines or area routing.

---

## 2. Target Project Structure

```text
PgrStudentManagement.Web/
├── Modules/
│   ├── Students/
│   │   ├── Contracts/
│   │   │   ├── IStudentLookup.cs
│   │   │   └── StudentSummary.cs
│   │   ├── Controllers/
│   │   │   └── StudentController.cs
│   │   ├── Data/
│   │   │   └── StudentStorage.cs
│   │   ├── Models/
│   │   │   ├── Status.cs
│   │   │   └── Student.cs
│   │   └── Services/
│   │       ├── OperationResult.cs
│   │       ├── OperationResultOfT.cs
│   │       └── StudentService.cs
│   │
│   └── AcademicSupervisors/
│       ├── Controllers/
│       │   └── AcademicSupervisorsController.cs
│       ├── Data/
│       │   └── AcademicSupervisorStorage.cs
│       ├── Models/
│       │   ├── AcademicSupervisor.cs
│       │   ├── AssignmentOutcome.cs
│       │   ├── SupervisorEmploymentType.cs
│       │   ├── SupervisoryAssignment.cs
│       │   ├── SupervisoryRole.cs
│       │   └── SupervisoryTeamAssessment.cs
│       └── Services/
│           └── AcademicSupervisorService.cs
│
├── Views/
│   ├── Home/
│   ├── Shared/
│   │   ├── _Layout.cshtml  (updated with Supervisor navigation)
│   │   └── ...
│   ├── Students/
│   └── AcademicSupervisors/
│       ├── Index.cshtml
│       ├── Details.cshtml
│       ├── Create.cshtml
│       ├── Assign.cshtml
│       ├── SupervisoryTeam.cshtml
│       └── SupervisorNotFound.cshtml
├── Program.cs
└── PgrStudentManagement.Web.csproj
```

---

## 3. Step-by-Step Implementation Plan

### Step 1: Reorganize Existing Student Code into `Modules/Students/`
- **Actions**:
  - Move `Controllers/StudentController.cs` → `Modules/Students/Controllers/`
  - Move `Data/StudentStorage.cs` → `Modules/Students/Data/`
  - Move `Models/Student.cs` & `Models/Status.cs` → `Modules/Students/Models/`
  - Move `Services/StudentService.cs`, `Services/OperationResult.cs`, `Services/OperationResultOfT.cs` → `Modules/Students/Services/`
  - Update namespaces to `PgrStudentManagement.Web.Modules.Students.*`
  - Update `using` directives across moved files, `Program.cs`, and `Views/_ViewImports.cshtml`.
- **Validation**: Build and run tests / manually verify all existing student use cases (W2-UC01–W2-UC05, W3-UC01–W3-UC03).

---

### Step 2: Define and Implement the Student Lookup Contract
- **Rationale**: The Academic Supervisors module needs to verify a student exists and display basic details (number and full name), but should not have access to full student entity internals or `StudentStorage`.
- **Actions**:
  - Create `Modules/Students/Contracts/StudentSummary.cs`:
    ```csharp
    namespace PgrStudentManagement.Web.Modules.Students.Contracts;
    public record StudentSummary(string StudentNumber, string DisplayName);
    ```
  - Create `Modules/Students/Contracts/IStudentLookup.cs`:
    ```csharp
    namespace PgrStudentManagement.Web.Modules.Students.Contracts;
    public interface IStudentLookup
    {
        StudentSummary? GetStudentSummary(string studentNumber);
    }
    ```
  - Have `StudentService` implement `IStudentLookup`.
  - Register `IStudentLookup` in `Program.cs` pointing to the scoped `StudentService`.

---

### Step 3: Create Domain Types for Academic Supervisors Module
- **Location**: `Modules/AcademicSupervisors/Models/`
- **Domain Types**:
  1. `SupervisorEmploymentType` (enum: `InstitutionEmployed`, `External`).
  2. `SupervisoryRole` (enum: `DirectorOfStudies`, `FirstSupervisor`, `SecondSupervisor`, `Adviser`).
  3. `AcademicSupervisor`:
     - Properties: `StaffNumber`, `FirstName`, `LastName`, `EmailAddress`, `EmploymentType`, `TrainingCompleted`, `EligibleRoles` (HashSet/List of `SupervisoryRole`), `FullName` helper.
  4. `SupervisoryAssignment`:
     - Properties: `StudentNumber`, `SupervisorStaffNumber`, `Role`, `StartDate`, `EndDate`, `ExceptionAuthorised`.
     - Derived property: `bool Current => EndDate is null;`
  5. `AssignmentOutcome` (enum/result type) for explicit service return codes.
  6. `SupervisoryTeamAssessment` (record: `bool IsComplete`, `IReadOnlyList<string> Issues`).

---

### Step 4: Create `AcademicSupervisorStorage`
- **Location**: `Modules/AcademicSupervisors/Data/AcademicSupervisorStorage.cs`
- **Rationale**: In-memory repository owning collections for supervisors and assignments for the lifetime of the process.
- **Responsibilities**:
  - `_academicSupervisors` (`List<AcademicSupervisor>`) with initial seed data.
  - `_supervisoryAssignments` (`List<SupervisoryAssignment>`) with initial seed data.
  - CRUD operations: `GetAcademicSupervisors()`, `GetAcademicSupervisor(string staffNumber)`, `AddAcademicSupervisor(...)`, `StaffNumberExists(string staffNumber)`, `GetAssignmentsForStudent(string studentNumber)`, `GetAssignmentsForSupervisor(string staffNumber)`, `AddSupervisoryAssignment(...)`, `UpdateSupervisoryAssignment(...)`.

---

### Step 5: Create `AcademicSupervisorService`
- **Location**: `Modules/AcademicSupervisors/Services/AcademicSupervisorService.cs`
- **Dependencies**: Inject `AcademicSupervisorStorage` and `IStudentLookup`.
- **Core Business Logic**:
  - **W4-UC01 (Create Supervisor)**: Validate duplicate `StaffNumber`. Set initial eligibility based on training/employment.
  - **W4-UC02 (List Supervisors)**: Return ordered list by `LastName`, `FirstName`, `StaffNumber`.
  - **W4-UC03 (Get Details)**: Retrieve by `StaffNumber`.
  - **W4-UC04 (Assign Supervisor)**: Enforce assignment rules:
    - Verify student exists via `IStudentLookup`.
    - Verify supervisor exists in storage.
    - Verify supervisor is eligible for selected role.
    - Disallow external supervisor as `DirectorOfStudies` or `FirstSupervisor` (BR13).
    - Disallow duplicate active assignment for the same student, supervisor, and role (BR15).
    - Disallow duplicate active `DirectorOfStudies` or `FirstSupervisor` for the student (BR14).
    - Limit active supervisors to 3 unless `ExceptionAuthorised` is true (BR02/BR03).
    - Record assignment.
  - **W4-UC05 (Supervisory Team & Assessment)**:
    - Retrieve current and historical assignments for a student.
    - Evaluate team completeness (presence of DoS & FirstSupervisor, team size 2–3).

---

### Step 6: Register Services in `Program.cs`
- **Direct Registration**:
  ```csharp
  // Students Module
  builder.Services.AddSingleton<StudentStorage>();
  builder.Services.AddScoped<StudentService>();
  builder.Services.AddScoped<IStudentLookup>(sp => sp.GetRequiredService<StudentService>());

  // Academic Supervisors Module
  builder.Services.AddSingleton<AcademicSupervisorStorage>();
  builder.Services.AddScoped<AcademicSupervisorService>();
  ```

---

### Step 7: Create `AcademicSupervisorsController` and Views
- **Controller**: `Modules/AcademicSupervisors/Controllers/AcademicSupervisorsController.cs`
  - Action methods:
    - `Index()` -> Views/AcademicSupervisors/Index.cshtml (List supervisors)
    - `Details(string staffNumber)` -> Details view or SupervisorNotFound
    - `Create()` [HttpGet / HttpPost] -> Create supervisor form
    - `SupervisoryTeam(string studentNumber)` -> View student team & assessment
    - `Assign(string studentNumber)` [HttpGet / HttpPost] -> Assign supervisor form
- **Views** (`Views/AcademicSupervisors/`):
  - `Index.cshtml`, `Details.cshtml`, `Create.cshtml`, `Assign.cshtml`, `SupervisoryTeam.cshtml`, `SupervisorNotFound.cshtml`.
- **Navigation & Links**:
  - Update `Views/Shared/_Layout.cshtml` to add "Academic Supervisors" in top navigation bar.
  - Update student views (e.g. student details/thesis) to link directly to `SupervisoryTeam(studentNumber)`.

---

### Step 8: Verification & Module Boundary Audit
- Build project (`dotnet build`).
- Verify all 8 previous student use cases (`W2-UC01` to `W3-UC03`).
- Verify all 5 new core supervisor use cases (`W4-UC01` to `W4-UC05`).
- Ensure no direct cross-boundary leaks (e.g. `AcademicSupervisors` accessing `StudentStorage` or `StudentController` accessing `AcademicSupervisorStorage`).

---

## 4. Summary of Design Decisions

| Decision Area | Selected Approach | Rationale |
|---|---|---|
| **View Location** | Application-level `Views/` (Approach 4) | Keeps standard ASP.NET Core MVC routing & view discovery intact without requiring custom view location expanders or areas. |
| **Cross-Module Communication** | `IStudentLookup` returning `StudentSummary` record | Prevents supervisor module from accessing internal student entity models or data storage, maintaining strict encapsulation. |
| **Current vs Historical Assignments** | Derived property `Current => EndDate is null` | Simplifies state management and avoids data inconsistencies between boolean flags and end dates. |
| **Role Limits & Rules** | Checked in `AcademicSupervisorService` | Ensures business rules (BR01–BR15) are strictly enforced in domain/service logic rather than controllers or views. |
