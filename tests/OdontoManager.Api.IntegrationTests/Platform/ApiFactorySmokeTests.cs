using System.Net;
using OdontoManager.Api.IntegrationTests.Infrastructure;

namespace OdontoManager.Api.IntegrationTests.Platform;

public class ApiFactorySmokeTests(PostgresFixture postgres)
{
    [Fact]
    public async Task Api_QuandoRotaNaoExiste_Responde404()
    {
        await using var factory = new ApiFactory(postgres.ConnectionString);
        using var client = factory.CreateClient();

        var response = await client.GetAsync(new Uri("/rota-inexistente", UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ResetDatabase_QuandoBancoSemTabelas_NaoFalha()
    {
        var reset = postgres.ResetDatabaseAsync;

        await reset.Should().NotThrowAsync();
    }
}
