using OdontoManager.Web.Client;
using OdontoManager.Web.Components;

WebClientServices.ApplyCulture();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddWebClientServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseRequestLocalization(options => options
    .SetDefaultCulture(WebClientServices.Cultura.Name)
    .AddSupportedCultures(WebClientServices.Cultura.Name)
    .AddSupportedUICultures(WebClientServices.Cultura.Name));
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(OdontoManager.Web.Client._Imports).Assembly);

await app.RunAsync();

public partial class Program;
