namespace PaymentOrders.Domain.Orders;

public sealed class PaymentOrderEvent
{
    private PaymentOrderEvent()
    {
    }

    internal PaymentOrderEvent(Guid paymentOrderId, PaymentOrderEventType type, DateTimeOffset occurredAt, string description)
    {
        Id = Guid.NewGuid();
        PaymentOrderId = paymentOrderId;
        Type = type;
        OccurredAt = occurredAt;
        Description = description;
    }

    public Guid Id { get; private set; }

    public Guid PaymentOrderId { get; private set; }

    public PaymentOrderEventType Type { get; private set; }

    public DateTimeOffset OccurredAt { get; private set; }

    public string Description { get; private set; } = string.Empty;
}
