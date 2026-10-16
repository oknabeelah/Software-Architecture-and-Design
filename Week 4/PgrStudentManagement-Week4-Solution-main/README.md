# PGR Student Management: Week 4 Starter

## Overview

This starter contains the completed Week 3 layered Student Management implementation.

The application uses:

- ASP.NET Core MVC
- `StudentController`
- `StudentService`
- `StudentStorage`
- An in-memory student collection
- Constructor dependency injection

## Existing behaviour

The starter implements:

- W2-UC01 to W2-UC05
- W3-UC01 to W3-UC03

## Week 4

In Week 4, you will refactor the application into a modular monolith and introduce Academic Supervisor Management.

Read:

- `LabResources/week04-use-cases-student-revised.md`
- `LabResources/week04-domain-model-student-supervisor.md`

Do not delete the existing Student Management behaviour during the refactoring.

## Run the project

```text
dotnet build
dotnet run --project PgrStudentManagement.Web

## Data lifetime

Student records are held in memory.

Changes remain available while the application process is running and are lost when it stops or restarts.
