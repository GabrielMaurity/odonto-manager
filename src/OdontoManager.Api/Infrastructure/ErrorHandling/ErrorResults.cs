using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OdontoManager.Domain.SharedKernel;

namespace OdontoManager.Api.Infrastructure.ErrorHandling;

public static class ErrorResults
{
    public static ProblemHttpResult ToProblem(this Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        var (status, title) = error.Type switch
        {
            ErrorType.Validation => (StatusCodes.Status400BadRequest, "Requisição inválida"),
            ErrorType.NotFound => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
            ErrorType.Conflict => (StatusCodes.Status409Conflict, "Conflito"),
            ErrorType.BusinessRule => (StatusCodes.Status422UnprocessableEntity, "Regra de negócio violada"),
            _ => throw new ArgumentOutOfRangeException(nameof(error), error.Type, "Tipo de erro sem mapeamento HTTP.")
        };

        ProblemDetails problem = error is ValidationError validation
            ? new HttpValidationProblemDetails(validation.Errors.ToDictionary(par => par.Key, par => par.Value))
            : new ProblemDetails();

        problem.Status = status;
        problem.Title = title;
        problem.Detail = error.Message;
        problem.Extensions["code"] = error.Code;

        return TypedResults.Problem(problem);
    }
}
