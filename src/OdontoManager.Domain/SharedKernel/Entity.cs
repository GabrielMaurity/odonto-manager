namespace OdontoManager.Domain.SharedKernel;

public abstract class Entity<TId>
    where TId : struct
{
    public TId Id { get; protected init; }
}
