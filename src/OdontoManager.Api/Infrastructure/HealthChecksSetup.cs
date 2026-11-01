using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OdontoManager.Infrastructure.Persistence;

namespace OdontoManager.Api.Infrastructure;

internal static class HealthChecksSetup
{
    private const string ReadyTag = "ready";

    public static IServiceCollection AddApiHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(PersistenceServiceCollectionExtensions.ConnectionStringName)
            ?? throw new InvalidOperationException(
                $"A connection string '{PersistenceServiceCollectionExtensions.ConnectionStringName}' não foi configurada.");

        services.AddHealthChecks().AddNpgSql(connectionString, name: "postgres", tags: [ReadyTag]);

        return services;
    }

    public static IEndpointRouteBuilder MapApiHealthChecks(this IEndpointRouteBuilder endpoints, IHostEnvironment environment)
    {
        var writer = environment.IsDevelopment() ? WriteDetailedAsync : (Func<HttpContext, HealthReport, Task>?)null;

        endpoints.MapHealthChecks("/health/live", Options(_ => false, writer)).AllowAnonymous();
        endpoints.MapHealthChecks("/health/ready", Options(check => check.Tags.Contains(ReadyTag), writer)).AllowAnonymous();

        return endpoints;
    }

    private static HealthCheckOptions Options(Func<HealthCheckRegistration, bool> predicate, Func<HttpContext, HealthReport, Task>? writer)
    {
        var options = new HealthCheckOptions { Predicate = predicate };
        if (writer is not null)
        {
            options.ResponseWriter = writer;
        }

        return options;
    }

    private static Task WriteDetailedAsync(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsync(JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description ?? entry.Value.Exception?.Message
            })
        }));
    }
}
