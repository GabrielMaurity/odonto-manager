namespace OdontoManager.Application.Abstractions;

public interface IUnitOfWork
{
    Task SalvarAsync(CancellationToken cancellationToken);
}
