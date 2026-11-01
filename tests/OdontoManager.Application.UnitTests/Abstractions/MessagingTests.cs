using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OdontoManager.Application.Abstractions.Messaging;
using OdontoManager.Domain.SharedKernel;

namespace OdontoManager.Application.UnitTests.Abstractions;

public sealed class MessagingTests : IDisposable
{
    private readonly CapturingLoggerProvider _logs = new();
    private readonly RegistroDeChamadas _chamadas = new();
    private readonly ServiceProvider _provider;

    public MessagingTests()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddProvider(_logs).SetMinimumLevel(LogLevel.Trace));
        services.AddSingleton(_chamadas);
        services.AddApplication(typeof(MessagingTests).Assembly);
        _provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
    }

    [Fact]
    public async Task CommandHandler_ResolvidoPelaInterface_ExecutaOHandler()
    {
        using var scope = _provider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<CadastrarItemDeTeste, Guid>>();

        var resultado = await handler.Handle(new CadastrarItemDeTeste("Item", "item@example.com", "123"), CancellationToken.None);

        resultado.IsSuccess.Should().BeTrue();
        _chamadas.Quantidade.Should().Be(1);
    }

    [Fact]
    public async Task QueryHandler_ResolvidoPelaInterface_ExecutaOHandler()
    {
        using var scope = _provider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IQueryHandler<ObterItemDeTeste, string>>();

        var resultado = await handler.Handle(new ObterItemDeTeste(Guid.CreateVersion7()), CancellationToken.None);

        resultado.Value.Should().Be("item");
        _logs.Entradas.Should().Contain(entrada => entrada.Contains("ObterItemDeTeste concluída", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Validacao_QuandoComandoInvalido_RetornaValidationErrorPorCampoSemChamarOHandler()
    {
        using var scope = _provider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<CadastrarItemDeTeste, Guid>>();

        var resultado = await handler.Handle(new CadastrarItemDeTeste("", "x@", "123"), CancellationToken.None);

        _chamadas.Quantidade.Should().Be(0);
        var erro = resultado.Error.Should().BeOfType<ValidationError>().Subject;
        erro.Errors.Keys.Should().BeEquivalentTo("nome", "email");
        erro.Errors["email"].Should().BeEquivalentTo("E-mail inválido.", "E-mail muito curto.");
    }

    [Fact]
    public async Task Logging_QuandoComandoFalha_RegistraOCodigoDoErro()
    {
        using var scope = _provider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<CadastrarItemDeTeste, Guid>>();

        await handler.Handle(new CadastrarItemDeTeste("duplicado", "item@example.com", "123"), CancellationToken.None);

        _logs.Entradas.Should().Contain(entrada =>
            entrada.StartsWith("Warning|", StringComparison.Ordinal) && entrada.Contains("Teste.Duplicado", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Logging_QuandoComandoExecutado_NaoRegistraOConteudoDoComando()
    {
        using var scope = _provider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<CadastrarItemDeTeste, Guid>>();

        await handler.Handle(new CadastrarItemDeTeste("Maria da Silva", "maria@example.com", "52998224725"), CancellationToken.None);

        _logs.Entradas.Should().NotBeEmpty();
        _logs.Entradas.Should().NotContain(entrada =>
            entrada.Contains("Maria", StringComparison.Ordinal)
            || entrada.Contains("maria@example.com", StringComparison.Ordinal)
            || entrada.Contains("52998224725", StringComparison.Ordinal));
    }

    [Fact]
    public void AddApplication_SemHandlers_NaoFalha()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        var acao = () => services.AddApplication().BuildServiceProvider();

        acao.Should().NotThrow();
    }

    public void Dispose()
    {
        _provider.Dispose();
        _logs.Dispose();
    }
}
