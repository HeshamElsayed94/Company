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

    public static void AddConfigureIISIntegration(this IServiceCollection services) =>
        services.Configure<IISOptions>(op =>
        {
        });
}
