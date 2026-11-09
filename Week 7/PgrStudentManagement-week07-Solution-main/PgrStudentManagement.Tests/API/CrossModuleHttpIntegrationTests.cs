using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PgrStudentManagement.Adapters.API.AcademicSupervisors.StudentLookupClient;
using PgrStudentManagement.Adapters.API.Controllers;
using PgrStudentManagement.Adapters.API.Mappers;
using PgrStudentManagement.Adapters.API.Requests;
using PgrStudentManagement.Adapters.API.Responses;
using PgrStudentManagement.Business.AcademicSupervisors.AssignAcademicSupervisor;
using PgrStudentManagement.Business.Students.GetStudentSummary;
using PgrStudentManagement.Domain.AcademicSupervisors;
using PgrStudentManagement.Domain.Students;
using PgrStudentManagement.Tests.TestDoubles;

namespace PgrStudentManagement.Tests.API;

public class CrossModuleHttpIntegrationTests
{
    [Fact]
    public async Task AssignSupervisor_ViaHttpStudentLookupAdapter_InvokesStudentsLookupEndpointAndCreatesAssignment()
    {
        var studentGateway = new FakeStudentGateway();
        studentGateway.Add(new Student
        {
            StudentNumber = "S100001",
            FirstName = "John",
            LastName = "Doe",
            EmailAddress = "john.doe@example.test"
        });

        var studentsController = new StudentsApiController(
            new StubViewStudentEnrolmentsUseCase(),
            new StudentEnrolmentsApiMapper(),
            new GetStudentSummaryUseCase(studentGateway),
            new StudentLookupApiMapper());

        using var httpClient = CreateLoopbackClient(studentsController);
        var httpLookupAdapter = new HttpStudentLookupAdapter(httpClient);

        var supervisorGateway = new FakeAcademicSupervisorGateway();
        await supervisorGateway.AddSupervisorAsync(new AcademicSupervisor
        {
            StaffNumber = "AS1004",
            FirstName = "Avery",
            LastName = "Brown",
            EmailAddress = "avery.brown@example.test",
            EmploymentType = SupervisorEmploymentType.InstitutionEmployed,
            TrainingCompleted = true,
            EligibleRoles = new HashSet<SupervisoryRole> { SupervisoryRole.SecondSupervisor }
        });

        var assignUseCase = new AssignAcademicSupervisorUseCase(supervisorGateway, httpLookupAdapter);
        var supervisorsController = new AcademicSupervisorsApiController(
            assignUseCase,
            new AcademicSupervisorsApiMapper());

        // 1. Known student -> 201 Created
        var createdResult = await supervisorsController.AssignSupervisor(
            "S100001",
            new AssignAcademicSupervisorRequest
            {
                StaffNumber = "AS1004",
                Role = SupervisoryRole.SecondSupervisor,
                StartDate = new DateTime(2026, 10, 1),
                ExceptionAuthorised = false
            },
            CancellationToken.None);

        var createdObjectResult = Assert.IsType<ObjectResult>(createdResult.Result);
        Assert.Equal(201, createdObjectResult.StatusCode);
        var createdResponse = Assert.IsType<AssignAcademicSupervisorResponse>(createdObjectResult.Value);
        Assert.Equal("Created", createdResponse.Outcome);

        // 2. Unknown student -> Students endpoint returns 404 -> AcademicSupervisors returns 404 StudentNotFound
        var notFoundResult = await supervisorsController.AssignSupervisor(
            "UNKNOWN",
            new AssignAcademicSupervisorRequest
            {
                StaffNumber = "AS1004",
                Role = SupervisoryRole.SecondSupervisor,
                StartDate = new DateTime(2026, 10, 1),
                ExceptionAuthorised = false
            },
            CancellationToken.None);

        var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(notFoundResult.Result);
        Assert.Equal(404, notFoundObjectResult.StatusCode);
        var notFoundResponse = Assert.IsType<AssignAcademicSupervisorResponse>(notFoundObjectResult.Value);
        Assert.Equal("StudentNotFound", notFoundResponse.Outcome);
    }

    private static HttpClient CreateLoopbackClient(StudentsApiController studentsController)
    {
        var handler = new ControllerLoopbackHandler(async (request, ct) =>
        {
            var path = request.RequestUri?.AbsolutePath ?? string.Empty;
            const string prefix = "/api/students/";
            const string suffix = "/lookup";

            if (request.Method == HttpMethod.Get
                && path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                && path.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                var studentNumber = Uri.UnescapeDataString(
                    path.Substring(prefix.Length, path.Length - prefix.Length - suffix.Length));

                var actionResult = await studentsController.GetStudentLookup(studentNumber, ct);
                if (actionResult.Result is OkObjectResult ok && ok.Value is StudentLookupResponse payload)
                {
                    var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions(JsonSerializerDefaults.Web));
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json")
                    };
                }

                if (actionResult.Result is NotFoundResult)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });

        return new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5066")
        };
    }

    private sealed class ControllerLoopbackHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handler;

        public ControllerLoopbackHandler(
            Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return _handler(request, cancellationToken);
        }
    }
}

