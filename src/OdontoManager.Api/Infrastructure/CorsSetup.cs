namespace OdontoManager.Api.Infrastructure;

internal static class CorsSetup
{
    public const string WebPolicy = "Web";

    public static IServiceCollection AddWebCors(this IServiceCollection services, IConfiguration configuration)
    {
        var origens = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

        services.AddCors(options => options.AddPolicy(WebPolicy, policy => policy
            .WithOrigins(origens)
            .AllowAnyHeader()
            .AllowAnyMethod()));

        return services;
    }
}
