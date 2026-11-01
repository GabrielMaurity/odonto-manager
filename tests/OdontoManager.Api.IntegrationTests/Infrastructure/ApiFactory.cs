using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace OdontoManager.Api.IntegrationTests.Infrastructure;

public sealed class ApiFactory(
    string connectionString,
    string environment = "Production",
    IReadOnlyDictionary<string, string?>? settings = null,
    Action<IServiceCollection>? configureServices = null)
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(environment);
        builder.UseSetting("ConnectionStrings:OdontoManager", connectionString);

        foreach (var (chave, valor) in settings ?? new Dictionary<string, string?>())
        {
            builder.UseSetting(chave, valor);
        }

        if (configureServices is not null)
        {
            builder.ConfigureTestServices(configureServices);
        }
    }
}
