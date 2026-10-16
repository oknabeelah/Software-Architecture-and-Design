using Microsoft.Extensions.DependencyInjection;
using PgrStudentManagement.Web.Data;
using PgrStudentManagement.Web.Services;
using PgrStudentManagement.Web.Modules.Students.Contracts;

namespace PgrStudentManagement.Web.Modules.Students;
public static class StudentModule
{
    public static IServiceCollection AddStudentsModule(this IServiceCollection services)
    {
        services.AddControllersWithViews();

        services.AddSingleton<StudentStorage>(); // use add Singleton lifetime for a in memory storage. other requests include AddScoped, AddTransient. Singleton is used here because we want to keep the same instance of StudentStorage for the entire application lifetime, so that all controllers share the same data.
        services.AddScoped<StudentService>(); // use add Scoped lifetime for individual web requests.

        services.AddScoped<IStudentLookup>(provider => provider.GetRequiredService<StudentService>());

        return services;
    }
}
