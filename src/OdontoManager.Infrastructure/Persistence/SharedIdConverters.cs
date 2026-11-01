using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OdontoManager.Domain.SharedKernel;

namespace OdontoManager.Infrastructure.Persistence;

public sealed class UsuarioIdConverter() : ValueConverter<UsuarioId, Guid>(id => id.Value, valor => new UsuarioId(valor));

public sealed class PacienteIdConverter() : ValueConverter<PacienteId, Guid>(id => id.Value, valor => new PacienteId(valor));

public sealed class DentistaIdConverter() : ValueConverter<DentistaId, Guid>(id => id.Value, valor => new DentistaId(valor));
