using TestResult = NetArchTest.Rules.TestResult;

namespace OdontoManager.ArchitectureTests;

internal static class TestResultExtensions
{
    public static void ShouldPass(this TestResult result, string regra)
    {
        var violacoes = result.FailingTypeNames ?? [];
        result.IsSuccessful.Should().BeTrue($"{regra}; tipos que violam: {string.Join(", ", violacoes)}");
    }
}
