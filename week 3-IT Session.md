06/10/2026

last week we used MVC, we also learnt there are situations where we have 'super controller' - when the controller has too many responsibilities its more disadvantageous to use MVC.

the new architecture we are looking at is called layered architecture - where instead of MVC we have layers that interact with either with the directly next layer (strict) or any layer (relaxed)

### Goal: refactor lab1 application to implement layered architecture

Layer responsibilities
	Presentation layer: Controller and view
	Business Layer: Service
	Data Layer: Storage

Presentation Layer: includes StudentController and Razor Views
Business Layer: includes StudentService
Data Layer: includes StudentStorage

Task 1 - Review current responsibilities
look at StudentController and identify where the code:
	Receives requests
	Selects views or redirects
	Finds students
	Updates student information
	Applies business rules
	Orders records
	Accesses the static collection.
Record each responsibility, its current location, and its intended Week 3 layer.

---
08/10/2026

continue layered implementation

Presentation - replacing view
Business - replacing model + controller (to some extent)
Data - replacing model

- [x] L1 complete UC01 to UC05 with StudentStorage and StudentService refactored cleanly
- [x] L1 Implement W3UC01
- [ ] L1 Implement W3UC02
- [ ] L1 Implement W3UC03
- [ ] L1 Implement W3UC04 (optional)
- [ ] L1 Implement W3UC05 (optional)
- [ ] L1 Create test suite (optional)
- [ ] L2 Make class diagram for week 3
- [ ] L2 Make use case diagrams for UC01 to UC05 for week 3

---
W3UC01 Design Decisions
Which student attributes are mandatory?
	Student number, first name, last name, course, mode of study, start date, and status.
Which student attributes are optional?
	thesis title, thesis expected and actual submission dates, 
Which attributes should appear on the creation form?
	all the above mandatory and optional attributes
Which additional attribute values require business rules?
	
How should missing optional information be represented?
	the enrollment details form only shows the mandatory info so there is no immediate concern for how optional info is represented, however it is typically null
