using Bunit;
using OdontoManager.Web.Client.Pages;
using OdontoManager.Web.Client.Services;

namespace OdontoManager.Web.ComponentTests;

public class HomeTests : WebShellTestContext
{
    [Fact]
    public void Home_ComApiDisponivel_IndicaSistemaDisponivel()
    {
        DefinirStatus(SystemStatus.Disponivel);

        var pagina = Render<Home>();

        pagina.WaitForAssertion(() => pagina.Find(".status-do-sistema").TextContent.Should().Contain("Sistema disponível"));
    }

    [Fact]
    public void Home_ComApiIndisponivel_IndicaSistemaIndisponivelSemDetalhesTecnicos()
    {
        DefinirStatus(SystemStatus.Indisponivel);

        var pagina = Render<Home>();

        pagina.WaitForAssertion(() =>
        {
            var status = pagina.Find(".status-do-sistema").TextContent;
            status.Should().Contain("Sistema indisponível");
            status.Should().NotContainAny("503", "Exception", "health", "HTTP");
        });
    }

    [Fact]
    public void Home_QuandoRenderizada_FormataADataEmPtBr()
    {
        DefinirStatus(SystemStatus.Disponivel);

        var pagina = Render<Home>();

        pagina.Find(".data-de-hoje").TextContent.Should().Be("Hoje é domingo, 1 de novembro de 2026");
    }
}
