using PgrStudentManagement.Web.Modules.AcademicSupervisors;
using PgrStudentManagement.Web.Modules.Students;
using PgrStudentManagement.Web.Data;
using PgrResearchActivity.Web.Data;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<StudentManagementDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("StudentManagementDatabase")));

builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(builder.Configuration["ResearchActivityDatabase:ConnectionString"]));
builder.Services.AddScoped<ResearchActivityRepository>();

builder.Services
     .AddStudentsModule()
     .AddAcademicSupervisorsModule(); // custom service extension method

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<StudentManagementDbContext>(); //StudentManagementDbContext is registered as a scoped dependency
    await context.Database.MigrateAsync(); // applies any outstanding migrations before the seed operation runs
    await StudentDataSeeder.SeedAsync(context);

    var researchActivityRepository = scope.ServiceProvider.GetRequiredService<ResearchActivityRepository>();
    await ResearchActivityDataSeeder.SeedAsync(researchActivityRepository);
} // run the seeder during application startup
// Do not use EnsureCreated() in this project because the database schema is being managed through Entity Framework Core migrations.

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();