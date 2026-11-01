namespace OdontoManager.Domain.SharedKernel;

public readonly record struct DentistaId(Guid Value)
{
    public static DentistaId Novo() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();
}
