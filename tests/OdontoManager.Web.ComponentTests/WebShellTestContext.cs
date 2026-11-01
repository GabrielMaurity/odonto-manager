using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OdontoManager.Web.Client;
using OdontoManager.Web.Client.Services;
using Radzen;

namespace OdontoManager.Web.ComponentTests;

public abstract class WebShellTestContext : BunitContext
{
    protected WebShellTestContext()
    {
        WebClientServices.ApplyCulture();
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddRadzenComponents();
        Services.AddSingleton(StatusClient);
        Services.AddSingleton<TimeProvider>(Relogio);
    }

    protected ISystemStatusClient StatusClient { get; } = Substitute.For<ISystemStatusClient>();

    protected RelogioFixo Relogio { get; } = new(new DateTimeOffset(2026, 11, 1, 12, 0, 0, TimeSpan.Zero));

    protected void DefinirStatus(SystemStatus status) =>
        StatusClient.ObterAsync(Arg.Any<CancellationToken>()).Returns(status);
}

public sealed class RelogioFixo(DateTimeOffset agora) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => agora;

    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
}
