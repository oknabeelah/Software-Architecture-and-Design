using Microsoft.AspNetCore.Mvc;
using PgrStudentManagement.Adapters.API.Mappers;
using PgrStudentManagement.Adapters.API.Responses;
using PgrStudentManagement.Business.Students.ViewStudentEnrolments;

namespace PgrStudentManagement.Adapters.API.Controllers;

[ApiController]
[Route("api/students")]
public sealed class StudentsApiController : ControllerBase
{
    private readonly IViewStudentEnrolmentsUseCase _viewStudentEnrolmentsUseCase;
    private readonly StudentEnrolmentsApiMapper _mapper;

    public StudentsApiController(
        IViewStudentEnrolmentsUseCase viewStudentEnrolmentsUseCase,
        StudentEnrolmentsApiMapper mapper)
    {
        _viewStudentEnrolmentsUseCase = viewStudentEnrolmentsUseCase;
        _mapper = mapper;
    }

    [HttpGet("{studentNumber}/enrolments")]
    public async Task<ActionResult<StudentEnrolmentsResponse>> GetEnrolments(
        string studentNumber,
        CancellationToken cancellationToken)
    {
        var input = new ViewStudentEnrolmentsInput(studentNumber);
        var output = await _viewStudentEnrolmentsUseCase.ExecuteAsync(input, cancellationToken);

        return Ok(_mapper.ToResponse(output));
    }
}
