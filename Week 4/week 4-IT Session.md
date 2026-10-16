13/10/2026
### Modular Monolith
! Engage with literature
	https://martinfowler.com/bliki/DomainDrivenDesign.html

? Splitting code into different modules and figuring out how they communicate - contracts, microservices, network calls,
? how to share data between modules - 

Goal
	Identifying modules from  business capabilities
	Explaining the difference between a layered monolith and modular monolith

Task
	Create a new module to represent Academic supervisors with its own presentation, business, data, and domain elements
	Represent student-supervisor relationships using "SupervisoryAssignment".
	Define and respect module ownership
	Coordinate a UC requiring info from two modules
	Keep UC's, domain models, module boundaries, views, and
	code consistent.

Files
	completed week 3 code
	week 4 UC's
	week 4 domain model (student, supervisor)
	quick guide
	lab instructions

**A modular monolith** is a single application divided internally into modules that represent business capabilities.
**A business capability** is something the organisation needs to be able to do to achieve its goals. It describes a cohesive area of business responsibility rather than a technical layer, screen, class, or database table.
	*Student Management* maintains student records, enrolment information, status, and thesis information.
	*Academic Supervisor Management* will maintain supervisor records, eligibility, and supervisory assignments.

A business capability usually has its own terminology, rules, information, lifecycle, and related use cases. 
In a modular monolith, a module should normally represent one cohesive business capability and own the domain concepts and operations needed to provide it.
Each module should:
	Own its domain concepts.  
	Own the operations that manage those concepts.  
	Hide internal implementation details from other modules.  
	Expose only the operations required by other parts of the application.

**NOTE:** A modular monolith is still deployed as one application. The distinction is its internal organisation and dependency discipline.

%%how to Obsidian custom checkbox start ie - [ ] T1: %%

- [x] T1: Preserve and check the week 3 baseline
- [x] T2: Identify the module boundaries
	Add the new Week 4 concepts and decide which module owns them?
- [x] T3: Reorganise the student Module
	reorganised successfully, resolved unhandled exception, confirmed existing use cases are still working
- [x] T4: Review the week 4 domain model
	**Which suggested fields you will include** - All suggested supervisor and assignment fields required by the use cases are included.
	add withdrawn to Status enum
	**Which fields are mandatory or optional** - 
	**How external supervisors are identified using the supplied staffNumber model** - staffNumber is the unique identifier for both institution-employed and external supervisors; external supervisors use an institution- or system-issued value in the same field with a prefix distinction (IE for institution employed and E for external) when creating a new employee a dropdown asking whether a supervisor is institution employed or external will be used to build staff number string
	**Whether eligible roles are stored or derived** - eligibleRoles is stored because eligibility is role-specific and can be updated.
	**Whether additional information is needed to explain why a supervisor is eligible for a role** - add an optional hash set field in academic supervisor to store roles for which an exception to be eligible was authorised and set authorised exception to true for the assignment. to confirm simply check that the the supervisor either has the role in eligible roles or in "excepted roles" and authorised exception must be true. additionally, add Student and Academic supervisor field to supervisor assignments
	**Whether current assignment state is calculated from endDate** - current is derived from endDate == null; historical assignments remain retained.
	**How authorised exceptions are represented** - 
	additionally excepted roles could be an object rather than hash set with the following fields for documentation of formal process. exceptionAuthorised, exceptionReason, exceptionAuthorisedBy, exceptionAuthorisedDate
	**Whether an assignment requires an end reason** - this could be added as an optional property only if there is an end date. 
	**What information is required to authorise an exception** - a request could be put in by a student to an IE employee requesting an external supervisor citing reason. this is then used in the "excepted roles" object by adding the above info and setting exceptionAuthorised depending on the outcome
	Whether additional training details or dates are required - a new training object could track trainings, dates assigned, and to who.  the trainingCompleted bool in AcademicSuprvisor is then evaluated based on whether all training assigned to a supervisor (after their start date) have been completed
	How Academic Supervisors and assignments are displayed and ordered - display academic supervisors as a list to admin with filtering for internal and external employees. adding assignments to the student object is a way to link the student to their supervisor  
