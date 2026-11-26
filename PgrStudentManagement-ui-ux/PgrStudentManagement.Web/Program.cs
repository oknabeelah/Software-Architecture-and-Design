using Microsoft.EntityFrameworkCore;

using PgrStudentManagement.Web.Modules.Students.Contracts;
using PgrStudentManagement.Web.Modules.Students.Data;
using PgrStudentManagement.Web.Modules.Students.Data.Daos;
using PgrStudentManagement.Web.Modules.Students.Services;

using PgrStudentManagement.Web.Modules.AcademicSupervisors.Contracts;
using PgrStudentManagement.Web.Modules.AcademicSupervisors.Data;
using PgrStudentManagement.Web.Modules.AcademicSupervisors.Data.Daos;
using PgrStudentManagement.Web.Modules.AcademicSupervisors.Services;

using PgrStudentManagement.Web.Modules.PhdApplications.Data;
using PgrStudentManagement.Web.Modules.PhdApplications.Data.Daos;
using PgrStudentManagement.Web.Modules.PhdApplications.Services;

using PgrStudentManagement.Web.Modules.ResearchActivities.Data;
using PgrStudentManagement.Web.Modules.ResearchActivities.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MVC services (using native HTML validation rather than jQuery unobtrusive data-val attributes).
builder.Services.AddControllersWithViews()
    .AddViewOptions(options =>
        options.HtmlHelperOptions.ClientValidationEnabled = false);

// Configure relational persistence.
builder.Services.AddDbContext<StudentManagementDbContext>(
    options => options.UseSqlite(
        builder.Configuration.GetConnectionString(
            "StudentManagementDatabase")));

// Students module.
builder.Services.AddScoped<StudentDao>();
builder.Services.AddScoped<EnrolmentDao>();
builder.Services.AddScoped<StudentService>();

// Register StudentService as the implementation
// of the IStudentLookup contract.
builder.Services.AddScoped<IStudentLookup>(
    provider =>
        provider.GetRequiredService<StudentService>());

// Students-module seeders.
builder.Services.AddScoped<StudentDataSeeder>();
builder.Services.AddScoped<EnrolmentDataSeeder>();

// Research Activities module.
builder.Services.AddScoped<ResearchActivityRepository>();
builder.Services.AddScoped<ResearchActivityService>();

// Academic Supervisors module.
builder.Services.AddScoped<AcademicSupervisorDao>();
builder.Services.AddScoped<AcademicSupervisorService>();
builder.Services.AddScoped<ISupervisorLookup>(
    provider =>
        provider.GetRequiredService<AcademicSupervisorService>());
builder.Services.AddScoped<AcademicSupervisorDataSeeder>();

// PhD Applications module.
builder.Services.AddScoped<PhdApplicationDao>();
builder.Services.AddScoped<PhdApplicationService>();
builder.Services.AddScoped<PhdApplicationDataSeeder>();

var app = builder.Build();

// Apply EF Core migrations and seed initial data.
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;

    var context =
        serviceProvider.GetRequiredService<
            StudentManagementDbContext>();

    await context.Database.MigrateAsync();

    // Students must be seeded before Enrolments
    // because Enrolments refer to StudentNumber.
    var studentSeeder =
        serviceProvider.GetRequiredService<
            StudentDataSeeder>();

    await studentSeeder.SeedAsync();

    var enrolmentSeeder =
        serviceProvider.GetRequiredService<
            EnrolmentDataSeeder>();

    await enrolmentSeeder.SeedAsync();

    var academicSupervisorSeeder =
        serviceProvider.GetRequiredService<
            AcademicSupervisorDataSeeder>();

    await academicSupervisorSeeder.SeedAsync();

    var phdApplicationSeeder =
        serviceProvider.GetRequiredService<
            PhdApplicationDataSeeder>();

    await phdApplicationSeeder.SeedAsync();

    var researchActivityRepository =
        serviceProvider.GetRequiredService<
            ResearchActivityRepository>();

    await ResearchActivityDataSeeder.SeedAsync(
        researchActivityRepository);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

// .NET 10 static asset endpoints.
app.MapStaticAssets();

// Research Activities route.
app.MapControllerRoute(
        name: "research_activities",
        pattern:
            "ResearchActivities/{action=Index}/{id?}",
        defaults: new
        {
            controller = "ResearchActivity"
        })
    .WithStaticAssets();

// Default MVC route.
app.MapControllerRoute(
        name: "default",
        pattern:
            "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
