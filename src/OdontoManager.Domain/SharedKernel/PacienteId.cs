namespace OdontoManager.Domain.SharedKernel;

public readonly record struct PacienteId(Guid Value)
{
    public static PacienteId Novo() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();
}
