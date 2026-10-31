<style>
.uml-comparison {
    display: grid;
    grid-template-columns: minmax(0, 1fr) minmax(0, 1fr);
    gap: 1.5rem;
    align-items: start;
}

.uml-source,
.uml-diagram {
    min-width: 0;
}

.uml-source pre,
.uml-diagram pre {
    overflow-x: auto;
}

@media (max-width: 800px) {
    .uml-comparison {
        grid-template-columns: 1fr;
    }
}
</style>

# Week 2 Lab 2: Reverse-Engineer Your Implementation

## Lab Purpose

In Lab 1, you implemented five Student Management use cases and made decisions about the fields, views, messages, and behaviour included in your application.

In this lab, you will examine the implementation you created and produce UML models that describe what the code currently does.

This is a reverse-engineering activity. Your diagrams must represent your implementation, not an ideal solution and not a design copied from a later week.

## Files and Project Required

Before starting, make sure you have:

- Your completed Week 2 Lab 1 project
- `week02-lab-instructions-final.md`
- `week02-domain-model-student-lab.md`
- `week02-usecases-student.md`
- Your revised domain model
- Any use-case changes or design decisions made during Lab 1
- Any test cases created as a Lab 1 extension

## Learning Outcomes

By completing this lab, you should be able to:

1. Identify relevant implementation elements in an ASP.NET Core MVC project.
2. Reverse-engineer a UML class diagram from source code.
3. Create sequence diagrams that describe implemented use-case behaviour.
4. Compare a candidate domain model, refined use cases, and an implementation.
5. Identify coupling, mixed responsibilities, shared state, and other architectural concerns.
6. Preserve architectural models for comparison with later iterations.

## Modelling Principle

Model what exists in your completed Lab 1 implementation.

Do not add a service, repository, database, or other component to a diagram unless it exists in your code.

Differences between students' diagrams are expected because you may have selected different domain fields, renamed attributes, created different views, or implemented different controller actions.

# Core Tasks

## Task 1: Identify Relevant Implementation Elements

Review the code you created or modified for the five Week 2 use cases.

Identify the elements that participate in those use cases, including:

- The `Student` domain class
- The `Status` enumeration
- The Students controller
- The in-memory Student collection
- The Student Razor views
- Any additional runtime classes you introduced

Do not include:

- Generated files
- Framework internals
- Static CSS or JavaScript libraries
- Build output under `bin` or `obj`
- NuGet package classes
- Files that do not participate in the Week 2 use cases

If you added a test project during the Lab 1 extension, keep its classes separate from the runtime application elements.

## Task 2: Create an Implementation Class Diagram

Create a PlantUML class diagram representing your implementation.

Your diagram should show, where present:

- Relevant namespaces or packages
- Classes and enumerations
- Fields and properties
- Controller actions
- Important relationships or dependencies
- The location and type of the in-memory collection

Use C# property notation rather than inventing Java-style getter and setter methods.

For example:

<div class="uml-comparison" markdown="1">

<div class="uml-source" markdown="1">

**PlantUML source**

```text
set namespaceSeparator none
package PgrStudentManagement.Web {
    package Models {
        class Student {
            +StudentNumber : string
            +FirstName : string
            +Status : Status
        }
    }
}
```
</div>

<div class="uml-diagram" markdown="1">

**Diagram**

```plantuml
set namespaceSeparator none
package PgrStudentManagement.Web {
    package Models {
        class Student {
            +StudentNumber : string
            +FirstName : string
            +Status : Status
        }
    }
}
```
</div>
</div>


An enumeration used as a property type can be shown through the property itself. If you also show a dependency, use a dependency arrow:


<div class="uml-comparison" markdown="1">

<div class="uml-source" markdown="1">

**PlantUML source**

```text
Student ..> Status : uses
```

</div>

<div class="uml-diagram" markdown="1">

**Diagram**

```plantuml
Student .r.> Status : uses
```

</div>
</div>

If you include Razor views, represent them as stereotyped elements. For example:


<div class="uml-comparison" markdown="1">

<div class="uml-source" markdown="1">

**PlantUML source**

```text
set namespaceSeparator none
package PgrStudentManagement.Web {
    package Views {
        class "EnrolmentDetails.cshtml" as EnrolmentDetailsView <<view>>
    }
}
```

</div>

<div class="uml-diagram" markdown="1">

**Diagram**

```plantuml
set namespaceSeparator none
package PgrStudentManagement.Web {
    package Views {
        class "EnrolmentDetails.cshtml" as EnrolmentDetailsView <<view>>
    }
}
```

</div>
</div>

Explain any stereotype or notation you introduce and use it consistently.

Use the namespace from your own project. Do not copy a package name that does not match your implementation.

## Task 3: Create Sequence Diagrams for the Core Use Cases

Create one sequence diagram for each use case implemented in Lab 1:

