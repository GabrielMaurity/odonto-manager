using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace OdontoManager.Api.IntegrationTests.Infrastructure;

public sealed class ApiFactory(string connectionString, string environment = "Production")
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(environment);
        builder.UseSetting("ConnectionStrings:OdontoManager", connectionString);
    }
}
