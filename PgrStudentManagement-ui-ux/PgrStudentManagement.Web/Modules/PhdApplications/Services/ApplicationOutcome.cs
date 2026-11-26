namespace PgrStudentManagement.Web.Modules.PhdApplications.Services;

public enum ApplicationOutcome
{
    Created,
    Updated,
    DuplicateReference,
    ApplicationNotFound,
    SupervisorNotFound,
    SupervisorNotEligible,
    StudentNotFound,
    InvalidStatusTransition,
    IncompleteApplication
}
