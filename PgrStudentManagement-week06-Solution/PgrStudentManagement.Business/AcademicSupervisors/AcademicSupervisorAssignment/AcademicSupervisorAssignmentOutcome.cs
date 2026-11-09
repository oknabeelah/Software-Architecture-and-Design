namespace PgrStudentManagement.Business.AcademicSupervisors.AcademicSupervisorAssignment;

public enum AcademicSupervisorAssignmentOutcome
{
    Created,
    InvalidInput,
    StudentNotFound,
    AcademicSupervisorNotFound,
    DuplicateAssignment,
    RoleAlreadyFilled,
    SupervisorNotEligible,
    EmploymentTypeNotPermitted,
    MaximumSupervisorsReached,
    ExceptionRequired
}
