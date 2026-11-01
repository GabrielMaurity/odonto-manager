using NetArchTest.Rules;
using static OdontoManager.ArchitectureTests.Solution;

namespace OdontoManager.ArchitectureTests;

public class LayerTests
{
    [Fact]
    public void Domain_NaoDependeDeOutrasCamadas()
    {
        Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(ApplicationNamespace, InfrastructureNamespace, ApiNamespace, ContractsNamespace, WebNamespace)
            .GetResult()
            .ShouldPass("Domain não depende de outras camadas (constituição IV.2)");
    }

    [Fact]
    public void Domain_NaoDependeDeFrameworksDeInfraestrutura()
    {
        Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny("Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore")
            .GetResult()
            .ShouldPass("Domain não depende de EF Core nem de ASP.NET Core (constituição IV.2)");
    }

    [Fact]
    public void Application_NaoDependeDeCamadasExternasNemDeEfCore()
    {
        Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(InfrastructureNamespace, ApiNamespace, ContractsNamespace, WebNamespace, "Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore")
            .GetResult()
            .ShouldPass("Application depende apenas de Domain (constituição IV.1)");
    }

    [Fact]
    public void Contracts_NaoDependeDeOutrosProjetos()
    {
        Types.InAssembly(ContractsAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(DomainNamespace, ApplicationNamespace, InfrastructureNamespace, ApiNamespace, WebNamespace)
            .GetResult()
            .ShouldPass("Contracts não depende de outros projetos (constituição IV.1)");
    }

    [Fact]
    public void Web_DependeApenasDeContracts()
    {
        Types.InAssemblies([WebServerAssembly, WebClientAssembly])
            .ShouldNot()
            .HaveDependencyOnAny(DomainNamespace, ApplicationNamespace, InfrastructureNamespace, ApiNamespace)
            .GetResult()
            .ShouldPass("Web e Web.Client dependem apenas de Contracts (constituição IV.3)");
    }
}
