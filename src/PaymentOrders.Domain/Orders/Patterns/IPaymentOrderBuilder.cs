namespace PaymentOrders.Domain.Orders.Patterns;

public interface IPaymentOrderBuilder
{
    IPaymentOrderBuilder WithId(Guid id);

    IPaymentOrderBuilder WithSourceAccount(string sourceAccount);

    IPaymentOrderBuilder WithTargetAccount(string targetAccount);

    IPaymentOrderBuilder WithAmount(decimal amount);

    IPaymentOrderBuilder WithCurrency(string currency);

    IPaymentOrderBuilder WithSwiftCode(string? swiftCode);

    IPaymentOrderBuilder WithScheduledFor(DateTimeOffset? scheduledFor);

    IPaymentOrderBuilder WithCreatedAt(DateTimeOffset createdAt);

    PaymentOrder Build();
}
