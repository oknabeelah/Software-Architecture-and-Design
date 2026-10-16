# Week 4 Lab 2: Reverse-Engineered Modular Monolith UML Models

This directory contains the PlantUML (`.puml`) diagrams and documentation reverse-engineered from the Week 4 Modular Monolith implementation.

---

## 1. Table of Generated Diagrams

| Diagram File | Description | Related Tasks |
|---|---|---|
| [`domain-model.puml`](file:///Users/martincooper/Documents/learning%20materials/sad-2026/PgrStudentManagement-Week4-Starter/Diagrams/domain-model.puml) | Domain-model class diagram capturing domain entities, value concepts, derived attributes, and business rules grouped by module. | Tasks 1–4 |
| [`module-boundaries.puml`](file:///Users/martincooper/Documents/learning%20materials/sad-2026/PgrStudentManagement-Week4-Starter/Diagrams/module-boundaries.puml) | High-level module boundary diagram illustrating explicit cross-module contract dependencies and data encapsulation. | Task 7 |
| [`implementation-model.puml`](file:///Users/martincooper/Documents/learning%20materials/sad-2026/PgrStudentManagement-Week4-Starter/Diagrams/implementation-model.puml) | Structural implementation class diagram showing controllers, services, storage classes, public contracts, outcomes, view models, and Razor views. | Tasks 5–9 |
| [`w4-uc01-create-academic-supervisor.puml`](file:///Users/martincooper/Documents/learning%20materials/sad-2026/PgrStudentManagement-Week4-Starter/Diagrams/w4-uc01-create-academic-supervisor.puml) | Sequence diagram for **W4-UC01: Create Academic Supervisor** (including Main Success and Alternative Flow A1: Duplicate Staff Number). | Task 10 |
| [`w4-uc02-list-academic-supervisors.puml`](file:///Users/martincooper/Documents/learning%20materials/sad-2026/PgrStudentManagement-Week4-Starter/Diagrams/w4-uc02-list-academic-supervisors.puml) | Sequence diagram for **W4-UC02: List Academic Supervisors** (including Main Success and Alternative Flow A1: No Academic Supervisors Found). | Task 11 |
| [`w4-uc03-view-academic-supervisor-details.puml`](file:///Users/martincooper/Documents/learning%20materials/sad-2026/PgrStudentManagement-Week4-Starter/Diagrams/w4-uc03-view-academic-supervisor-details.puml) | Sequence diagram for **W4-UC03: View Academic Supervisor Details** (including Main Success and Alternative Flow A1: Academic Supervisor Not Found). | Task 12 |
| [`w4-uc04-assign-academic-supervisor.puml`](file:///Users/martincooper/Documents/learning%20materials/sad-2026/PgrStudentManagement-Week4-Starter/Diagrams/w4-uc04-assign-academic-supervisor.puml) | Sequence diagram for **W4-UC04: Assign Academic Supervisor to Student** (including cross-module `IStudentLookup` contract, business rule validations, and Alternative Flow A1: Assignment Already Exists). | Task 13 |
| [`w4-uc05-view-student-supervisory-team.puml`](file:///Users/martincooper/Documents/learning%20materials/sad-2026/PgrStudentManagement-Week4-Starter/Diagrams/w4-uc05-view-student-supervisory-team.puml) | Sequence diagram for **W4-UC05: View Student Supervisory Team** (including student confirmation, assignment retrieval, completeness assessment, and Alternative Flow A1: No Supervisory Assignments Found). | Task 14 |
| [`extension-thesis-assessment-module.puml`](file:///Users/martincooper/Documents/learning%20materials/sad-2026/PgrStudentManagement-Week4-Starter/Diagrams/extension-thesis-assessment-module.puml) | Candidate module domain model for **Thesis Assessment & Examination**. | Extension Tasks 1–5 |

---

## 2. Module Boundary Analysis (Task 1 & Task 7)

| Module | Business Capability | Owned Domain Concepts | Implementation Classes | Razor Views | Exposed Contracts | Consumed Contracts |
|---|---|---|---|---|---|---|
| **Students Module** | Student records, study modes, enrolments, progression status, and thesis details. | `Student`, `Status` | `StudentController`, `StudentService`, `StudentStorage`, `OperationResult`, `OperationResult<T>` | `Views/Student/*.cshtml` (9 views) | `IStudentLookup` (`StudentSummary`) | None |
| **Academic Supervisors Module** | Supervisor records, role eligibility, training status, and student supervisory arrangements. | `AcademicSupervisor`, `SupervisorEmploymentType`, `SupervisoryRole`, `SupervisoryAssignment`, `AssignmentOutcome`, `SupervisoryTeamAssessment` | `AcademicSupervisorsController`, `AcademicSupervisorService`, `AcademicSupervisorStorage`, ViewModels | `Views/AcademicSupervisors/*.cshtml` (6 views) | None (currently internal consumer) | `IStudentLookup` (from Students Module) |

### Dependency Discipline
- `AcademicSupervisors` depends solely on the public contract `IStudentLookup` / `StudentSummary`.
- `AcademicSupervisors` **never** references `StudentStorage` or accesses internal student entities directly.
- `Students` module has **zero** dependencies on `AcademicSupervisors`.

---

## 3. Comparison of the Three Model Types (Task 16)

| Concept / Element | Domain-Model Class Diagram | Implementation Class Diagram | Sequence Diagrams |
|---|---|---|---|
| **Purpose** | Captures pure business concepts, domain vocabulary, relationships, and invariants independent of technical framework. | Captures software structure, classes, methods, DI wiring, contracts, view models, and Razor presentation components. | Captures dynamic execution flow, message dispatch, error branches, and step-by-step business rule enforcement. |
| **Example: Student-Supervisor Relationship** | `Student "1" .. "0..*" SupervisoryAssignment` and `AcademicSupervisor "1" -- "0..*"` | `SupervisoryAssignment` holding `StudentNumber` (string) and `SupervisorStaffNumber` (string) as discrete foreign keys. | Controller calling `IStudentLookup.GetStudentSummary(studentNumber)` to validate student before saving assignment. |
| **Example: Supervisory Business Rules (BR01–BR15)** | UML notes on entities (`A student must have <= 1 current DoS and FirstSupervisor`). | `AcademicSupervisorService` methods returning `AssignmentOutcome` and `SupervisoryTeamAssessment`. | Explicit `alt / else` conditional blocks evaluating duplicate active assignment and supervisor role eligibility. |
| **Example: Data Persistence** | Abstract concept of entities and multiplicities. | In-memory singleton storage classes (`StudentStorage`, `AcademicSupervisorStorage`) owning `List<T>` fields. | `Storage -> Collection : Add(entity)` / `Where(...)`. |
| **Example: Module Contract** | Conceptual boundary between domain packages. | `IStudentLookup` interface returning immutable `StudentSummary` record. | Service dispatching message across module boundary via contract interface. |

---

## 4. Reflection Questions Answered

1. **Why are the domain-model and implementation class diagrams separate?**
   - The *domain model* represents business reality and rules (understandable by business analysts and domain experts) without polluting it with technical concerns (controllers, HTTP actions, Razor views, DI singletons). The *implementation model* represents the actual software design and structure in ASP.NET Core.

2. **Which module owns `SupervisoryAssignment`?**
   - The **Academic Supervisors Module** owns `SupervisoryAssignment` because managing supervisory teams, roles, and eligibility is an academic supervision capability, not a core student demographic concern.

3. **How is the student relationship represented without duplicating `Student`?**
   - `SupervisoryAssignment` stores only the student identifier (`StudentNumber : string`). When student display information is needed, it is resolved on-demand through the public `IStudentLookup` contract returning a lightweight `StudentSummary` record.

4. **Which business rules appear as service interactions?**
   - In `AcademicSupervisorService`:
     - Verifying supervisor role eligibility (`supervisor.EligibleRoles.Contains(role)`).
     - Preventing external supervisors from being DoS / FirstSupervisor (`BR13`).
     - Enforcing single active DoS and FirstSupervisor (`BR14`).
     - Preventing duplicate current assignments (`BR15`).
     - Requiring authorised exceptions for teams exceeding 3 supervisors (`BR02`, `BR03`).

5. **Does any diagram reveal direct access to another module's storage?**
   - **No.** The implementation diagram and all sequence diagrams strictly show `AcademicSupervisorService` accessing `IStudentLookup` and its own `AcademicSupervisorStorage`. Direct access to `StudentStorage` is completely absent.

6. **Which sequence contains the most cross-module interaction?**
   - **`W4-UC04` (Assign Academic Supervisor)** and **`W4-UC05` (View Student Supervisory Team)** contain the most cross-module interaction because they require resolving and confirming student identity from the Students module before executing supervisor-specific operations.

---

## 5. Extension: Candidate Business Module Analysis

### Candidate Module Evaluation

| Candidate Module | Related Requirements | Owned Concepts | Responsibilities | Evaluation & Viability |
|---|---|---|---|---|
| **Thesis Assessment & Examination** | PGR Viva voce, Examiner appointments (Internal/External/Chair), Viva outcomes, Corrections sign-off. | `ThesisExamination`, `Examiner`, `ExaminationPanelAppointment`, `ExaminationOutcome`, `ExaminerRole`. | Manages examination panel nominations, viva scheduling, recording exam outcomes, and tracking corrections deadlines. | **Selected (High Viability)**: Has distinct terminology, independent lifecycle, and clear institutional boundaries. |
| **PGR Progression & Annual Review** | Formal progress reviews, annual milestone monitoring, progression panel reports. | `ProgressionReview`, `Milestone`, `ReviewOutcome`, `PanelReviewer`. | Coordinates formal annual progression reviews, panel sign-offs, and confirmation of continuation. | Viable candidate for subsequent iterations. |
| **Ethics & Governance Approval** | Research ethics applications, hazard assessments, ethics panel approvals. | `EthicsApplication`, `EthicsReviewer`, `RiskLevel`, `EthicsApproval`. | Manages ethical clearance and compliance submissions for PGR projects prior to research commencement. | Independent capability; separate governance lifecycle. |

### Selected Candidate Module: Thesis Assessment & Examination
- **Name**: Thesis Assessment & Examination Module
- **Purpose**: Manage the appointment of thesis examination panels, viva scheduling, viva outcome recording, and corrections tracking for postgraduate researchers.
- **Owned Concepts**: `ThesisExamination`, `Examiner`, `ExaminationPanelAppointment`, `ExaminerRole`, `ExaminationOutcome`.
- **Dependencies**:
  - `Students Module`: Consumes `IStudentLookup` for student summary and thesis title.
  - `Academic Supervisors Module`: Verifies Director of Studies confirmation for examination readiness.
- **PlantUML Model**: Rendered in [`extension-thesis-assessment-module.puml`](file:///Users/martincooper/Documents/learning%20materials/sad-2026/PgrStudentManagement-Week4-Starter/Diagrams/extension-thesis-assessment-module.puml).
