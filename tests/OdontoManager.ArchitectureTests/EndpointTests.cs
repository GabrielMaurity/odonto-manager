using Microsoft.EntityFrameworkCore;
using NetArchTest.Rules;
using static OdontoManager.ArchitectureTests.Solution;

namespace OdontoManager.ArchitectureTests;

public class EndpointTests
{
    [Fact]
    public void Api_NaoReferenciaEfCore()
    {
        Types.InAssembly(ApiAssembly)
            .ShouldNot()
            .HaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult()
            .ShouldPass("Endpoints não referenciam DbContext (specification 10.2)");
    }

    [Fact]
    public void Api_NaoReferenciaRepositoriosNemDbContexts()
    {
        var proibidos = DomainAssembly.GetTypes()
            .Where(type => type.IsInterface && type.Name.EndsWith("Repository", StringComparison.Ordinal))
            .Concat(InfrastructureAssembly.GetTypes().Where(type => typeof(DbContext).IsAssignableFrom(type)))
            .Select(type => type.FullName!)
            .ToArray();

        if (proibidos.Length == 0)
        {
            return;
        }

        Types.InAssembly(ApiAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(proibidos)
            .GetResult()
            .ShouldPass("Endpoints não referenciam repositórios nem DbContext (specification 10.2)");
    }
}
