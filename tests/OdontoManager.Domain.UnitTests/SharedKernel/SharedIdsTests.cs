using OdontoManager.Domain.SharedKernel;

namespace OdontoManager.Domain.UnitTests.SharedKernel;

public class SharedIdsTests
{
    [Fact]
    public void Novo_QuandoChamado_GeraGuidVersao7()
    {
        PacienteId.Novo().Value.Version.Should().Be(7);
        DentistaId.Novo().Value.Version.Should().Be(7);
        UsuarioId.Novo().Value.Version.Should().Be(7);
    }

    [Fact]
    public void Ids_ComMesmoValor_SaoIguais()
    {
        var valor = Guid.CreateVersion7();

        new PacienteId(valor).Should().Be(new PacienteId(valor));
        (new DentistaId(valor) == new DentistaId(valor)).Should().BeTrue();
    }

    [Fact]
    public void Ids_ComValoresDiferentes_SaoDiferentes()
    {
        PacienteId.Novo().Should().NotBe(PacienteId.Novo());
    }

    [Fact]
    public async Task Novo_GeradosEmSequencia_SaoOrdenadosPeloTempo()
    {
        var primeiro = UsuarioId.Novo();
        await Task.Delay(5, TestContext.Current.CancellationToken);
        var segundo = UsuarioId.Novo();

        string.CompareOrdinal(primeiro.ToString(), segundo.ToString()).Should().BeNegative();
        primeiro.Value.CompareTo(segundo.Value).Should().BeNegative();
    }

    [Fact]
    public void ToString_QuandoChamado_RetornaOGuid()
    {
        var valor = Guid.CreateVersion7();

        new PacienteId(valor).ToString().Should().Be(valor.ToString());
    }
}
