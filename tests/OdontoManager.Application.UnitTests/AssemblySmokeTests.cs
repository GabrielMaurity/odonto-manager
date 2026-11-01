namespace OdontoManager.Application.UnitTests;

public class AssemblySmokeTests
{
    [Fact]
    public void Assembly_QuandoCarregado_TemNomeDoProjeto()
    {
        var nome = typeof(OdontoManager.Application.AssemblyMarker).Assembly.GetName().Name;

        nome.Should().Be("OdontoManager.Application");
    }
}
