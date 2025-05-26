using Microsoft.Extensions.DependencyInjection;
using Service.Contracts;

namespace Service.Extensions;

public static class ServiceExtensions
{
    public static void AddConfigureServiceManager(this IServiceCollection services)
       => services.AddScoped<IServiceManger, ServiceManager>();

    public static void AddConfigureCompanyServices(this IServiceCollection services)
        => services.AddScoped<ICompanyService, CompanyService>();

    public static void AddConfigureEmployeeServices(this IServiceCollection services)
        => services.AddScoped<IEmployeeService, EmployeeService>();
}