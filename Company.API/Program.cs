using System.Text.Json.Serialization;
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


builder.Services.AddControllers(config =>
    {
        config.RespectBrowserAcceptHeader = true;
        config.ReturnHttpNotAcceptable = true;
        config.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
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

builder.Services.AddConfigureServiceManager();
builder.Services.AddConfigureCompanyServices();
builder.Services.AddConfigureEmployeeServices();

builder.Services.AddScoped<ValidationFilterAttribute>();

builder.Services.AddScoped<IDataShaper<EmployeeDto>, DataShaper<EmployeeDto>>();

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

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();