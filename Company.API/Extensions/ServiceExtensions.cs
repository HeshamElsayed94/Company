using Asp.Versioning;
using Contracts;
using LoggerService;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace CompanyEmployees.API.Extensions;

public static class ServiceExtensions
{
    public static void AddConfigureCors(this IServiceCollection services)
    {
        services.AddCors(op => op.AddPolicy("CorsPlicy", builder =>
            builder.AllowAnyHeader()
            .WithExposedHeaders("X-Pagination")
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
    {
        services.AddScoped<ICompanyRepository, CompanyRepository>();

        services.AddScoped(provider =>
                new Lazy<ICompanyRepository>(() => provider.GetRequiredService<ICompanyRepository>()));
    }

    public static void AddConfigureEmployeeRepository(this IServiceCollection services)
    {
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        services.AddScoped(provider =>
                new Lazy<IEmployeeRepository>(() => provider.GetRequiredService<IEmployeeRepository>()));
    }

    public static void AddConfigureSqlContext(this IServiceCollection services,
        IConfiguration configuration)
        => services.AddDbContext<RepositoryContext>(opts =>
              opts.UseSqlServer(configuration.GetConnectionString("sqlConnection")));


    public static IMvcBuilder AddCustomCSVFormatter(this IMvcBuilder builder)
        => builder.AddMvcOptions(config => config.OutputFormatters.Add(new CsvOutputFormatter()));

    public static void ConfigureVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(opt =>
        {
            opt.ReportApiVersions = true;
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.DefaultApiVersion = new ApiVersion(1, 0);
            opt.ApiVersionReader = ApiVersionReader.Combine(new HeaderApiVersionReader("api-version"),
                //new QueryStringApiVersionReader("api-version"),
                new MediaTypeApiVersionReader("api-version"));
        }).AddMvc()
        .AddApiExplorer(op =>
        {
            op.GroupNameFormat = "'v'V";
            op.SubstituteApiVersionInUrl = true;
        });
    }
}