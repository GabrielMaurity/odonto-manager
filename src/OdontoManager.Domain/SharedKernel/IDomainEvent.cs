namespace OdontoManager.Domain.SharedKernel;

public interface IDomainEvent
{
    Guid Id { get; }
}
