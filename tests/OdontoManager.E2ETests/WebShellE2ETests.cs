using Microsoft.Playwright;

namespace OdontoManager.E2ETests;

[Trait("Category", "E2E")]
public class WebShellE2ETests(WebAppFixture app) : IClassFixture<WebAppFixture>
{
    [Fact]
    public async Task PaginaInicial_ComApiIndisponivel_ExibeLayoutEIndicaSistemaIndisponivel()
    {
        var page = await app.Browser.NewPageAsync();

        await page.GotoAsync(app.BaseAddress.ToString());

        (await page.Locator(".app-nome").InnerTextAsync()).Should().Be("Odonto Manager");
        (await page.Locator("nav[aria-label='Navegação principal']").InnerTextAsync()).Should().Contain("Início");
        await Assertions.Expect(page.Locator(".status-do-sistema")).ToContainTextAsync("Sistema indisponível");
        (await page.TitleAsync()).Should().Be("Início — Odonto Manager");
        (await page.Locator("html").GetAttributeAsync("lang")).Should().Be("pt-BR");
    }

    [Fact]
    public async Task EnderecoInexistente_ExibePaginaNaoEncontradaDentroDoLayout()
    {
        var page = await app.Browser.NewPageAsync();

        await page.GotoAsync(new Uri(app.BaseAddress, "/pagina-que-nao-existe").ToString());

        await Assertions.Expect(page.Locator("main")).ToContainTextAsync("Página não encontrada");
        (await page.Locator(".app-nome").InnerTextAsync()).Should().Be("Odonto Manager");
        await page.Locator("main a[href='/']").ClickAsync();
        await Assertions.Expect(page.Locator(".status-do-sistema")).ToBeVisibleAsync();
    }
}
