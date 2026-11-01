using System.Diagnostics;
using Microsoft.Extensions.Logging;
using OdontoManager.Application.Abstractions.Messaging;
using OdontoManager.Domain.SharedKernel;

namespace OdontoManager.Application.Abstractions.Decorators;

internal sealed class LoggingCommandHandler<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> inner,
    ILogger<LoggingCommandHandler<TCommand, TResponse>> logger)
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        var inicio = Stopwatch.GetTimestamp();
        var resultado = await inner.Handle(command, cancellationToken);
        OperationLogging.Registrar(logger, typeof(TCommand).Name, resultado, Stopwatch.GetElapsedTime(inicio));
        return resultado;
    }
}

internal sealed class LoggingQueryHandler<TQuery, TResponse>(
    IQueryHandler<TQuery, TResponse> inner,
    ILogger<LoggingQueryHandler<TQuery, TResponse>> logger)
    : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
    {
        var inicio = Stopwatch.GetTimestamp();
        var resultado = await inner.Handle(query, cancellationToken);
        OperationLogging.Registrar(logger, typeof(TQuery).Name, resultado, Stopwatch.GetElapsedTime(inicio));
        return resultado;
    }
}

internal static class OperationLogging
{
    public static void Registrar(ILogger logger, string operacao, Result resultado, TimeSpan duracao)
    {
        if (resultado.IsSuccess)
        {
            ApplicationLog.OperacaoConcluida(logger, operacao, duracao.TotalMilliseconds);
        }
        else
        {
            ApplicationLog.OperacaoFalhou(logger, operacao, resultado.Error.Code, duracao.TotalMilliseconds);
        }
    }
}
