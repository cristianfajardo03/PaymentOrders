using PaymentOrders.Domain.Orders;

namespace PaymentOrders.Application.Contracts;

public sealed record PaymentOrderEventResponse(
    Guid Id,
    PaymentOrderEventType Type,
    DateTimeOffset OccurredAt,
    string Description);
