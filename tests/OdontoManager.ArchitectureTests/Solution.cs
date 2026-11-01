using System.Reflection;

namespace OdontoManager.ArchitectureTests;

internal static class Solution
{
    public static readonly Assembly DomainAssembly = typeof(OdontoManager.Domain.AssemblyMarker).Assembly;
    public static readonly Assembly ApplicationAssembly = typeof(OdontoManager.Application.AssemblyMarker).Assembly;
    public static readonly Assembly InfrastructureAssembly = typeof(OdontoManager.Infrastructure.AssemblyMarker).Assembly;
    public static readonly Assembly ContractsAssembly = typeof(OdontoManager.Contracts.AssemblyMarker).Assembly;
    public static readonly Assembly ApiAssembly = typeof(OdontoManager.Api.AssemblyMarker).Assembly;
    public static readonly Assembly WebServerAssembly = typeof(OdontoManager.Web.AssemblyMarker).Assembly;
    public static readonly Assembly WebClientAssembly = typeof(OdontoManager.Web.Client.AssemblyMarker).Assembly;

    public static readonly Assembly[] Backend = [DomainAssembly, ApplicationAssembly, InfrastructureAssembly, ApiAssembly];

    public static readonly string[] Modulos = ["Identidade", "Pacientes", "Profissionais", "Agenda", "Materiais", "Exames"];

    public const string DomainNamespace = "OdontoManager.Domain";
    public const string ApplicationNamespace = "OdontoManager.Application";
    public const string InfrastructureNamespace = "OdontoManager.Infrastructure";
    public const string ContractsNamespace = "OdontoManager.Contracts";
    public const string ApiNamespace = "OdontoManager.Api";
    public const string WebNamespace = "OdontoManager.Web";
}
