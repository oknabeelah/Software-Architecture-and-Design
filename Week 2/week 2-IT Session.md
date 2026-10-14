29/09/2026

 Soln - https://github.com/mustbebuilt/PgrStudentManagementWk2

### Task 1 - Review the use cases

Use case 1: View enrolment details
	**Info to be displayed:** programme, mode of study, start date, and status
	**Info to be updated:** n/a
	**Actor, trigger, precondition, postcondition:** Student, onClick /Student/EnrollmentDetails(studentId), has studentId, ...
	**Alternative flow:** throw User notFound
	**Important terminology:** ..


### Task 2 - Review and Refine the Domain Model

Use case 1:
	review the model
		review the supplied fields in the domain `model week02-domain-model-student-lab.md` and decide which fields are mandatory and optional
		add any missing fields expected by use case
		rename or remove fields at your discretion
		?review the status enumeration and ensure it supports all use cases
	add appropriate business rules
		identify and implement appropriate business rules on the use cases in the student model
		document decisions before implementing
Use case 2:
	review the model
	add appropriate business rules
Use case 3:
	review the model
	add appropriate business rules



---
Lab 2 - Reverse engineer a UML class diagram from the code and create sequence diagrams that describe implemented behavior

### Task 1 - Review modified code
in StudentController.cs
	added functionality for `public IActionResult EnrolmentDetails(string studentNumber)`
	added functionality for `public IActionResult ThesisDetails(string studentNumber)`
in Views/Students
	updated `EnrolmentDetails.cshtml`
	updated `ThesisDetails.cshtml`
	added `NoThesisDetails.cshtml`



