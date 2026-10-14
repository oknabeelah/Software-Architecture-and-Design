using Microsoft.Extensions.DependencyInjection;
using PgrAcademicSupervisorManagement.Web.Data;
using PgrAcademicSupervisorManagement.Web.Services;

namespace PgrStudentManagement.Web.Modules.AcademicSupervisors;
public static class AcademicSupervisorsModule
{
    public static IServiceCollection AddAcademicSupervisorsModule(this IServiceCollection services)
    {
        services.AddSingleton<AcademicSupervisorStorage>();
        services.AddScoped<AcademicSupervisorService>();

        return services;
    }
}

// The "this IServiceCollection services" parameter makes this an extension method. 
// Returning IServiceCollection allows it to be combined with other registration methods.