using Microsoft.AspNetCore.Mvc;
using PgrStudentManagement.Adapters.API.Mappers;
using PgrStudentManagement.Adapters.API.Responses;
using PgrStudentManagement.Business.Students.GetStudentSummary;
using PgrStudentManagement.Business.Students.ViewStudentEnrolments;

namespace PgrStudentManagement.Adapters.API.Controllers;

[ApiController]
[Route("api/students")]
public class StudentsApiController : ControllerBase
{
    private readonly IViewStudentEnrolmentsUseCase _useCase;
    private readonly StudentEnrolmentsApiMapper _mapper;
    private readonly IGetStudentSummaryUseCase? _lookupUseCase;
    private readonly StudentLookupApiMapper _lookupMapper;

    public StudentsApiController(
        IViewStudentEnrolmentsUseCase useCase,
        StudentEnrolmentsApiMapper mapper,
        IGetStudentSummaryUseCase? lookupUseCase = null,
        StudentLookupApiMapper? lookupMapper = null)
    {
        _useCase = useCase;
        _mapper = mapper;
        _lookupUseCase = lookupUseCase;
        _lookupMapper = lookupMapper ?? new StudentLookupApiMapper();
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
    public async Task<ActionResult<StudentLookupResponse>> GetStudentLookup(
        string studentNumber,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(studentNumber))
        {
            return BadRequest();
        }

        if (_lookupUseCase is null)
        {
            return StatusCode(500, "Student lookup use case is not configured.");
        }

        var input = _lookupMapper.ToInput(studentNumber);
        var output = await _lookupUseCase.ExecuteAsync(input, cancellationToken);
        var response = _lookupMapper.ToResponse(output);

        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }
}