- [x] T5: Create the Academic Supervisors module
- [x] T6: Create AcademicSupervisorStorage
	updated the domain model as i went
- [x] T7: Create AcademicSupervisorService
	Need to re implement Update method
- [x] T8: Register the Academic Supervisors module (see appendix)
	used custom service extension
- [x] T9: Implement W4-UC01 Create Academic Supervisor
- [x] T10: Implement W4-UC02 List Academic Supervisors
- [x] T11: Implement W4-UC03 View Academic Supervisor Details
- [x] T12: Define the Student Lookup Contract
	**Which module owns the contract** - Student module
	**Which class implements it** - StudentService class
	**Which student fields are exposed through StudentSummary** - Student number and Student full name
	**Why those fields are required by the supervisory use cases** - in UC04 and UC05, its used to confirm that a provided student number is indeed valid
	**How the contract prevents direct access to the Students module's internal data** - 
	**How the contract is registered with dependency injection** - 
- [x] T13: Implement W4-UC04 Assign Academic Supervisor to Student
	Decide and document:  
	**Whether Adviser assignments count towards the supervisor number limit** - yes they do, all current assignments cannot exceed 3 without authorised exception
	**Where an authorised fourth-supervisor exception is recorded** - bool exception authorise and expanded in new authorised exception model and 
	**Whether eligibility is stored or derived** - derived from employment type 
	**How a current assignment is identified** - current bool
	**Whether start dates include time or only a calendar date** -   should not include time
	How incomplete and complete supervisory teams are displayed - n/a 
	Which service operation evaluates team completeness - n/a
- [x] T14: Implement W4-UC05 View Student Supervisory Team
	Record your decisions about: 
	**Fields displayed for each supervisor**  - Staff number, role, start date, end date, assignment state
	**Assignment ordering**  - In chronological order
	**Current and historical grouping**  - Displayed as one table
	**Representation of the derived current state** - replaced end date with current if true
- [x] T15: Check Module Encapsulation
	Create a short dependency table showing each cross-module dependency and its purpose.
	| Dependency | Owning module | Consumer | Purpose |
	| `IStudentLookup` | Student Management | Academic Supervisor Management | Validates that a student exists and provides the student number required by supervisory use cases. |
	| `StudentSummary` | Student Management | Academic Supervisor Management | Exposes limited student identity data without exposing student storage. | 
	The Academic Supervisor Management module owns supervisors, supervisory assignments, and their business rules. It depends on Student Management only through the `IStudentLookup` contract. It does not access `StudentStorage` or the Students collection directly. The contract is registered by `AddStudentsModule()` and injected into `AcademicSupervisorService`.
- [x] T16: Verify the Complete Core Application
- [ ] ET1: Implement W4-UC06 Update Academic Supervisor Eligibility
- [ ] ET2: Implement W4-UC07 End Supervisory Assignment
- [ ] ET3: Implement W4-UC08 View Academic Supervisor Assignments
- [ ] ET4: Add Module-Level Tests
- [ ] ET5: Identify Additional Alternative Flows
- [x] Answer Reflection questions

---

Lab 2
Goals
	Reverse-engineer domain concepts and relationships from source code.
	Separate a domain-model diagram from an implementation-structure
	diagram.
	Represent module ownership and module boundaries in UML.
	Represent controllers, services, storage classes, contracts, and views.
	Create sequence diagrams from implemented use-case behaviour.
	Check consistency between use cases, models, diagrams, and code.
	Identify potential modules from requirements and business capabilities.

A class diagram that captures the domain model
A separate class diagram that captures the rest of the implementation
Sequence diagrams for the five new Week 4 core UCs

