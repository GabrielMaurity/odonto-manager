using NetArchTest.Rules;
using static OdontoManager.ArchitectureTests.Solution;

namespace OdontoManager.ArchitectureTests;

public class ModuleTests
{
    public static TheoryData<string, string> ParesDeModulos()
    {
        var pares = new TheoryData<string, string>();
        foreach (var origem in Modulos)
        {
            foreach (var destino in Modulos.Where(destino => destino != origem))
            {
                pares.Add(origem, destino);
            }
        }

        return pares;
    }

    [Theory]
    [MemberData(nameof(ParesDeModulos))]
    public void Modulo_SoReferenciaOutroModuloPelaPublicApi(string origem, string destino)
    {
        var proibidos = ProhibitedReferencesTo(destino);
        if (proibidos.Length == 0)
        {
            return;
        }

        Types.InAssemblies(Backend)
            .That()
            .ResideInNamespaceMatching($@"^OdontoManager\.\w+\.{origem}(\.|$)")
            .ShouldNot()
            .HaveDependencyOnAny(proibidos)
            .GetResult()
            .ShouldPass($"{origem} só referencia {destino} pela PublicApi ou pelo Shared Kernel (constituição IV.5)");
    }

    private static string[] ProhibitedReferencesTo(string modulo)
    {
        var publicApi = $"{ApplicationNamespace}.{modulo}.PublicApi";

        return Backend
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.Namespace is { } ns
                && System.Text.RegularExpressions.Regex.IsMatch(ns, $@"^OdontoManager\.\w+\.{modulo}(\.|$)")
                && !ns.StartsWith(publicApi, StringComparison.Ordinal))
            .Select(type => type.FullName!)
            .ToArray();
    }
}
