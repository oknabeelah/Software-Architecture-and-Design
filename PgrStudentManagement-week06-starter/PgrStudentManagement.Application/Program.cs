using Microsoft.EntityFrameworkCore;
using PgrStudentManagement.Adapters.Persistence.Relational;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<StudentManagementDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("StudentManagement")));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
