# Week 3 Use-Case Descriptions 

## Purpose 

Week 3 continues the Student Management work completed in Week 2. 

The five Week 2 use cases remain part of the core application. Week 3 adds three core student-management use cases and two extension use cases. 

## Requirements Traceability 

The reused use cases continue to support `US001` , `US004` , `FR02` , `FR07` , `FR10` , and `BR12` . 

The new Week 3 use cases support `FR02` and provide the record-management operations needed by the student-facing and administrator-facing backlog stories. `W3-UC05` also supports `FR13` by presenting statistics calculated from student records. 

## Use-Case Summary 

|**Use**<br>**Case**<br>**ID**|**Origin**|**Related**<br>**Requirements**|**Use Case**|**Primary Actor**|**Classification**|
|---|---|---|---|---|---|
|W2-<br>UC01|US001|FR02|View Enrolment<br>Details|Student|Reused Week 2<br>core|
|W2-<br>UC02|US001|FR02, FR10|View Thesis Details|Student|Reused Week 2<br>core|
|W2-<br>UC03|US004|FR02, FR07, BR12|Update Student<br>Status|College<br>Administrator|Reused Week 2<br>core|
|W2-<br>UC04|US004|FR02, FR10|Update Expected<br>Thesis Submission<br>Date|College<br>Administrator|Reused Week 2<br>core|
|W2-<br>UC05|US004|FR02, FR07,<br>FR10, BR12|Record Actual<br>Thesis Submission|College<br>Administrator|Reused Week 2<br>core|
|W3-<br>UC01|Supporting<br>operation|FR02|Create Student|College<br>Administrator|New Week 3<br>core|
|W3-<br>UC02|Supporting<br>operation|FR02|List Students|College<br>Administrator|New Week 3<br>core|
|W3-<br>UC03|Supporting<br>operation|FR02|View Student<br>Details|College<br>Administrator|New Week 3<br>core|
|W3-<br>UC04|Supporting<br>operation|FR02|Search Students|College<br>Administrator|Week 3<br>extension|

| Use Case ID | Origin | Related Requirements | Use Case | Primary Actor | Classification |
|---|---|---|---|---|---|
| W2-UC01 | US001 | View Enrolment Details | Student | View current enrolment information, including programme, mode of study, start date, and status | Student Not Found |
| W2-UC02 | US001 | View Thesis Details | Student | View thesis information, including title and expected or actual submission dates | Thesis Information Not Available |
| W2-UC03 | US004 | Update Student Status | College Administrator | Change the current academic status of a student | Student Not Found |
| W2-UC04 | US004 | Update Expected Thesis Submission Date | College Administrator | Change the date by which a student is expected to submit their thesis | Student Not Found |
| W2-UC05 | US004 | Record Actual Thesis Submission | College Administrator | Record the actual thesis submission date and update the student status to Submitted | Invalid Submission Date |





|**Use**<br>**Case**<br>**ID**|**Origin**|**Related**<br>**Requirements**|**Use Case**|**Primary Actor**|**Classification**|
|---|---|---|---|---|---|
|W3-|Supporting|FR02 FR13|View Student|College|Week 3|
|UC05|operation|,|Statistics|Administrator|extension|



The use cases for week 2 continue to apply. Refer to the Week 2 use-case document for their complete descriptions. 

## Use-Case Diagram 



<!-- Start of picture text -->
Week 3 - Student Management Use-Case Diagram<br>Student Management System<br>Core Scope Extension Scope<br>Reused Week 2 Use Cases W3-UC05<br>View Student Statistics<br>W2-UC03<br>Update Student Status<br>W3-UC04<br>Search Students<br>W2-UC04<br>Update Expected Thesis<br>Submission Date<br>W2-UC05<br>Record Actual Thesis<br>Submission<br>W2-UC01<br>View Enrolment Details College Administrator<br>Student W2-UC02<br>View Thesis Details<br>New Week 3 Use Cases<br>W3-UC01<br>Create Student<br>W3-UC02<br>List Students<br>W3-UC03<br>View Student Details<br><!-- End of picture text -->

Core Use-Case Descriptions 

2 / 10 

week03-use-cases-student.md 

2026-10-02 

## W3-UC01: Create Student 

**Supports:** FR02 

**Primary Actor:** College Administrator 

**Classification:** Core 

### Goal 

Create a student record so that the student's enrolment and thesis information can be managed by the system. 

### Preconditions 

The Student Management System is available. 

The College Administrator can access student management functionality. 

### Trigger 

The College Administrator selects **Create Student** . 

### Main Success Scenario 

1. The College Administrator requests to create a student. 

2. The system displays the student creation form. 

