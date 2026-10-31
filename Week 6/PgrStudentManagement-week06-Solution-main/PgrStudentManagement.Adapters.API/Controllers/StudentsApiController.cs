using Microsoft.AspNetCore.Mvc;
using PgrStudentManagement.Adapters.API.Mappers;
using PgrStudentManagement.Adapters.API.Responses;
using PgrStudentManagement.Business.Students.ViewStudentEnrolments;

namespace PgrStudentManagement.Adapters.API.Controllers;

[ApiController]
[Route("api/students")]
public class StudentsApiController : ControllerBase
{
    private readonly IViewStudentEnrolmentsUseCase _useCase;
    private readonly StudentEnrolmentsApiMapper _mapper;

    public StudentsApiController(
        IViewStudentEnrolmentsUseCase useCase,
        StudentEnrolmentsApiMapper mapper)
    {
        _useCase = useCase;
        _mapper = mapper;
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
}
