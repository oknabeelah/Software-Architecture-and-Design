namespace PgrAcademicSupervisorManagement.Web.Models;

public class SupervisoryAssignment {
        public SupervisoryRole Role { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Current { get { return !(EndDate.ToString() is null); } } // derived from evaluating whether the assignment has an end date
        public bool exceptionAuthorised { get; set; }
    }
//     A current combination of student,
// Academic Supervisor, and role must
// not be recorded more than once.