3. The College Administrator enters the required student information. 

4. The College Administrator may enter optional enrolment information and thesis information. 

5. The College Administrator submits the student information. 

6. The system checks that the required information has been supplied. 

7. The system checks that the student number is not already in use. 

8. The system creates the student record. 

9. The system confirms that the student has been created. 

10. The system displays the new student's details. 

11. The College Administrator reviews the student record. 

### Alternative Flow A1: Duplicate Student Number 

At Step 7: 

1. A student with the supplied student number already exists. 

2. The system does not create another student record. 

3. The system displays **A student with this student number already exists** . 

4. The system redisplays the student creation form. 

5. The College Administrator may provide a different student number and resubmit. 

### Postconditions 

#### **Success** 

A new student record has been created. 

The student number uniquely identifies the student. 

3 / 10 

week03-use-cases-student.md 

2026-10-02 

The student information supplied by the College Administrator has been recorded. 

The new student appears in the list of students. 

#### **Failure** 

- No student record has been created. 

Existing student records remain unchanged. 

- A duplicate-student-number message has been displayed. 

### Business Rules 

Every student must have a unique student number. 

- All mandatory student information must be supplied before a student record can be created. The student number must not be changed automatically by the system. 

- Optional enrolment and thesis information may be recorded later. 

- The fields displayed and stored must be consistent with the selected domain model. 

### Design Decisions 

Review the supplied domain model and decide: 

Which student attributes are mandatory? 

- Which student attributes are optional? 

- Which attributes should appear on the creation form? 

- Which additional attribute values require business rules? 

- How should missing optional information be represented? 

Ensure that the creation form, use-case description, UML models, and implementation use the same set of mandatory and optional attributes. 

## W3-UC02: List Students 

**Supports:** FR02 

- **Primary Actor:** College Administrator **Classification:** Core 

### Goal 

View the students recorded in the system so that the College Administrator can locate and manage student information. 

### Preconditions 

The Student Management System is available. 

The College Administrator can access student information. 

### Trigger 

The College Administrator selects **Students** . 

Main Success Scenario 

4 / 10 

week03-use-cases-student.md 

2026-10-02 

1. The College Administrator requests the list of students. 

2. The system retrieves all available student records. 

3. The system finds one or more students. 

4. The system orders the students using the ordering rule selected in the design. 

5. The system displays the selected summary information for each student. 

6. The system provides an option to view each student's details. 

7. The College Administrator reviews the student list. 

### Alternative Flow A1: No Students Found 

At Step 3: 

1. No student records are available. 

2. The system displays **No students found** . 

3. The system may provide an option to create a student. 

4. The use case ends. 

### Postconditions 

#### **Success** 

The available students have been displayed. 

The students have been presented in the selected order. 

No student information has been changed. 

#### **Failure** 

No student information has been changed. 

A **No students found** message has been displayed. 

### Business Rules 

- Each student must be identified by their student number. 

- Student records must be displayed in a consistent order. 

- The student list should display summary information rather than every available student attribute. Viewing the student list must not modify student records. 

### Design Decisions 

Decide which student attributes should appear in the student list. 

Consider: 

- Which attributes help users identify a student? 

- Which attributes are suitable for a summary view? 

- Which attributes should appear only in the details view? 

- In what order should students be displayed? 

Record your decisions and ensure that the use-case description, UML models, list view, and implementation remain consistent. 

5 / 10 

week03-use-cases-student.md 

2026-10-02 

## W3-UC03: View Student Details 

**Supports:** FR02 

**Primary Actor:** College Administrator 

**Classification:** Core 

Goal 

View the information recorded for a selected student. 

### Preconditions 

The Student Management System is available. 

The College Administrator can access student information. 

A student number has been selected or supplied. 

### Trigger 

The College Administrator selects **Details** for a student. 

### Main Success Scenario 

1. The College Administrator selects a student record. 

2. The system receives the student number. 

3. The system searches for the student. 

4. The system finds the matching student record. 

5. The system retrieves the available student information. 

6. The system displays the student attributes selected for the details view. 

7. The College Administrator reviews the student details. 

### Alternative Flow A1: Student Not Found 

At Step 4: 

1. No student matches the supplied student number. 

2. The system does not display student details. 

3. The system displays **Student not found** . 

4. The use case ends. 

### Postconditions 

#### **Success** 

The selected student's available details have been displayed. 

No student information has been changed. 

#### **Failure** 

No student details have been displayed. 

No student information has been changed. 

6 / 10 

week03-use-cases-student.md 

2026-10-02 

#### A **Student not found** message has been displayed. 

### Business Rules 

The student number uniquely identifies a student. 

