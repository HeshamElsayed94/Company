using System.Text.Json.Serialization;
using CompanyEmployees.API.Extensions;
using Contracts;
using Microsoft.AspNetCore.HttpOverrides;
using NLog;
using Service.Extensions;

var builder = WebApplication.CreateBuilder(args);

LogManager.Setup().LoadConfiguration(op => Path.Combine(Directory.GetCurrentDirectory(), "/nlog.config"));

builder.Services.AddControllers()
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