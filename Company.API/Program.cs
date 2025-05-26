using Company.API.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using NLog;

var builder = WebApplication.CreateBuilder(args);

LogManager.Setup().LoadConfiguration(op => Path.Combine(Directory.GetCurrentDirectory(), "/nlog.config"));

builder.Services.AddControllers();

builder.Services.AddConfigureCors();
builder.Services.AddConfigureIISIntegration();
builder.Services.AddConfigureLoggerService();
builder.Services.AddConfigureRepositoryManager();
builder.Services.AddConfigureCompanyRepository();
builder.Services.AddConfigureEmployeeRepository();
builder.Services.AddConfigureServiceManager();
builder.Services.AddConfigureCompanyServices();
builder.Services.AddConfigureEmployeeServices();
builder.Services.AddConfigureSqlContext(builder.Configuration);

builder.Services.AddOpenApi().AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
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