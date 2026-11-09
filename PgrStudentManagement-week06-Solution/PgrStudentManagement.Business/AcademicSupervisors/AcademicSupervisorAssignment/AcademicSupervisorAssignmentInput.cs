using PgrStudentManagement.Domain.AcademicSupervisors;

namespace PgrStudentManagement.Business.AcademicSupervisors.AcademicSupervisorAssignment;

public record AcademicSupervisorAssignmentInput(
    string StudentNumber,
    string SupervisorStaffNumber,
    SupervisoryRole Role,
    DateTime StartDate,
    bool ExceptionAuthorised
    );