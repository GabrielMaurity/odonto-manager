using System.Text.Json;
using FluentValidation;
using OdontoManager.Application.Abstractions.Messaging;
using OdontoManager.Domain.SharedKernel;

namespace OdontoManager.Application.Abstractions.Decorators;

internal sealed class ValidationCommandHandler<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> inner,
    IEnumerable<IValidator<TCommand>> validators)
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        var falhas = new List<FluentValidation.Results.ValidationFailure>();
        foreach (var validator in validators)
        {
            var resultado = await validator.ValidateAsync(command, cancellationToken);
            falhas.AddRange(resultado.Errors);
        }

        if (falhas.Count == 0)
        {
            return await inner.Handle(command, cancellationToken);
        }

        var erros = falhas
            .GroupBy(falha => JsonNamingPolicy.CamelCase.ConvertName(falha.PropertyName))
            .ToDictionary(grupo => grupo.Key, grupo => grupo.Select(falha => falha.ErrorMessage).Distinct().ToArray());

        return new ValidationError(erros);
    }
}
