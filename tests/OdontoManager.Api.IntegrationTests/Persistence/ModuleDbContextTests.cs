using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OdontoManager.Api.IntegrationTests.Infrastructure;
using OdontoManager.Application.Abstractions;
using OdontoManager.Infrastructure.Persistence;

namespace OdontoManager.Api.IntegrationTests.Persistence;

public sealed class ModuleDbContextTests : IAsyncLifetime
{
    private readonly PostgresFixture _postgres;
    private readonly EventosRecebidos _recebidos = new();
    private readonly ServiceProvider _provider;

    public ModuleDbContextTests(PostgresFixture postgres)
    {
        _postgres = postgres;
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["ConnectionStrings:OdontoManager"] = postgres.ConnectionString })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(_recebidos);
        services.AddSingleton(new ConnectionStringDeTeste(postgres.ConnectionString));
        services.AddScoped<IDomainEventHandler<ItemDeTesteCriado>, HandlerQueFalha>();
        services.AddScoped<IDomainEventHandler<ItemDeTesteCriado>, HandlerQueVerificaOCommit>();
        services.AddModuleDbContext<TestModuleDbContext>(configuration, TestModuleDbContext.NomeDoSchema);
        _provider = services.BuildServiceProvider();
    }

    public async ValueTask InitializeAsync()
    {
        await using var scope = _provider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<TestModuleDbContext>();
        await context.Database.ExecuteSqlRawAsync($"DROP SCHEMA IF EXISTS {TestModuleDbContext.NomeDoSchema} CASCADE");
        await context.Database.EnsureCreatedAsync();
    }

    public async ValueTask DisposeAsync() => await _provider.DisposeAsync();

    [Fact]
    public async Task Modelo_QuandoCriado_UsaSchemaDoModuloSnakeCaseEUuid()
    {
        var colunas = await ColunasAsync("itens");

        colunas.Should().Contain(("id", "uuid"))
            .And.Contain(("paciente_id", "uuid"))
            .And.Contain(("nome", "character varying"));
    }

    [Fact]
    public async Task SalvarAsync_QuandoBemSucedido_DespachaEventosAposOCommit()
    {
        var item = ItemDeTeste.Criar("item-commit");

        await SalvarAsync(item);

        _recebidos.Registros.Should().ContainSingle(r => r.ItemId == item.Id)
            .Which.LinhaExistiaNoBanco.Should().BeTrue();
        item.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public async Task SalvarAsync_QuandoFalha_NaoDespachaEventos()
    {
        await SalvarAsync(ItemDeTeste.Criar("item-duplicado"));
        var duplicado = ItemDeTeste.Criar("item-duplicado");

        var acao = () => SalvarAsync(duplicado);

        await acao.Should().ThrowAsync<DbUpdateException>();
        _recebidos.Registros.Should().NotContain(r => r.ItemId == duplicado.Id);
        duplicado.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public async Task SalvarAsync_QuandoUmHandlerFalha_OsDemaisSaoExecutados()
    {
        var item = ItemDeTeste.Criar("item-handler-falha");

        var acao = () => SalvarAsync(item);

        await acao.Should().NotThrowAsync();
        _recebidos.Registros.Should().Contain(r => r.ItemId == item.Id && r.Handler == nameof(HandlerQueVerificaOCommit));
    }

    private async Task SalvarAsync(ItemDeTeste item)
    {
        await using var scope = _provider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<TestModuleDbContext>();
        context.Itens.Add(item);
        await ((IUnitOfWork)context).SalvarAsync(TestContext.Current.CancellationToken);
    }

    private async Task<List<(string Nome, string Tipo)>> ColunasAsync(string tabela)
    {
        await using var connection = new NpgsqlConnection(_postgres.ConnectionString);
        await connection.OpenAsync(TestContext.Current.CancellationToken);
        await using var command = new NpgsqlCommand(
            "SELECT column_name, data_type FROM information_schema.columns WHERE table_schema = @schema AND table_name = @tabela",
            connection);
        command.Parameters.AddWithValue("schema", TestModuleDbContext.NomeDoSchema);
        command.Parameters.AddWithValue("tabela", tabela);

        var colunas = new List<(string, string)>();
        await using var reader = await command.ExecuteReaderAsync(TestContext.Current.CancellationToken);
        while (await reader.ReadAsync(TestContext.Current.CancellationToken))
        {
            colunas.Add((reader.GetString(0), reader.GetString(1)));
        }

        return colunas;
    }
}
