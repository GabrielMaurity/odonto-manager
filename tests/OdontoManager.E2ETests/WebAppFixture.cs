using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Playwright;

namespace OdontoManager.E2ETests;

public sealed class WebAppFixture : IAsyncLifetime
{
    private const string ApiIndisponivel = "http://127.0.0.1:1";

    private readonly WebFactory _factory = new();
    private IPlaywright? _playwright;

    public IBrowser Browser { get; private set; } = null!;

    public Uri BaseAddress { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        _factory.UseKestrel(0);
        _factory.StartServer();
        BaseAddress = _factory.ClientOptions.BaseAddress;

        _playwright = await Playwright.CreateAsync();
        Browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
            ExecutablePath = Environment.GetEnvironmentVariable("PLAYWRIGHT_CHROMIUM_EXECUTABLE_PATH")
        });
    }

    public async ValueTask DisposeAsync()
    {
        if (Browser is not null)
        {
            await Browser.DisposeAsync();
        }

        _playwright?.Dispose();
        await _factory.DisposeAsync();
    }

    private sealed class WebFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder) =>
            builder.UseSetting("Api:BaseUrl", ApiIndisponivel);
    }
}
