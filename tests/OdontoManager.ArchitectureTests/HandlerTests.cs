using NetArchTest.Rules;
using OdontoManager.Application.Abstractions.Messaging;
using static OdontoManager.ArchitectureTests.Solution;

namespace OdontoManager.ArchitectureTests;

public class HandlerTests
{
    public static TheoryData<Type> InterfacesDeHandler() => new(typeof(ICommandHandler<,>), typeof(IQueryHandler<,>));

    [Theory]
    [MemberData(nameof(InterfacesDeHandler))]
    public void Handlers_SaoSealedENaoPublicos(Type interfaceDeHandler)
    {
        var handlers = Types.InAssemblies([ApplicationAssembly, InfrastructureAssembly])
            .That()
            .ImplementInterface(interfaceDeHandler)
            .And()
            .AreClasses();

        handlers.GetTypes().Should().NotBeEmpty("a regra precisa verificar ao menos os decorators");

        handlers.Should().BeSealed().And().NotBePublic()
            .GetResult()
            .ShouldPass($"implementações de {interfaceDeHandler.Name} são sealed e não públicas (specification 4.1)");
    }
}
