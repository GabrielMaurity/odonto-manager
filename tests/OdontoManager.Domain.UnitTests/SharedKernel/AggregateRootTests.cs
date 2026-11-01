using OdontoManager.Domain.SharedKernel;

namespace OdontoManager.Domain.UnitTests.SharedKernel;

public class AggregateRootTests
{
    [Fact]
    public void Raise_QuandoChamado_RegistraEventoNaOrdem()
    {
        var agregado = AgregadoDeTeste.Criar();

        agregado.Alterar();

        agregado.DomainEvents.Should().HaveCount(2);
        agregado.DomainEvents.Select(e => e.GetType()).Should().ContainInOrder(typeof(AgregadoCriado), typeof(AgregadoAlterado));
    }

    [Fact]
    public void ClearDomainEvents_QuandoChamado_RemoveTodosOsEventos()
    {
        var agregado = AgregadoDeTeste.Criar();

        agregado.ClearDomainEvents();

        agregado.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void DomainEvents_QuandoConvertidoParaLista_NaoPermiteAlterarOAgregado()
    {
        var agregado = AgregadoDeTeste.Criar();

        var acao = () => ((ICollection<IDomainEvent>)agregado.DomainEvents).Clear();

        acao.Should().Throw<NotSupportedException>();
        agregado.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void Id_QuandoAgregadoCriado_EhAtribuidoPelaFabrica()
    {
        var agregado = AgregadoDeTeste.Criar();

        agregado.Id.Should().NotBe(Guid.Empty);
    }

    private sealed record AgregadoCriado(Guid Id) : IDomainEvent;

    private sealed record AgregadoAlterado(Guid Id) : IDomainEvent;

    private sealed class AgregadoDeTeste : AggregateRoot<Guid>
    {
        private AgregadoDeTeste()
        {
        }

        public static AgregadoDeTeste Criar()
        {
            var agregado = new AgregadoDeTeste { Id = Guid.CreateVersion7() };
            agregado.Raise(new AgregadoCriado(Guid.CreateVersion7()));
            return agregado;
        }

        public void Alterar() => Raise(new AgregadoAlterado(Guid.CreateVersion7()));
    }
}
