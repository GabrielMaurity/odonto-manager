using OdontoManager.Domain.SharedKernel;

namespace OdontoManager.Domain.UnitTests.SharedKernel;

public class ResultTests
{
    private static readonly Error ErroDeTeste = Error.BusinessRule("Teste.Erro", "Erro de teste.");

    [Fact]
    public void Success_QuandoCriado_EhSucessoSemErro()
    {
        var resultado = Result.Success();

        resultado.IsSuccess.Should().BeTrue();
        resultado.IsFailure.Should().BeFalse();
        resultado.Invoking(r => r.Error).Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Failure_QuandoCriado_ExpoeOErro()
    {
        var resultado = Result.Failure(ErroDeTeste);

        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().Be(ErroDeTeste);
    }

    [Fact]
    public void ConversaoImplicita_DeError_GeraFalha()
    {
        Result resultado = ErroDeTeste;

        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().Be(ErroDeTeste);
    }

    [Fact]
    public void ResultGenerico_ConversaoDeValor_GeraSucessoComValor()
    {
        Result<int> resultado = 42;

        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().Be(42);
    }

    [Fact]
    public void ResultGenerico_ConversaoDeError_GeraFalha()
    {
        Result<int> resultado = ErroDeTeste;

        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Code.Should().Be("Teste.Erro");
    }

    [Fact]
    public void ResultGenerico_ValueEmFalha_LancaExcecaoComCodigoDoErro()
    {
        Result<int> resultado = ErroDeTeste;

        resultado.Invoking(r => r.Value)
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*Teste.Erro*");
    }

    [Fact]
    public void ResultGenerico_RetornadoDeMetodo_AceitaValorOuErro()
    {
        static Result<string> Dividir(int divisor) =>
            divisor == 0 ? Error.BusinessRule("Teste.DivisaoPorZero", "Divisor zero.") : "ok";

        Dividir(1).Value.Should().Be("ok");
        Dividir(0).Error.Code.Should().Be("Teste.DivisaoPorZero");
    }

    [Fact]
    public void Error_Fabricas_DefinemOTipoCorreto()
    {
        Error.NotFound("a", "b").Type.Should().Be(ErrorType.NotFound);
        Error.Conflict("a", "b").Type.Should().Be(ErrorType.Conflict);
        Error.BusinessRule("a", "b").Type.Should().Be(ErrorType.BusinessRule);
    }

    [Fact]
    public void ValidationError_QuandoCriado_EhDoTipoValidacaoComCamposEMensagens()
    {
        var erro = new ValidationError(new Dictionary<string, string[]> { ["nome"] = ["Obrigatório."] });

        erro.Type.Should().Be(ErrorType.Validation);
        erro.Code.Should().Be("Validation");
        erro.Errors["nome"].Should().ContainSingle().Which.Should().Be("Obrigatório.");
    }
}
