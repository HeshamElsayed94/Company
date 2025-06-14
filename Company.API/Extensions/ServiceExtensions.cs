using System.Text;
using Asp.Versioning;
using AspNetCoreRateLimit;
using Contracts;
using Entities.Models;
using LoggerService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repository;
using Service;
using Service.Contracts;

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

    public static void AddConfigureAuthenticationService(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        services.AddScoped(provider =>
                new Lazy<IAuthenticationService>(() => provider.GetRequiredService<IAuthenticationService>()));
    }

    public static void AddConfigureSqlContext(this IServiceCollection services,
        IConfiguration configuration)
        => services.AddDbContext<RepositoryContext>(opts =>
              opts.UseSqlServer(configuration.GetConnectionString("sqlConnection")));


    public static IMvcBuilder AddCustomCSVFormatter(this IMvcBuilder builder)
        => builder.AddMvcOptions(config => config.OutputFormatters.Add(new CsvOutputFormatter()));

    public static void AddConfigureVersioning(this IServiceCollection services)
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

    public static void AddConfigureRateLimitingOptions(this IServiceCollection services)
    {
        var rateLimitRules = new List<RateLimitRule>
        {
            new()
            {
                Endpoint = "*",
                Limit = 100,
                Period = "5m"
            },
        new()
        {
            Endpoint = "*",
            Limit = 1000,
            Period = "1d"
        }
        };

        services.Configure<IpRateLimitOptions>(opt => opt.GeneralRules = rateLimitRules);

        services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

    }

    public static void AddConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole>(o =>
        {
            o.Password.RequireDigit = true;
            o.Password.RequireLowercase = false;
            o.Password.RequireUppercase = false;
            o.Password.RequireNonAlphanumeric = false;
            o.Password.RequiredLength = 6;
            o.User.RequireUniqueEmail = true;
        })
            .AddEntityFrameworkStores<RepositoryContext>()
            .AddDefaultTokenProviders();
    }

    public static void AddConfigureJWT(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");

        var secretKey = Environment.GetEnvironmentVariable("SECRET")
            ?? throw new Exception("Secret key not found");

        services.AddAuthentication(opt => opt.DefaultAuthenticateScheme =
                        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.FromSeconds(5),

                ValidIssuer = jwtSettings["validIssuer"],
                ValidAudience = jwtSettings["validAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            });

    }
}