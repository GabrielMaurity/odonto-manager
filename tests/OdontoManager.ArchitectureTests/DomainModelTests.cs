using System.Reflection;
using System.Runtime.CompilerServices;
using NetArchTest.Rules;
using OdontoManager.Domain.SharedKernel;
using static OdontoManager.ArchitectureTests.Solution;

namespace OdontoManager.ArchitectureTests;

public class DomainModelTests
{
    [Fact]
    public void Entidades_NaoTemSettersPublicos()
    {
        var violacoes = DomainAssembly.GetTypes()
            .Where(IsEntity)
            .SelectMany(type => type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            .Where(property => property.SetMethod is { IsPublic: true } setter && !IsInitOnly(setter))
            .Select(property => $"{property.DeclaringType!.FullName}.{property.Name}")
            .ToArray();

        violacoes.Should().BeEmpty("entidades não têm setters públicos (specification 3.2)");
    }

    [Fact]
    public void EventosDeDominio_SaoSealed()
    {
        Types.InAssembly(DomainAssembly)
            .That()
            .ImplementInterface(typeof(IDomainEvent))
            .And()
            .AreClasses()
            .Should()
            .BeSealed()
            .GetResult()
            .ShouldPass("eventos de domínio são sealed (specification 3.5)");
    }

    private static bool IsEntity(Type type)
    {
        for (var current = type.BaseType; current is not null; current = current.BaseType)
        {
            if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(Entity<>))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsInitOnly(MethodInfo setter) =>
        setter.ReturnParameter.GetRequiredCustomModifiers().Contains(typeof(IsExternalInit));
}
