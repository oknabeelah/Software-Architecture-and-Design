using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PgrStudentManagement.Adapters.API.Mappers;
using PgrStudentManagement.Adapters.API.Requests;
using PgrStudentManagement.Adapters.API.Responses;
using PgrStudentManagement.Business.AcademicSupervisors.AssignAcademicSupervisor;
using PgrStudentManagement.Business.AcademicSupervisors.ViewSupervisoryTeam;

namespace PgrStudentManagement.Adapters.API.Controllers;

[ApiController]
[Route("api/students/{studentNumber}/academic-supervisors")]
public class AcademicSupervisorsApiController : ControllerBase
{
    private readonly IAssignAcademicSupervisorUseCase _assignUseCase;
    private readonly IViewSupervisoryTeamUseCase? _viewTeamUseCase;
    private readonly AcademicSupervisorsApiMapper _mapper;

    public AcademicSupervisorsApiController(
        IAssignAcademicSupervisorUseCase assignUseCase,
        AcademicSupervisorsApiMapper mapper,
        IViewSupervisoryTeamUseCase? viewTeamUseCase = null)
    {
        _assignUseCase = assignUseCase;
        _mapper = mapper;
        _viewTeamUseCase = viewTeamUseCase;
    }

    [HttpGet]
    public async Task<ActionResult<SupervisoryTeamResponse>> GetSupervisoryTeam(
        string studentNumber,
        CancellationToken cancellationToken)
    {
        if (_viewTeamUseCase is null)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                "Supervisory team use case is not configured.");
        }

        var input = new ViewSupervisoryTeamInput(studentNumber);
        var output = await _viewTeamUseCase.ExecuteAsync(input, cancellationToken);
        var response = _mapper.ToResponse(output);

        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<AssignAcademicSupervisorResponse>> AssignSupervisor(
        string studentNumber,
        [FromBody] AssignAcademicSupervisorRequest request,
        CancellationToken cancellationToken)
    {
        var input = _mapper.ToInput(studentNumber, request);
        var result = await _assignUseCase.ExecuteAsync(input, cancellationToken);
        var response = _mapper.ToResponse(result);

        return result.Outcome switch
        {
            AssignmentOutcome.Created =>
                StatusCode(StatusCodes.Status201Created, response),

            AssignmentOutcome.StudentNotFound =>
                NotFound(response),

            AssignmentOutcome.SupervisorNotFound =>
                NotFound(response),

            AssignmentOutcome.DuplicateAssignment =>
                Conflict(response),

            AssignmentOutcome.RoleAlreadyFilled =>
                Conflict(response),

            AssignmentOutcome.ExceptionRequired =>
                Conflict(response),

            AssignmentOutcome.SupervisorNotEligible =>
                UnprocessableEntity(response),

            AssignmentOutcome.EmploymentTypeNotPermitted =>
                UnprocessableEntity(response),

            _ =>
                BadRequest(response)
        };
    }
}
