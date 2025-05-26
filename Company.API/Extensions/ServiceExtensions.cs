using Contracts;
using LoggerService;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace Company.API.Extensions;

public static class ServiceExtensions
{
    public static void AddConfigureCors(this IServiceCollection services)
    {
        services.AddCors(op => op.AddPolicy("CorsPlicy", builder =>
            builder.AllowAnyHeader()
            .AllowAnyOrigin()
            .AllowAnyMethod()));
    }

    public static void AddConfigureIISIntegration(this IServiceCollection services)
        => services.Configure<IISOptions>(op =>
        {
        });

    public static void AddConfigureLoggerService(this IServiceCollection services)
        => services.AddSingleton<ILoggerManager, LoggerManager>();

    public static void AddConfigureRepositoryManager(this IServiceCollection services)
        => services.AddScoped<IRepositoryManager, RepositoryManager>();

    public static void AddConfigureCompanyRepository(this IServiceCollection services)
        => services.AddScoped<ICompanyRepository, CompanyRepository>();

    public static void AddConfigureEmployeeRepository(this IServiceCollection services)
        => services.AddScoped<IEmployeeRepository, EmployeeRepository>();

    public static void AddConfigureSqlContext(this IServiceCollection services,
        IConfiguration configuration)
        => services.AddDbContext<RepositoryContext>(opts =>
              opts.UseSqlServer(configuration.GetConnectionString("sqlConnection")));
}