using System.Globalization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OdontoManager.Web.Client.Services;
using Radzen;

namespace OdontoManager.Web.Client;

public static class WebClientServices
{
    public static readonly CultureInfo Cultura = CultureInfo.GetCultureInfo("pt-BR");

    public static IServiceCollection AddWebClientServices(this IServiceCollection services, IConfiguration configuration)
    {
        var apiBaseUrl = configuration["Api:BaseUrl"]
            ?? throw new InvalidOperationException("A configuração 'Api:BaseUrl' não foi definida.");

        services.AddRadzenComponents();
        services.TryAddSingleton(TimeProvider.System);
        services.AddScoped<ISystemStatusClient>(_ => new SystemStatusClient(new HttpClient
        {
            BaseAddress = new Uri(apiBaseUrl.EndsWith('/') ? apiBaseUrl : apiBaseUrl + "/"),
            Timeout = TimeSpan.FromSeconds(5)
        }));

        return services;
    }

    public static void ApplyCulture()
    {
        CultureInfo.DefaultThreadCurrentCulture = Cultura;
        CultureInfo.DefaultThreadCurrentUICulture = Cultura;
    }
}
