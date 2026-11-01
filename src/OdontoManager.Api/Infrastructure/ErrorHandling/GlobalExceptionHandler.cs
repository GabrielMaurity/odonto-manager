using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace OdontoManager.Api.Infrastructure.ErrorHandling;

internal sealed partial class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    IHostEnvironment environment,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        LogExcecaoNaoTratada(logger, httpContext.Request.Method, httpContext.Request.Path, exception);

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Erro interno",
            Detail = environment.IsDevelopment() ? exception.ToString() : "Ocorreu um erro inesperado. Tente novamente mais tarde."
        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problem,
            Exception = environment.IsDevelopment() ? exception : null
        });
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Exceção não tratada em {Metodo} {Caminho}")]
    private static partial void LogExcecaoNaoTratada(ILogger logger, string metodo, string caminho, Exception exception);
}
