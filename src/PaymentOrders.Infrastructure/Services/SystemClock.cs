using PaymentOrders.Application.Common;

namespace PaymentOrders.Infrastructure.Services;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
