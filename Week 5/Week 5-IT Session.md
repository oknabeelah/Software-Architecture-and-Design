20/10/2026
**Relational Data modelling and persistence**

In previous weeks, the application stored data in in-memory collections. In this lab, you will replace selected in-memory storage with relational persistence.

**Learning Outcomes**
By completing the core lab, you should be able to:
- Interpret a simple domain model and relational ERD.
- Implement and migrate a relational model using Entity Framework Core and SQLite.
- Use a DAO to retrieve persistent data through the service layer.
- Display persisted data through an ASP.NET Core MVC view while preserving module boundaries.
**NOTE:** If you are using the week 5 starter project, it is based on .NET sdk 8.0 and you will need guidance on how to make changes to ensure it works for version 10.0
	Open the project in VSCode
	modify the `.csproj` file
	change `<TargetFramework>` to `10`
	in `Program.cs` at the bottom of the page comment out the code for `.Net 8.0` and uncomment the code for `.Net 10.0`
	To confirm, open the terminal and do
	`cd /Pgr.Web`
	run `dotnet clean`
	run `dotnet restore`
	run `dotnet build`
	if it builds successfully with no error messages, it is now working on version 10

command for hot reload `dotnet watch run`

Fix for Entity Framework
```c#
// amend .csproj with:


```

**Part 1: Preserve the Week 4 Baseline**
- [x] Task 1: Check the Existing Application
- [x] Task 2: Review Current Data Ownership
	The academic supervisors list is owned by the academic supervisor module in the `AcademicSupervisorStorage class`, it contains the academic supervisor, supervisor employment type, and supervisory role models. it can be accessed by the `AcademicSupervisorService`.
	`private readonly List<AcademicSupervisor> _academicSupervisors`
	
	The supervisory assignment list is owned by the academic supervisor module in the `AcademicSupervisorStorage class`, it contains the supervisory assignment and supervisory role models. it can be accessed by the `AcademicSupervisorService`.
	`private readonly List<SupervisoryAssignment> _supervisoryAssignments`
	
	The students list is owned by the students module in the `StudentStorage` class, it contains the student and status models. it can be accessed by the `StudentService` and a public `StudentSummary` can be accessed by the `AcademicSupervisorService` via `IStudentLookup`
	`private readonly List<Student> _students`

**Part 2: Review the Initial Relational Model**
- [x] Task 3: Understand Student and Enrolment
	**Why Student and Enrolment are separate concepts -** because a student can be registered for multiple enrolments, it is easier to create a new enrolment model that tracks it. data like start date, end date, program code will be duplicated for a student with multiple enrolments and a single student object cannot track this, also it is not possible to have multiple records using the same student id in the students list so we are using enrolment to show this. 
	**Which values are required or optional -** the student id, enrolment id, start date, programme code, mode of study, and status are required
	**Which values should be unique -** student id and enrolment id
	**Which Student information remains stable across enrolments -** the student id, it will also serve as the foreign key in the db
	**Which Enrolment information may differ over time -** the expected end date, status, and possibly mode of study depending on business role
	**Whether modeOfStudy and status should use strings or enumerations -** as there are fixed values, using enums is more practical.  
	**How previous enrolments are retained and ordered -** using the end date, optionally adding a /current bool that is derived from (endDate is not null), when the student enrolments is listed it can be ordered by end or start date
- [x] Task 4: Review the Supplied Initial ERD
	**How is a student uniquely identified -** using their student number as the primary key which has to be unique 
	**How is an enrolment uniquely identified -** using the enrolment id as the primary key 
	**How does the relational model associate an enrolment with a student -** by introducing the student number as a foreign key in enrolment db
	**Can a student exist without an enrolment -** yes, although the business rule says that a student must be enrolled in at least one program, the student object must exist first before they are enrolled, hence the 1 to 0 or many relationship 
	**Can an enrolment exist without a student -** no, because an enrolment cannot be created without a foreign key student number 
	**Which columns are required -** columns labelled required `*` 
	**Why is expected_end_date optional -**  when a student is enrolled, the program can go on for as long as possible, expected end date is merely a suggestion. an update to the ERD could be to make expected end date required and add a new optional actual end date field
	**Which constraints should be enforced by the database -** data type and limit. don't implement stored procedures (checking business rules in db) because we have the services layer for this
	**Which rules should remain in application services -** all the business rules
	**Does the model allow one student to have several enrolments over time -** yes it does currently, a potential business rule could be that a student cannot be enrolled in 2 full time programs at the same time

```puml
||--o : One to Zero or One
||--|| : One to Exactly One
||--{ : One to One or Many
||--o{ : One to Zero or Many

```

**Part 3: Implement the Initial Model with Entity Framework Core**
- [x] Task 5: Add Entity Framework Core
	Entity Framework Core .NET Command-line Tools
	10.0.13
- [x] Task 6: Add the Connection String
- [x] Task 7: Implement the Initial Entity Classes
- [x] Task 8: Create and Register the Initial DbContext
- [x] Task 9: Configure the Initial Relational Mapping
	For each element in the ERD, identify how it is represented using one of these mechanisms:
	Convention - enrolment_id 
	Data annotation - student_number, first_name, last_name, email_address, programme_code, start_date, expected_end_date, mode_of_study
	Fluent API - enrolment_id, enrolment_status, student_number(FK), 
	C# type and nullability
- [x] Task 10: Create and Inspect the Initial Migration
- [x] Task 11: Create, Seed, and Test the Initial Database
- [ ] Task 12: Display the Seeded Enrolment Data Through the Application
- [ ] Task 13: Verify Persistence and Existing Behavior

**Extension Tasks**
- [ ] Extension 1: Update the Existing Implementation Class Diagram
- [ ] Extension 2: Refactor the Remaining Student Use Cases
- [ ] Extension 3: Select and Implement a New Relational Module
- [ ] Extension 4: Reverse-Engineer the Database
- [ ] Extension 5: Introduce a Data Transfer Object

TO DO
- [ ] convert w5 pdf files to md
- [ ] add in the lab instruction files for all the weeks
- [ ] add in ppt files
- [ ] link lectures
- [ ] link milanote
