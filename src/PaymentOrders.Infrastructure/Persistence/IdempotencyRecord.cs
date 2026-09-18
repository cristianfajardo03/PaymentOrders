namespace PaymentOrders.Infrastructure.Persistence;

public sealed class IdempotencyRecord
{
    private IdempotencyRecord()
    {
    }

    public IdempotencyRecord(string key, Guid paymentOrderId, DateTimeOffset createdAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        Key = key.Trim();
        PaymentOrderId = paymentOrderId;
        CreatedAt = createdAt;
    }

    public string Key { get; private set; } = string.Empty;

    public Guid PaymentOrderId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
}
