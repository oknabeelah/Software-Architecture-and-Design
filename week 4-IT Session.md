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
- [ ] T7: Create AcademicSupervisorService
- [ ] T8: Register the Academic Supervisors module (see appendix)
- [ ] T9: Implement W4-UC01 Create Academic Supervisor
- [ ] T10: Implement W4-UC02 List Academic Supervisors
- [ ] T11: Implement W4-UC03 View Academic Supervisor Details
- [ ] T12: Define the Student Lookup Contract
- [ ] T13: Implement W4-UC04 Assign Academic Supervisor to Student
- [ ] T14: Implement W4-UC05 View Student Supervisory Team
- [ ] T15: Check Module Encapsulation
- [ ] T16: Verify the Complete Core Application
- [ ] ET1: Implement W4-UC06 Update Academic Supervisor Eligibility
- [ ] ET2: Implement W4-UC07 End Supervisory Assignment
- [ ] ET3: Implement W4-UC08 View Academic Supervisor Assignments
- [ ] ET4: Add Module-Level Tests
- [ ] ET5: Identify Additional Alternative Flows
- [ ] Answer Reflection questions

---
