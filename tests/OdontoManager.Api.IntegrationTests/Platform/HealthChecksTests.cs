using System.Net;
using OdontoManager.Api.IntegrationTests.Infrastructure;

namespace OdontoManager.Api.IntegrationTests.Platform;

public class HealthChecksTests(PostgresFixture postgres)
{
    private const string BancoInacessivel = "Host=127.0.0.1;Port=1;Database=inexistente;Username=u;Password=segredo-do-banco;Timeout=2";

    [Fact]
    public async Task Live_ComApiEmExecucao_Responde200()
    {
        var (status, _) = await GetAsync("/health/live", postgres.ConnectionString);

        status.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Live_ComBancoIndisponivel_Responde200()
    {
        var (status, _) = await GetAsync("/health/live", BancoInacessivel);

        status.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Ready_ComBancoDisponivel_Responde200SemCredenciais()
    {
        var (status, _) = await GetAsync("/health/ready", postgres.ConnectionString);

        status.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Ready_ComBancoIndisponivel_Responde503()
    {
        var (status, _) = await GetAsync("/health/ready", BancoInacessivel);

        status.Should().Be(HttpStatusCode.ServiceUnavailable);
    }

    [Fact]
    public async Task Ready_ForaDeDesenvolvimentoComFalha_NaoExpoeDetalhesInternos()
    {
        var (_, corpo) = await GetAsync("/health/ready", BancoInacessivel);

        corpo.Should().Be("Unhealthy");
        corpo.Should().NotContain("127.0.0.1").And.NotContain("segredo-do-banco").And.NotContain("Exception").And.NotContain("postgres");
    }

    [Fact]
    public async Task Ready_EmDesenvolvimentoComFalha_ExibeDiagnostico()
    {
        var (status, corpo) = await GetAsync("/health/ready", BancoInacessivel, "Development");

        status.Should().Be(HttpStatusCode.ServiceUnavailable);
        corpo.Should().Contain("\"name\":\"postgres\"").And.Contain("\"status\":\"Unhealthy\"");
    }

    private static async Task<(HttpStatusCode Status, string Corpo)> GetAsync(string rota, string connectionString, string ambiente = "Production")
    {
        await using var factory = new ApiFactory(connectionString, ambiente);
        using var client = factory.CreateClient();
        var response = await client.GetAsync(new Uri(rota, UriKind.Relative), TestContext.Current.CancellationToken);
        return (response.StatusCode, await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }
}
