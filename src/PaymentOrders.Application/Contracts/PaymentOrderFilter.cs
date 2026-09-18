using PaymentOrders.Domain.Orders;

namespace PaymentOrders.Application.Contracts;

public sealed record PaymentOrderFilter(
    OrderType? OrderType,
    PaymentOrderStatus? Status,
    DateTimeOffset? CreatedFrom,
    DateTimeOffset? CreatedTo,
    int Page,
    int PageSize);
