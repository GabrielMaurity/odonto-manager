using System.Net;
using System.Text.Json;
using OdontoManager.Api.IntegrationTests.Infrastructure;

namespace OdontoManager.Api.IntegrationTests.Platform;

public class ApiDocumentationTests(PostgresFixture postgres)
{
    [Fact]
    public async Task OpenApi_EmDesenvolvimento_PublicaDocumentoValido()
    {
        await using var factory = new ApiFactory(postgres.ConnectionString, "Development");
        using var client = factory.CreateClient();

        var response = await client.GetAsync(new Uri("/openapi/v1.json", UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var documento = JsonDocument.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)).RootElement;
        documento.GetProperty("openapi").GetString().Should().StartWith("3.");
        documento.TryGetProperty("paths", out _).Should().BeTrue();
    }

    [Fact]
    public async Task Scalar_EmDesenvolvimento_ServeAPaginaDeDocumentacao()
    {
        await using var factory = new ApiFactory(postgres.ConnectionString, "Development");
        using var client = factory.CreateClient();

        var response = await client.GetAsync(new Uri("/scalar", UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("text/html");
    }

    [Theory]
    [InlineData("/openapi/v1.json")]
    [InlineData("/scalar")]
    public async Task Documentacao_ForaDeDesenvolvimento_Responde404(string rota)
    {
        await using var factory = new ApiFactory(postgres.ConnectionString, "Production");
        using var client = factory.CreateClient();

        var response = await client.GetAsync(new Uri(rota, UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
