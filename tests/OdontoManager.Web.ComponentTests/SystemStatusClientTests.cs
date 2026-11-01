using System.Net;
using OdontoManager.Web.Client.Services;

namespace OdontoManager.Web.ComponentTests;

public class SystemStatusClientTests
{
    [Theory]
    [InlineData(HttpStatusCode.OK, SystemStatus.Disponivel)]
    [InlineData(HttpStatusCode.ServiceUnavailable, SystemStatus.Indisponivel)]
    [InlineData(HttpStatusCode.InternalServerError, SystemStatus.Indisponivel)]
    public async Task ObterAsync_ConformeStatusDaApi_RetornaDisponibilidade(HttpStatusCode statusHttp, SystemStatus esperado)
    {
        var client = CriarClient(_ => new HttpResponseMessage(statusHttp));

        var status = await client.ObterAsync(TestContext.Current.CancellationToken);

        status.Should().Be(esperado);
    }

    [Fact]
    public async Task ObterAsync_QuandoApiNaoResponde_RetornaIndisponivel()
    {
        var client = CriarClient(_ => throw new HttpRequestException("Connection refused"));

        var status = await client.ObterAsync(TestContext.Current.CancellationToken);

        status.Should().Be(SystemStatus.Indisponivel);
    }

    [Fact]
    public async Task ObterAsync_ConsultaOEndpointDeProntidao()
    {
        Uri? chamada = null;
        var client = CriarClient(request =>
        {
            chamada = request.RequestUri;
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        await client.ObterAsync(TestContext.Current.CancellationToken);

        chamada!.AbsolutePath.Should().Be("/health/ready");
    }

    private static SystemStatusClient CriarClient(Func<HttpRequestMessage, HttpResponseMessage> responder)
    {
        var httpClient = new HttpClient(new StubHandler(responder)) { BaseAddress = new Uri("http://api.local/") };
        return new SystemStatusClient(httpClient);
    }

    private sealed class StubHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(responder(request));
    }
}
