using System.Diagnostics;

namespace OdontoManager.Api.Infrastructure.ErrorHandling;

internal static class ProblemDetailsSetup
{
    public static IServiceCollection AddApiProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails(options => options.CustomizeProblemDetails = context =>
        {
            context.ProblemDetails.Instance ??= context.HttpContext.Request.Path;
            context.ProblemDetails.Extensions.TryAdd("traceId", Activity.Current?.Id ?? context.HttpContext.TraceIdentifier);
        });
        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }
}
