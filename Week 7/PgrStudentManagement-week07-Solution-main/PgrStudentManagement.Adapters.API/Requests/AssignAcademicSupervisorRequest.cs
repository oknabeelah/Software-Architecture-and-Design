using System.Text.Json.Serialization;
using PgrStudentManagement.Domain.AcademicSupervisors;

namespace PgrStudentManagement.Adapters.API.Requests;

public class AssignAcademicSupervisorRequest
{
    public string? StudentNumber { get; init; }

    public string StaffNumber { get; init; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SupervisoryRole Role { get; init; }

    public DateTime StartDate { get; init; }

    public bool ExceptionAuthorised { get; init; }
}

