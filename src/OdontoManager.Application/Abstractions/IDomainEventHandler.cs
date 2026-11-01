using System.Diagnostics.CodeAnalysis;
using OdontoManager.Domain.SharedKernel;

namespace OdontoManager.Application.Abstractions;

[SuppressMessage("Naming", "CA1711", Justification = "Nome definido na specification 3.5; não é um delegate de evento .NET.")]
public interface IDomainEventHandler<in TEvent>
    where TEvent : IDomainEvent
{
    Task Handle(TEvent domainEvent, CancellationToken cancellationToken);
}
