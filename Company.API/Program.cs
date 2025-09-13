using System.Text.Json.Serialization;
using Asp.Versioning.ApiExplorer;
using AspNetCoreRateLimit;
using CompanyEmployees.API;
using CompanyEmployees.API.Extensions;
using CompanyEmployees.Presentation.ActionFilters;
using Contracts;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Service.DataShaping;
using Service.Extensions;
using Service.Mapping;
using Shared.DTOs;

var builder = WebApplication.CreateBuilder(args);

LogManager.Setup().LoadConfiguration(op => Path.Combine(Directory.GetCurrentDirectory(), "/nlog.config"));

builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

builder.Services.AddConfigureVersioning();

builder.Services.AddControllers(config =>
	{
		config.RespectBrowserAcceptHeader = true;
		config.ReturnHttpNotAcceptable = true;
		//config.CacheProfiles.Add("120SecondsDuration", new()
		//{
		//    Duration = 120
		//});
	})
	.AddXmlDataContractSerializerFormatters()
	.AddCustomCSVFormatter()
	.AddJsonOptions(op => op.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
	.AddApplicationPart(typeof(CompanyEmployees.Presentation.AssemblyReference).Assembly);

builder.Services.ConfigureOptions<ConfigureJsonPatchInputFormatter>();

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
		expirationOpt.MaxAge = 60;
		expirationOpt.CacheLocation = CacheLocation.Public;
	},
	validationOpt => validationOpt.MustRevalidate = true);

builder.Services.AddMemoryCache();

builder.Services.AddConfigureRateLimitingOptions();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication();
builder.Services.AddConfigureIdentity();

builder.Services.AddConfigureAuthenticationService();

builder.Services.AddConfigureJWT(builder.Configuration);

builder.Services.AddJwtConfiguration(builder.Configuration);

#endregion Services

builder.Services.AddConfigureSqlContext(builder.Configuration);

builder.Services.AddOpenApi();
builder.Services.AddConfigureSwagger();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILoggerManager>();

app.AddConfigureExceptionHandler(logger);

app.MapOpenApi();

app.UseSwagger();

app.UseSwaggerUI(options =>
{
	var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

	foreach (var description in provider.ApiVersionDescriptions)
	{
		options.SwaggerEndpoint(
			$"/swagger/{description.GroupName}/swagger.json",
			$"CompanyEmployees.API {description.GroupName.ToUpperInvariant()}"
		);
	}
});

if (!app.Environment.IsDevelopment())
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

//app.UseResponseCaching();
app.UseHttpCacheHeaders();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();