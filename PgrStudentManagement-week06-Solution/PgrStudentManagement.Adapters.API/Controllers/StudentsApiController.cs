using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PgrStudentManagement.Adapters.API.Mappers;
using PgrStudentManagement.Adapters.API.Responses;
using PgrStudentManagement.Business.Students.ViewStudentEnrolments;
using PgrStudentManagement.Business.Students.ViewStudentSummary;

namespace PgrStudentManagement.Adapters.API.Controllers;

[ApiController]
[Route("api/students")]
public class StudentsApiController : ControllerBase
{
    private readonly IViewStudentEnrolmentsUseCase _useCase;
    private readonly StudentEnrolmentsApiMapper _mapper;
    private readonly IViewStudentSummaryUseCase _summaryUseCase;
    private readonly StudentLookupApiMapper _summaryMapper;

    public StudentsApiController(
        IViewStudentEnrolmentsUseCase useCase,
        StudentEnrolmentsApiMapper mapper,
        IViewStudentSummaryUseCase summaryUseCase,
        StudentLookupApiMapper summaryMapper)
    {
        _useCase = useCase;
        _mapper = mapper;
        _summaryUseCase = summaryUseCase;
        _summaryMapper = summaryMapper;
    }

    [HttpGet("{studentNumber}/enrolments")]
    public async Task<ActionResult<StudentEnrolmentsResponse>> GetEnrolments(
        string studentNumber,
        CancellationToken cancellationToken)
    {
        var input = new ViewStudentEnrolmentsInput(studentNumber);
        var output = await _useCase.ExecuteAsync(input, cancellationToken);
        var response = _mapper.ToResponse(output);
        return Ok(response);
    }

    [HttpGet("{studentNumber}/lookup")]
    [ProducesResponseType<StudentLookupResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentLookupResponse>> GetLookup(
        string studentNumber,
        CancellationToken cancellationToken)
    {
        var output = await _summaryUseCase.ExecuteAsync(
            new ViewStudentSummaryInput(studentNumber),
            cancellationToken);

        if (output is null)
        {
            return NotFound();
        }

        return Ok(_summaryMapper.ToResponse(output));
    }
}
