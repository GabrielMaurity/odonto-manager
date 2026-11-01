namespace OdontoManager.Web.Client.Services;

public enum SystemStatus
{
    Disponivel,
    Indisponivel
}

public interface ISystemStatusClient
{
    Task<SystemStatus> ObterAsync(CancellationToken cancellationToken);
}

internal sealed class SystemStatusClient(HttpClient httpClient) : ISystemStatusClient
{
    public async Task<SystemStatus> ObterAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var response = await httpClient.GetAsync(new Uri("health/ready", UriKind.Relative), cancellationToken);
            return response.IsSuccessStatusCode ? SystemStatus.Disponivel : SystemStatus.Indisponivel;
        }
        catch (HttpRequestException)
        {
            return SystemStatus.Indisponivel;
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return SystemStatus.Indisponivel;
        }
    }
}
