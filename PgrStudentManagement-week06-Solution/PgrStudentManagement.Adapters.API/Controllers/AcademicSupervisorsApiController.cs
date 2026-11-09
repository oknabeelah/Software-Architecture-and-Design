using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PgrStudentManagement.Adapters.API.Mappers;
using PgrStudentManagement.Adapters.API.Requests;
using PgrStudentManagement.Adapters.API.Responses;
using PgrStudentManagement.Business.AcademicSupervisors.AcademicSupervisorAssignment;

namespace PgrStudentManagement.Adapters.API.Controllers;

[ApiController]
[Route("api/students/{studentNumber}/academic-supervisors")]
public sealed class AcademicSupervisorsApiController : ControllerBase
{
    private readonly IAcademicSupervisorUseCase _useCase;
    private readonly AcademicSupervisorApiMapper _mapper;

    public AcademicSupervisorsApiController(
        IAcademicSupervisorUseCase useCase,
        AcademicSupervisorApiMapper mapper)
    {
        _useCase = useCase;
        _mapper = mapper;
    }

    [HttpPost]
    [ProducesResponseType<AssignAcademicSupervisorResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<AssignAcademicSupervisorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<AssignAcademicSupervisorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<AssignAcademicSupervisorResponse>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<AssignAcademicSupervisorResponse>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<AssignAcademicSupervisorResponse>> Assign(
        string studentNumber,
        [FromBody] AssignAcademicSupervisorRequest request,
        CancellationToken cancellationToken)
    {
        var input = _mapper.ToInput(studentNumber, request);

        if (input is null)
        {
            return BadRequest(new AssignAcademicSupervisorResponse(
                false,
                AcademicSupervisorAssignmentOutcome.InvalidInput.ToString()));
        }

        var result = await _useCase.ExecuteAsync(input, cancellationToken);
        var response = _mapper.ToResponse(result);

        return result.Outcome switch
        {
            AcademicSupervisorAssignmentOutcome.Created =>
                StatusCode(StatusCodes.Status201Created, response),
            AcademicSupervisorAssignmentOutcome.StudentNotFound
                or AcademicSupervisorAssignmentOutcome.AcademicSupervisorNotFound =>
                NotFound(response),
            AcademicSupervisorAssignmentOutcome.DuplicateAssignment
                or AcademicSupervisorAssignmentOutcome.RoleAlreadyFilled
                or AcademicSupervisorAssignmentOutcome.MaximumSupervisorsReached
                or AcademicSupervisorAssignmentOutcome.ExceptionRequired =>
                Conflict(response),
            AcademicSupervisorAssignmentOutcome.SupervisorNotEligible
                or AcademicSupervisorAssignmentOutcome.EmploymentTypeNotPermitted =>
                UnprocessableEntity(response),
            _ => BadRequest(response)
        };
    }
}
