namespace PaymentOrders.Application.Common;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