- The details view may contain more information than the student list. Missing optional information may be omitted or displayed as **Not recorded** . The treatment of missing optional information must be consistent. Viewing student details must not modify the student record. 

### Design Decisions 

Decide which student attributes should appear in the student details view. 

The details view may contain more information than the student list. 

Consider: 

- Which attributes are required to support the use case? Which attributes are required by the actor? 

- Which attributes are optional? 

How should missing optional information be displayed? 

- Should missing values be omitted or displayed as **Not recorded** ? 

Apply the selected approach consistently in the use-case description, UML class diagram, view, and implementation. 

# Extension Use-Case Descriptions 

## W3-UC04: Search Students 

**Supports:** FR02 

- **Primary Actor:** College Administrator **Classification:** Extension 

### Goal 

Search for students so that the College Administrator can quickly locate relevant student records. 

### Preconditions 

The Student Management System is available. 

The College Administrator can access student information. 

One or more student records are available. 

### Trigger 

The College Administrator enters one or more values in the student search function. 

Main Success Scenario 

7 / 10 

week03-use-cases-student.md 

2026-10-02 

1. The College Administrator opens the student list. 

2. The system displays the student search function. 

3. The College Administrator enters one or more search values. 

4. The College Administrator submits the search. 

5. The system compares the search criteria with the student attributes selected as searchable in the design. 

6. The system finds one or more matching students. 

7. The system orders the matching students using the selected ordering rule. 

8. The system displays summary information for each matching student. 

9. The College Administrator reviews the search results. 

### Alternative Flow A1: No Matching Students 

At Step 6: 

1. No student records match the supplied search criteria. 

2. The system displays **No matching students found** . 

3. The use case ends. 

### Postconditions 

#### **Success** 

The students matching the search criteria have been displayed. 

No student information has been changed. 

#### **Failure** 

No student information has been changed. 

A **No matching students found** message has been displayed. 

Business Rules 

Search results must contain only students that satisfy the supplied search criteria. 

- Search results must be displayed in a consistent order. 

- Searching must not modify student information. 

### Design Decisions 

Design the search behaviour for the student list. 

Decide: 

Which student attributes are searchable. 

- Whether one or several criteria can be supplied. 

- Whether partial matches are supported. 

- Whether text comparisons are case-sensitive. 

- How multiple search criteria are combined. 

- Whether an empty search displays all students, displays no results, or is not submitted. 

8 / 10 

week03-use-cases-student.md 

2026-10-02 

Document your decisions and ensure that the use-case description, UML models, search interface, and implementation remain consistent. 

## W3-UC05: View Student Statistics 

**Supports:** FR02, FR13 

- **Primary Actor:** College Administrator **Classification:** Extension 

### Goal 

View summary statistics about student records so that the College Administrator can understand the current student population. 

### Preconditions 

The Student Management System is available. 

The College Administrator can access student statistics. 

### Trigger 

The College Administrator selects **Student Statistics** . 

### Main Success Scenario 

1. The College Administrator requests student statistics. 

2. The system retrieves all available student records. 

3. The system checks that one or more students are available. 

4. The system calculates the total number of students. 

5. The system calculates the additional statistics selected in the design. 

6. The system displays the calculated statistics. 

7. The College Administrator reviews the student statistics. 

### Alternative Flow A1: No Student Data Available 

At Step 3: 

1. No student records are available. 

2. The system does not calculate the statistics. 

3. The system displays **No student data available** . 

4. The use case ends. 

### Postconditions 

#### **Success** 

The selected student statistics have been calculated and displayed. 

No student information has been changed. 

#### **Failure** 

9 / 10 

week03-use-cases-student.md 

2026-10-02 

No statistics have been calculated. 

No student information has been changed. 

A **No student data available** message has been displayed. 

### Business Rules 

Every student must be included once in the total student count. 

The statistics must be calculated using the current student records. 

- A student must not be counted more than once within the same category. 

- Category totals must be consistent with the applicable student records. 

- Viewing statistics must not modify student information. 

### Design Decisions 

The total number of students is required. Select at least one additional meaningful statistic that can be calculated using the student attributes included in your design. 

Possible statistics include: 

- Number of students by current status 

- Number of students by mode of study 

- Number of students by research programme 

- Number of students with an expected thesis submission date 

- Number of students with an actual thesis submission date 

For each selected statistic: 

1. Define what it measures. 

2. Identify the student attributes required to calculate it. 

3. Define the calculation or counting rule. 

4. Decide how it will be displayed. 

5. Consider what should happen when no student data is available. 

Update the use-case description, UML models, statistics view, and implementation to reflect your decisions. 

10 / 10 

