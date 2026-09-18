namespace PaymentOrders.Domain.Orders;

public sealed record PaymentOrderDraft(
    Guid Id,
    string SourceAccount,
    string TargetAccount,
    decimal Amount,
    string Currency,
    string? SwiftCode,
    DateTimeOffset? ScheduledFor,
    DateTimeOffset CreatedAt);
