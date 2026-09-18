namespace PaymentOrders.Application.Ports;

public interface IIdempotencyStore
{
    Task<Guid?> GetOrderIdAsync(string key, CancellationToken cancellationToken);

    Task<bool> TryRegisterAsync(string key, Guid orderId, DateTimeOffset createdAt, CancellationToken cancellationToken);
}
