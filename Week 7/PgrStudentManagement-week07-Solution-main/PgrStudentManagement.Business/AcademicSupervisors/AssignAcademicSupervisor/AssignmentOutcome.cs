namespace PgrStudentManagement.Business.AcademicSupervisors.AssignAcademicSupervisor;

public enum AssignmentOutcome
{
    Created,
    StudentNotFound,
    SupervisorNotFound,
    DuplicateAssignment,
    RoleAlreadyFilled,
    SupervisorNotEligible,
    EmploymentTypeNotPermitted,
    ExceptionRequired
}

