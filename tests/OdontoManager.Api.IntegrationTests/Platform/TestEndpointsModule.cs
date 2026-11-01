using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OdontoManager.Api.Infrastructure.ErrorHandling;
using OdontoManager.Api.Infrastructure.Modules;
using OdontoManager.Domain.SharedKernel;

namespace OdontoManager.Api.IntegrationTests.Platform;

internal sealed class TestEndpointsModule : IApiModule
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var testes = endpoints.MapGroup("/testes");

        testes.MapGet("/erros/nao-encontrado", () => Error.NotFound("Teste.NaoEncontrado", "Item não encontrado.").ToProblem());
        testes.MapGet("/erros/conflito", () => Error.Conflict("Teste.Conflito", "Item em conflito.").ToProblem());
        testes.MapGet("/erros/regra", () => Error.BusinessRule("Agenda.ConsultaNaoPodeSerCancelada", "Consulta não pode ser cancelada.").ToProblem());
        testes.MapGet("/erros/validacao", () => new ValidationError(new Dictionary<string, string[]>
        {
            ["nome"] = ["Nome é obrigatório."],
            ["email"] = ["E-mail inválido.", "E-mail muito curto."]
        }).ToProblem());
        testes.MapGet("/excecao", string () => throw new InvalidOperationException("Falha secreta: Host=db-interno;Password=senha"));
        testes.MapGet("/ok", () => TypedResults.Ok("ok"));
    }
}
