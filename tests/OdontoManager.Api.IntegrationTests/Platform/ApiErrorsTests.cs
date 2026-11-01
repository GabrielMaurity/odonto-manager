using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using OdontoManager.Api.Infrastructure.Modules;
using OdontoManager.Api.IntegrationTests.Infrastructure;

namespace OdontoManager.Api.IntegrationTests.Platform;

public class ApiErrorsTests(PostgresFixture postgres)
{
    private static readonly Action<IServiceCollection> ComEndpointsDeTeste =
        services => services.AddSingleton<IApiModule, TestEndpointsModule>();

    [Fact]
    public async Task RotaInexistente_Responde404EmProblemDetails()
    {
        var (response, corpo) = await GetAsync("/rota-inexistente");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");
        corpo.GetProperty("status").GetInt32().Should().Be(404);
        corpo.TryGetProperty("type", out _).Should().BeTrue();
        corpo.TryGetProperty("title", out _).Should().BeTrue();
    }

    [Theory]
    [InlineData("/api/v1/testes/erros/nao-encontrado", HttpStatusCode.NotFound, "Teste.NaoEncontrado")]
    [InlineData("/api/v1/testes/erros/conflito", HttpStatusCode.Conflict, "Teste.Conflito")]
    [InlineData("/api/v1/testes/erros/regra", (HttpStatusCode)422, "Agenda.ConsultaNaoPodeSerCancelada")]
    [InlineData("/api/v1/testes/erros/validacao", HttpStatusCode.BadRequest, "Validation")]
    public async Task ErroDaAplicacao_RespondeStatusPorTipoComCodigo(string rota, HttpStatusCode esperado, string codigo)
    {
        var (response, corpo) = await GetAsync(rota);

        response.StatusCode.Should().Be(esperado);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");
        corpo.GetProperty("status").GetInt32().Should().Be((int)esperado);
        corpo.GetProperty("code").GetString().Should().Be(codigo);
    }

    [Fact]
    public async Task ErroDeValidacao_TrazMensagensPorCampo()
    {
        var (_, corpo) = await GetAsync("/api/v1/testes/erros/validacao");

        var erros = corpo.GetProperty("errors");
        erros.GetProperty("nome").EnumerateArray().Should().HaveCount(1);
        erros.GetProperty("email").EnumerateArray().Select(e => e.GetString()).Should().BeEquivalentTo("E-mail inválido.", "E-mail muito curto.");
    }

    [Theory]
    [InlineData("/rota-inexistente")]
    [InlineData("/api/v1/testes/erros/conflito")]
    [InlineData("/api/v1/testes/excecao")]
    public async Task QualquerErro_TrazTraceId(string rota)
    {
        var (_, corpo) = await GetAsync(rota);

        corpo.GetProperty("traceId").GetString().Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Excecao_ForaDeDesenvolvimento_Responde500SemDetalhesInternos()
    {
        var (response, corpo) = await GetAsync("/api/v1/testes/excecao");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");
        var texto = corpo.GetRawText();
        texto.Should().NotContain("Falha secreta").And.NotContain("Password").And.NotContain("InvalidOperationException").And.NotContain("   at ");
    }

    [Fact]
    public async Task Excecao_EmDesenvolvimento_Responde500ComDetalhes()
    {
        var (response, corpo) = await GetAsync("/api/v1/testes/excecao", "Development");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        corpo.GetRawText().Should().Contain("InvalidOperationException");
    }

    private async Task<(HttpResponseMessage Response, JsonElement Corpo)> GetAsync(string rota, string ambiente = "Production")
    {
        await using var factory = new ApiFactory(postgres.ConnectionString, ambiente, configureServices: ComEndpointsDeTeste);
        using var client = factory.CreateClient();
        var response = await client.GetAsync(new Uri(rota, UriKind.Relative), TestContext.Current.CancellationToken);
        var corpo = JsonDocument.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)).RootElement.Clone();
        return (response, corpo);
    }
}
