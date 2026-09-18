using System.Reflection;
using NetArchTest.Rules;
using PaymentOrders.Domain.Orders;

namespace PaymentOrders.ArchitectureTests;

public sealed class DomainDependencyTests
{
    private static readonly Assembly DomainAssembly = typeof(PaymentOrder).Assembly;

    [Theory]
    [InlineData("PaymentOrders.Api")]
    [InlineData("PaymentOrders.Infrastructure")]
    [InlineData("Microsoft.EntityFrameworkCore")]
    [InlineData("Microsoft.Data.Sqlite")]
    public void Domain_does_not_depend_on_outer_layers_or_orm(string forbiddenDependency)
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOn(forbiddenDependency)
            .GetResult();

        Assert.True(result.IsSuccessful, result.FailingTypeNames is null
            ? $"Domain depends on {forbiddenDependency}."
            : $"Domain depends on {forbiddenDependency}: {string.Join(", ", result.FailingTypeNames)}");
    }
}
