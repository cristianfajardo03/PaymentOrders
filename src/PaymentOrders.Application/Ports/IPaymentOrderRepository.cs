using PaymentOrders.Application.Contracts;
using PaymentOrders.Domain.Orders;

namespace PaymentOrders.Application.Ports;

public interface IPaymentOrderRepository
{
    Task AddAsync(PaymentOrder order, CancellationToken cancellationToken);

    Task<PaymentOrder?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken);

    Task<PagedResult<PaymentOrder>> ListAsync(PaymentOrderFilter filter, CancellationToken cancellationToken);

    Task<IReadOnlyList<PaymentOrderEvent>> GetEventsAsync(Guid orderId, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
