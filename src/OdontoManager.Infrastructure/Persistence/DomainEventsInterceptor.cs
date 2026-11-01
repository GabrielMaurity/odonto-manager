using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OdontoManager.Application.Abstractions;
using OdontoManager.Domain.SharedKernel;

namespace OdontoManager.Infrastructure.Persistence;

internal sealed partial class DomainEventsInterceptor(
    IServiceProvider serviceProvider,
    ILogger<DomainEventsInterceptor> logger) : SaveChangesInterceptor
{
    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        DispatchAsync(eventData.Context, CancellationToken.None).GetAwaiter().GetResult();
        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        await DispatchAsync(eventData.Context, cancellationToken);
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private async Task DispatchAsync(DbContext? context, CancellationToken cancellationToken)
    {
        if (context is null)
        {
            return;
        }

        var aggregates = context.ChangeTracker.Entries<IAggregateRoot>()
            .Select(entry => entry.Entity)
            .Where(aggregate => aggregate.DomainEvents.Count > 0)
            .ToList();

        var domainEvents = aggregates.SelectMany(aggregate => aggregate.DomainEvents).ToList();
        aggregates.ForEach(aggregate => aggregate.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await DispatchAsync(domainEvent, cancellationToken);
        }
    }

    private async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
        var handle = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.Handle))!;

        foreach (var handler in serviceProvider.GetServices(handlerType))
        {
            try
            {
                await (Task)handle.Invoke(handler, [domainEvent, cancellationToken])!;
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                var falha = exception is System.Reflection.TargetInvocationException { InnerException: { } inner } ? inner : exception;
                LogFalhaNoHandler(logger, domainEvent.GetType().Name, domainEvent.Id, handler!.GetType().Name, falha);
            }
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Falha ao tratar o evento {Evento} ({EventoId}) no handler {Handler}")]
    private static partial void LogFalhaNoHandler(ILogger logger, string evento, Guid eventoId, string handler, Exception exception);
}
