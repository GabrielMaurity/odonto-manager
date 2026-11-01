namespace OdontoManager.Domain.SharedKernel;

public readonly record struct UsuarioId(Guid Value)
{
    public static UsuarioId Novo() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();
}
