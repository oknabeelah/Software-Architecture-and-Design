using Microsoft.AspNetCore.Mvc;
using PgrStudentManagement.Adapters.Web.Mappers;
using PgrStudentManagement.Business.Students.ViewStudentEnrolments;

namespace PgrStudentManagement.Adapters.Web.Controllers;

[Route("[controller]")]
public sealed class StudentsController : Controller
{
    private readonly IViewStudentEnrolmentsUseCase _viewStudentEnrolmentsUseCase;
    private readonly StudentEnrolmentsWebMapper _mapper;

    public StudentsController(IViewStudentEnrolmentsUseCase viewStudentEnrolmentsUseCase, StudentEnrolmentsWebMapper mapper)
    {
        _viewStudentEnrolmentsUseCase = viewStudentEnrolmentsUseCase;
        _mapper = mapper;
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Enrolments(string studentNumber, CancellationToken cancellationToken)
    {
        var input = new ViewStudentEnrolmentsInput(studentNumber);
        var output = await _viewStudentEnrolmentsUseCase.ExecuteAsync(input, cancellationToken);
        var viewModel = _mapper.ToViewModel(output);

        return View(viewModel);
    }
}