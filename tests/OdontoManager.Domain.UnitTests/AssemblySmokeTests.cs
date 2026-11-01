namespace OdontoManager.Domain.UnitTests;

public class AssemblySmokeTests
{
    [Fact]
    public void Assembly_QuandoCarregado_TemNomeDoProjeto()
    {
        var nome = typeof(OdontoManager.Domain.AssemblyMarker).Assembly.GetName().Name;

        nome.Should().Be("OdontoManager.Domain");
    }
}
