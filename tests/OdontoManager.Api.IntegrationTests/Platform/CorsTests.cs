using Microsoft.Extensions.DependencyInjection;
using OdontoManager.Api.Infrastructure.Modules;
using OdontoManager.Api.IntegrationTests.Infrastructure;

namespace OdontoManager.Api.IntegrationTests.Platform;

public class CorsTests(PostgresFixture postgres)
{
    private const string OrigemDaWeb = "http://localhost:5090";

    [Fact]
    public async Task Preflight_DeOrigemPermitida_RetornaCabecalhoDeCors()
    {
        var response = await PreflightAsync(OrigemDaWeb);

        response.Headers.GetValues("Access-Control-Allow-Origin").Should().ContainSingle().Which.Should().Be(OrigemDaWeb);
    }

    [Fact]
    public async Task Preflight_DeOrigemNaoPermitida_NaoRetornaCabecalhoDeCors()
    {
        var response = await PreflightAsync("https://site-malicioso.example");

        response.Headers.Contains("Access-Control-Allow-Origin").Should().BeFalse();
    }

    private async Task<HttpResponseMessage> PreflightAsync(string origem)
    {
        await using var factory = new ApiFactory(
            postgres.ConnectionString,
            settings: new Dictionary<string, string?> { ["Cors:AllowedOrigins:0"] = OrigemDaWeb },
            configureServices: services => services.AddSingleton<IApiModule, TestEndpointsModule>());
        using var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Options, new Uri("/api/v1/testes/ok", UriKind.Relative));
        request.Headers.Add("Origin", origem);
        request.Headers.Add("Access-Control-Request-Method", "GET");

        return await client.SendAsync(request, TestContext.Current.CancellationToken);
    }
}
