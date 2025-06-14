using System.Text.Json.Serialization;
using AspNetCoreRateLimit;
using CompanyEmployees.API.Extensions;
using CompanyEmployees.Presentation.ActionFilters;
using Contracts;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using NLog;
using Service.DataShaping;
using Service.Extensions;
using Service.Mapping;
using Shared.DTOs;

var builder = WebApplication.CreateBuilder(args);

LogManager.Setup().LoadConfiguration(op => Path.Combine(Directory.GetCurrentDirectory(), "/nlog.config"));

builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

#region NewtoSoft config to patch request only

NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter() =>
new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson()
.Services.BuildServiceProvider()
.GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters
.OfType<NewtonsoftJsonPatchInputFormatter>().First();

#endregion NewtoSoft config to patch request only

builder.Services.ConfigureVersioning();
builder.Services.AddControllers(config =>
    {
        config.RespectBrowserAcceptHeader = true;
        config.ReturnHttpNotAcceptable = true;
        config.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
        config.CacheProfiles.Add("120SecondsDuration", new()
        {
            Duration = 120
        });
    }).AddXmlDataContractSerializerFormatters()
    .AddCustomCSVFormatter()
    .AddJsonOptions(op => op.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
    .AddApplicationPart(typeof(CompanyEmployees.Presentation.AssemblyReference).Assembly);

builder.Services.AddConfigureCors();
builder.Services.AddConfigureIISIntegration();
builder.Services.AddConfigureLoggerService();

#region Repositories

builder.Services.AddConfigureRepositoryManager();
builder.Services.AddConfigureCompanyRepository();
builder.Services.AddConfigureEmployeeRepository();

#endregion Repositories

#region Services

builder.Services.AddSingleton<MappingProfile>();

builder.Services.AddConfigureServiceManager();
builder.Services.AddConfigureCompanyServices();
builder.Services.AddConfigureEmployeeServices();

builder.Services.AddScoped<ValidationFilterAttribute>();

builder.Services.AddScoped<IDataShaper<EmployeeDto>, DataShaper<EmployeeDto>>();

builder.Services.AddResponseCaching();

builder.Services.AddHttpCacheHeaders(expirationOpt =>
{
    expirationOpt.MaxAge = 120;
    expirationOpt.CacheLocation = Marvin.Cache.Headers.CacheLocation.Public;
},
validationOpt => validationOpt.MustRevalidate = true);

builder.Services.AddMemoryCache();

builder.Services.AddConfigureRateLimitingOptions();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication();
builder.Services.AddConfigureIdentity();

#endregion Services

builder.Services.AddConfigureSqlContext(builder.Configuration);

builder.Services.AddOpenApi().AddSwaggerGen();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILoggerManager>();

app.AddConfigureExceptionHandler(logger);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseForwardedHeaders(new()
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.UseIpRateLimiting();

app.UseCors("CorsPolicy");

app.UseResponseCaching();
app.UseHttpCacheHeaders();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();