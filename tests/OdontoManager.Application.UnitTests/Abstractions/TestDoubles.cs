using System.Collections.Concurrent;
using FluentValidation;
using Microsoft.Extensions.Logging;
using OdontoManager.Application.Abstractions.Messaging;
using OdontoManager.Domain.SharedKernel;

namespace OdontoManager.Application.UnitTests.Abstractions;

public sealed record CadastrarItemDeTeste(string Nome, string Email, string Documento) : ICommand<Guid>;

public sealed record ObterItemDeTeste(Guid Id) : IQuery<string>;

public sealed class RegistroDeChamadas
{
    public int Quantidade { get; private set; }

    public void Registrar() => Quantidade++;
}

internal sealed class CadastrarItemDeTesteHandler(RegistroDeChamadas chamadas) : ICommandHandler<CadastrarItemDeTeste, Guid>
{
    public Task<Result<Guid>> Handle(CadastrarItemDeTeste command, CancellationToken cancellationToken)
    {
        chamadas.Registrar();
        Result<Guid> resultado = command.Nome == "duplicado"
            ? Error.Conflict("Teste.Duplicado", "Item duplicado.")
            : Guid.CreateVersion7();
        return Task.FromResult(resultado);
    }
}

internal sealed class ObterItemDeTesteHandler : IQueryHandler<ObterItemDeTeste, string>
{
    public Task<Result<string>> Handle(ObterItemDeTeste query, CancellationToken cancellationToken) =>
        Task.FromResult<Result<string>>("item");
}

internal sealed class CadastrarItemDeTesteValidator : AbstractValidator<CadastrarItemDeTeste>
{
    public CadastrarItemDeTesteValidator()
    {
        RuleFor(c => c.Nome).NotEmpty().WithMessage("Nome é obrigatório.");
        RuleFor(c => c.Email).EmailAddress().WithMessage("E-mail inválido.");
        RuleFor(c => c.Email).MinimumLength(5).WithMessage("E-mail muito curto.");
    }
}

public sealed class CapturingLoggerProvider : ILoggerProvider
{
    public ConcurrentQueue<string> Entradas { get; } = new();

    public ILogger CreateLogger(string categoryName) => new CapturingLogger(Entradas);

    public void Dispose()
    {
    }

    private sealed class CapturingLogger(ConcurrentQueue<string> entradas) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var valores = state is IEnumerable<KeyValuePair<string, object?>> pares
                ? string.Join(";", pares.Select(par => $"{par.Key}={par.Value}"))
                : string.Empty;
            entradas.Enqueue($"{logLevel}|{formatter(state, exception)}|{valores}");
        }
    }
}
