using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OdontoManager.Web.Client;

WebClientServices.ApplyCulture();

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddWebClientServices(builder.Configuration);

await builder.Build().RunAsync();
