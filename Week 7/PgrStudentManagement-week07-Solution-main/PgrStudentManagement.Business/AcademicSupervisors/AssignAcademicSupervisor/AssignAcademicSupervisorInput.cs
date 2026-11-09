using PgrStudentManagement.Domain.AcademicSupervisors;

namespace PgrStudentManagement.Business.AcademicSupervisors.AssignAcademicSupervisor;

public record AssignAcademicSupervisorInput(
    string StudentNumber,
    string StaffNumber,
    SupervisoryRole Role,
    DateTime StartDate,
    bool ExceptionAuthorised);

