using OdontoManager.Api.Infrastructure;
using OdontoManager.Api.Infrastructure.ErrorHandling;
using OdontoManager.Api.Infrastructure.Modules;
using OdontoManager.Application;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication(typeof(OdontoManager.Infrastructure.AssemblyMarker).Assembly);
builder.Services.AddApiProblemDetails();
builder.Services.AddWebCors(builder.Configuration);
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors(CorsSetup.WebPolicy);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

var api = app.MapGroup("/api/v1");
foreach (var module in app.Services.GetServices<IApiModule>())
{
    module.MapEndpoints(api);
}

await app.RunAsync();

public partial class Program;
