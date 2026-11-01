using Bunit;
using Microsoft.AspNetCore.Components;
using OdontoManager.Web.Client.Layout;
using OdontoManager.Web.Client.Pages;

namespace OdontoManager.Web.ComponentTests;

public class LayoutTests : WebShellTestContext
{
    [Fact]
    public void MainLayout_QuandoRenderizado_ExibeNomeMenuEConteudo()
    {
        var layout = Render<MainLayout>(parametros => parametros
            .Add(p => p.Body, (RenderFragment)(builder => builder.AddMarkupContent(0, "<p id=\"conteudo\">Conteúdo da página</p>"))));

        layout.Find(".app-nome").TextContent.Should().Be("Odonto Manager");
        layout.Find("nav[aria-label='Navegação principal']").TextContent.Should().Contain("Início");
        layout.Find("main #conteudo").TextContent.Should().Be("Conteúdo da página");
    }

    [Fact]
    public void NotFound_QuandoRenderizado_ExibeMensagemELinkParaInicio()
    {
        var pagina = Render<NotFound>();

        pagina.Markup.Should().Contain("Página não encontrada");
        pagina.FindAll("a").Should().Contain(link => link.GetAttribute("href") == "/");
    }
}
