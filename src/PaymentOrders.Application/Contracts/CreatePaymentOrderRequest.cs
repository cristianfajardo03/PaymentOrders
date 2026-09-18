using PaymentOrders.Domain.Orders;

namespace PaymentOrders.Application.Contracts;

public sealed record CreatePaymentOrderRequest(
    OrderType OrderType,
    string SourceAccount,
    string TargetAccount,
    decimal Amount,
    string Currency,
    string? SwiftCode,
    DateTimeOffset? ScheduledFor);
