using Microsoft.EntityFrameworkCore;
using OdontoManager.Application.Abstractions;
using OdontoManager.Domain.SharedKernel;

namespace OdontoManager.Infrastructure.Persistence;

public abstract class ModuleDbContext(DbContextOptions options, string schema) : DbContext(options), IUnitOfWork
{
    public string Schema { get; } = schema;

    public Task SalvarAsync(CancellationToken cancellationToken) => SaveChangesAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.HasDefaultSchema(Schema);
        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);
        configurationBuilder.IgnoreAny<IDomainEvent>();
        configurationBuilder.Properties<UsuarioId>().HaveConversion<UsuarioIdConverter>();
        configurationBuilder.Properties<PacienteId>().HaveConversion<PacienteIdConverter>();
        configurationBuilder.Properties<DentistaId>().HaveConversion<DentistaIdConverter>();
        base.ConfigureConventions(configurationBuilder);
    }
}
