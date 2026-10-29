using Microsoft.EntityFrameworkCore;
using PgrStudentManagement.Adapters.Persistence.Relational;
using PgrStudentManagement.Adapters.API.Controllers;
using PgrStudentManagement.Adapters.Persistence.Relational.Gateway;
using PgrStudentManagement.Adapters.Persistence.Relational.Mappers;
using PgrStudentManagement.Adapters.API.Mappers;
using PgrStudentManagement.Adapters.Web.Controllers;
using PgrStudentManagement.Adapters.Web.Mappers;
using PgrStudentManagement.Business.Students.Gateway;
using PgrStudentManagement.Business.Students.ViewStudentEnrolments;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<StudentManagementDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("StudentManagement")));

builder.Services.AddScoped<IViewStudentEnrolmentsUseCase, ViewStudentEnrolmentsUseCase>();
builder.Services.AddScoped<IEnrolmentGateway, EfEnrolmentGateway>();
builder.Services.AddScoped<EnrolmentPersistenceMapper>();
builder.Services.AddScoped<StudentEnrolmentsWebMapper>();
builder.Services.AddScoped<StudentEnrolmentsApiMapper>();
builder.Services
    .AddControllersWithViews()
    .AddApplicationPart(typeof(StudentsController).Assembly)
    .AddApplicationPart(typeof(StudentsApiController).Assembly);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Students}/{action=Enrolments}/{id?}");
app.MapControllers();

app.Run();
