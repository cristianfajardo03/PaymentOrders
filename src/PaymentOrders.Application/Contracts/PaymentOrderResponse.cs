using PaymentOrders.Domain.Orders;

namespace PaymentOrders.Application.Contracts;

public sealed record PaymentOrderResponse(
    Guid Id,
    OrderType Type,
    PaymentOrderStatus Status,
    string SourceAccount,
    string TargetAccount,
    decimal Amount,
    string Currency,
    string? SwiftCode,
    DateTimeOffset? ScheduledFor,
    decimal? Commission,
    string? FailureReason,
    DateTimeOffset CreatedAt);
