using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OdontoManager.Api.IntegrationTests.Infrastructure;
using OdontoManager.Application.Abstractions.Clinica;

namespace OdontoManager.Api.IntegrationTests.Platform;

public class StartupTests(PostgresFixture postgres)
{
    [Fact]
    public async Task Inicializacao_ComFusoHorarioInvalido_Falha()
    {
        await using var factory = new ApiFactory(
            postgres.ConnectionString,
            settings: new Dictionary<string, string?> { ["Clinica:FusoHorario"] = "Fuso/Inexistente" });

        var acao = () => factory.CreateClient();

        acao.Should().Throw<OptionsValidationException>().WithMessage("*Fuso/Inexistente*");
    }

    [Fact]
    public async Task Inicializacao_SemConfiguracao_UsaFusoDeSaoPaulo()
    {
        await using var factory = new ApiFactory(postgres.ConnectionString);
        using var client = factory.CreateClient();

        var opcoes = factory.Services.GetRequiredService<IOptions<ClinicaOptions>>().Value;

        opcoes.ObterFusoHorario().Id.Should().Be("America/Sao_Paulo");
        factory.Services.GetRequiredService<TimeProvider>().Should().BeSameAs(TimeProvider.System);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("http://localhost:4317")]
    public async Task Inicializacao_ComOuSemEndpointOtlp_Sobe(string? endpoint)
    {
        await using var factory = new ApiFactory(
            postgres.ConnectionString,
            settings: new Dictionary<string, string?> { ["OTEL_EXPORTER_OTLP_ENDPOINT"] = endpoint });
        using var client = factory.CreateClient();

        var response = await client.GetAsync(new Uri("/health/live", UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
