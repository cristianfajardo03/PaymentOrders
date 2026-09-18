using PaymentOrders.Domain.Orders;
using PaymentOrders.Domain.Orders.Patterns;

namespace PaymentOrders.ContractTests;

public sealed class CreationalPatternContractSamples
{
    // NO MODIFICAR — muestra reducida de la suite real de evaluación.
    [Theory]
    [MemberData(nameof(ValidOrderCases))]
    public void Factory_and_builder_create_the_requested_order_type(
        OrderType orderType,
        string? swiftCode,
        DateTimeOffset? scheduledFor)
    {
        var order = CreateConfiguredBuilder(orderType, 250m, swiftCode, scheduledFor).Build();

        Assert.Equal(orderType, order.Type);
        Assert.Equal(PaymentOrderStatus.Created, order.Status);
    }

    // NO MODIFICAR — muestra reducida de la suite real de evaluación.
    [Theory]
    [MemberData(nameof(InvalidOrderCases))]
    public void Builder_rejects_invalid_data(
        OrderType orderType,
        string sourceAccount,
        string targetAccount,
        decimal amount,
        string? swiftCode,
        DateTimeOffset? scheduledFor)
    {
        var exception = Record.Exception(() => CreateConfiguredBuilder(
            orderType,
            amount,
            swiftCode,
            scheduledFor,
            sourceAccount,
            targetAccount).Build());

        Assert.NotNull(exception);
        Assert.IsNotType<NotImplementedException>(exception);
    }

    // NO MODIFICAR — muestra reducida de la suite real de evaluación.
    [Theory]
    [MemberData(nameof(CommissionCases))]
    public void Commission_factory_selects_the_strategy_that_calculates_the_expected_fee(
        OrderType orderType,
        string? swiftCode,
        DateTimeOffset? scheduledFor,
        decimal expectedCommission)
    {
        var order = CreateConfiguredBuilder(orderType, 101.23m, swiftCode, scheduledFor).Build();
        var strategy = new CommissionStrategyFactory().GetFor(order.Type);

        var commission = strategy.Calculate(order);

        Assert.Equal(expectedCommission, commission);
    }

    public static IEnumerable<object?[]> ValidOrderCases =>
    [
        [OrderType.National, null, null],
        [OrderType.International, "BANKCOBB", null],
        [OrderType.Scheduled, null, DateTimeOffset.UtcNow.AddDays(1)]
    ];

    public static IEnumerable<object?[]> InvalidOrderCases =>
    [
        [OrderType.National, "001-001-000000001", "001-001-000000001", 250m, null, null],
        [OrderType.National, "001-001-000000001", "001-001-000000002", 0m, null, null],
        [OrderType.International, "001-001-000000001", "001-001-000000002", 250m, null, null],
        [OrderType.Scheduled, "001-001-000000001", "001-001-000000002", 250m, null, DateTimeOffset.UtcNow.AddDays(-1)]
    ];

    public static IEnumerable<object?[]> CommissionCases =>
    [
        [OrderType.National, null, null, 1.01m],
        [OrderType.International, "BANKCOBB", null, 3.04m],
        [OrderType.Scheduled, null, DateTimeOffset.UtcNow.AddDays(1), 0.51m]
    ];

    private static IPaymentOrderBuilder CreateConfiguredBuilder(
        OrderType orderType,
        decimal amount,
        string? swiftCode,
        DateTimeOffset? scheduledFor,
        string sourceAccount = "001-001-000000001",
        string targetAccount = "001-001-000000002") =>
        new PaymentOrderFactory()
            .CreateBuilder(orderType)
            .WithId(Guid.NewGuid())
            .WithSourceAccount(sourceAccount)
            .WithTargetAccount(targetAccount)
            .WithAmount(amount)
            .WithCurrency("USD")
            .WithSwiftCode(swiftCode)
            .WithScheduledFor(scheduledFor)
            .WithCreatedAt(DateTimeOffset.UtcNow);
}