- `W2-UC01`: View Enrolment Details
- `W2-UC02`: View Thesis Details
- `W2-UC03`: Update Student Status
- `W2-UC04`: Update Expected Thesis Submission Date
- `W2-UC05`: Record Actual Thesis Submission

For each sequence diagram:

1. Begin with the primary actor from the use-case description.
2. Include the view or browser interaction represented by your implementation.
3. Include the controller action or actions that are called.
4. Show access to the in-memory collection where it occurs.
5. Show changes made to the `Student` object where they occur.
6. Show the information or view returned to the actor.
7. Include the supplied alternative flow from the corresponding use case.
8. Use the attribute names and messages from your implementation.

Do not add a participant that does not exist in your code.

Your sequence diagrams may differ from the tutor examples because the fields and implementation choices made in Lab 1 may differ.

## Task 4: Compare the Domain Model, Use Cases, and Implementation

Compare:

- The candidate domain model supplied in Lab 1
- Your refined use-case descriptions
- Your implementation class diagram
- Your sequence diagrams

Identify:

- Fields you retained
- Fields you omitted
- Fields you renamed or added
- Status values you retained, removed, or added
- Behaviour implemented in the controller
- Behaviour implemented in the views
- Differences between the described use cases and implemented behaviour

Record the comparison in a short Markdown section or table.

Do not change your diagrams merely to match the candidate model. The diagrams should describe the implementation you actually produced.

## Task 5: Identify Architectural Concerns

Review your implementation and diagrams for the following concerns.

### Tight Coupling

Look for classes that directly depend on another element's implementation details.

### Poor Separation of Concerns

Look for request handling, business decisions, data access, data modification, and navigation being mixed in one place.

### Shared Global State

Look for a static or globally accessible collection that can be changed from several actions.

### Large or God Classes

Look for a class that has too many unrelated responsibilities or knows too much about the application.

### Repeated Behaviour

Look for repeated student-search, message, mapping, or update logic.

### Change Impact

Consider how many files would need to change if:

- Student data moved from memory to a file or database
- A new status rule was introduced
- Another controller needed access to Student data
- The application stored 100,000 students
- Ten developers changed the application at the same time

For each concern you identify:

1. State where it appears.
2. Cite the relevant class, action, field, or relationship.
3. Explain the possible consequence.
4. Do not redesign the application at this stage.

## Task 6: Check Model Consistency

Check that:

- Every class shown exists in the implementation.
- Every property and method shown exists in the implementation.
- Every message in a sequence diagram corresponds to an implemented interaction.
- Every use-case participant appears consistently across diagrams.
- The supplied alternative flow is represented in the corresponding sequence diagram.
- Field names match your revised domain model and code.
- View names and controller action names match the project.
- The diagrams do not silently introduce a service, repository, or database.

# Extension Tasks

## Extension Task 1: Include Test Classes

If you created a test project in the Lab 1 extension, create a separate test package or diagram showing:

- Test classes
- The runtime classes they exercise
- Which use cases or scenarios the tests cover

Do not mix test classes with runtime application classes in a way that suggests they are deployed together.

## Extension Task 2: Model Additional Alternative Flows

If you completed the additional-alternative-flow extension in Lab 1, revise the relevant sequence diagrams to show one or more of those flows.

Do not add an alternative branch unless it is also documented in the corresponding use-case description and represented by the implementation.

# Discussion Questions

1. Which class has the most responsibilities?
2. Where is Student data stored?
3. Which actions directly search or modify the collection?
4. What happens to changes when the application restarts?
5. Which code is repeated across controller actions?
6. How closely does the implementation match the refined use cases?
7. How closely does the implementation match the candidate domain model?
8. Which differences were deliberate design decisions?
9. Which differences resulted from incomplete implementation?
10. Which parts of the current design may become difficult to change?

# Completion Checklist

- [ ] I used my completed Lab 1 project as the modelling source.
- [ ] I identified only relevant runtime implementation elements.
- [ ] I created an implementation class diagram.
- [ ] I used C# property notation.
- [ ] I used the namespaces, classes, methods, fields, and views from my project.
- [ ] I created sequence diagrams for `W2-UC01` to `W2-UC05`.
- [ ] Each sequence diagram includes its supplied alternative flow.
- [ ] My sequence diagrams use the field choices made in Lab 1.
- [ ] I compared the candidate domain model, refined use cases, and implementation.
- [ ] I identified architectural concerns without redesigning the application.
- [ ] I checked that the diagrams describe the implementation rather than an ideal solution.

# Keeping the Models for Later Weeks

There is no submission for this lab.

Keep the following in your own files or repository:

- Your completed Week 2 project
- Your refined use cases
- Your revised domain model
- Your reverse-engineered class diagram
- Your five sequence diagrams
- Your comparison notes
- Your architectural observations
- Any extension diagrams or tests

You will revisit these artefacts in later weeks to compare the effects of architectural changes.
