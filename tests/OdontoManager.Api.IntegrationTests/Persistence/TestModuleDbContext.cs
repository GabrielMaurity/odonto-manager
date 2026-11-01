using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OdontoManager.Application.Abstractions;
using OdontoManager.Domain.SharedKernel;
using OdontoManager.Infrastructure.Persistence;

namespace OdontoManager.Api.IntegrationTests.Persistence;

public readonly record struct ItemDeTesteId(Guid Value);

public sealed record ItemDeTesteCriado(Guid Id, ItemDeTesteId ItemId) : IDomainEvent;

public sealed class ItemDeTeste : AggregateRoot<ItemDeTesteId>
{
    private ItemDeTeste()
    {
    }

    public string Nome { get; private set; } = "";

    public PacienteId PacienteId { get; private set; }

    public static ItemDeTeste Criar(string nome)
    {
        var item = new ItemDeTeste { Id = new ItemDeTesteId(Guid.CreateVersion7()), Nome = nome, PacienteId = PacienteId.Novo() };
        item.Raise(new ItemDeTesteCriado(Guid.CreateVersion7(), item.Id));
        return item;
    }
}

public sealed class TestModuleDbContext(DbContextOptions<TestModuleDbContext> options) : ModuleDbContext(options, NomeDoSchema)
{
    public const string NomeDoSchema = "teste_modulo";

    public DbSet<ItemDeTeste> Itens => Set<ItemDeTeste>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ItemDeTeste>(item =>
        {
            item.HasKey(i => i.Id);
            item.Property(i => i.Id).HasConversion(new ValueConverter<ItemDeTesteId, Guid>(id => id.Value, valor => new ItemDeTesteId(valor)));
            item.Property(i => i.Nome).HasMaxLength(100);
            item.HasIndex(i => i.Nome).IsUnique();
        });
    }
}

public sealed class EventosRecebidos
{
    public ConcurrentQueue<(string Handler, ItemDeTesteId ItemId, bool LinhaExistiaNoBanco)> Registros { get; } = new();
}

public sealed class HandlerQueFalha : IDomainEventHandler<ItemDeTesteCriado>
{
    public Task Handle(ItemDeTesteCriado domainEvent, CancellationToken cancellationToken) =>
        throw new InvalidOperationException("Falha proposital do handler.");
}

public sealed class HandlerQueVerificaOCommit(EventosRecebidos recebidos, ConnectionStringDeTeste conexao) : IDomainEventHandler<ItemDeTesteCriado>
{
    public async Task Handle(ItemDeTesteCriado domainEvent, CancellationToken cancellationToken)
    {
        await using var connection = new Npgsql.NpgsqlConnection(conexao.Valor);
        await connection.OpenAsync(cancellationToken);
        await using var command = new Npgsql.NpgsqlCommand($"SELECT COUNT(*) FROM {TestModuleDbContext.NomeDoSchema}.itens WHERE id = @id", connection);
        command.Parameters.AddWithValue("id", domainEvent.ItemId.Value);
        var existe = (long)(await command.ExecuteScalarAsync(cancellationToken))! > 0;

        recebidos.Registros.Enqueue((nameof(HandlerQueVerificaOCommit), domainEvent.ItemId, existe));
    }
}

public sealed record ConnectionStringDeTeste(string Valor);
