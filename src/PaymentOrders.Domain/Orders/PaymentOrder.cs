namespace PaymentOrders.Domain.Orders;

public abstract class PaymentOrder
{
    public const decimal MaximumAmount = 1_000_000m;

    private readonly List<PaymentOrderEvent> _auditEvents = [];

    protected PaymentOrder()
    {
    }

    protected PaymentOrder(PaymentOrderDraft draft, OrderType type)
    {
        ValidateBaseInvariants(draft);
        ValidateTypeSpecificInvariants(draft);

        Id = draft.Id;
        Type = type;
        SourceAccount = draft.SourceAccount.Trim();
        TargetAccount = draft.TargetAccount.Trim();
        Amount = draft.Amount;
        Currency = draft.Currency.Trim().ToUpperInvariant();
        SwiftCode = string.IsNullOrWhiteSpace(draft.SwiftCode) ? null : draft.SwiftCode.Trim().ToUpperInvariant();
        ScheduledFor = draft.ScheduledFor;
        CreatedAt = draft.CreatedAt;
        Status = PaymentOrderStatus.Created;

        AddAuditEvent(PaymentOrderEventType.Created, "Payment order created.", CreatedAt);
    }

    public Guid Id { get; private set; }

    public OrderType Type { get; private set; }

    public string SourceAccount { get; private set; } = string.Empty;

    public string TargetAccount { get; private set; } = string.Empty;

    public decimal Amount { get; private set; }

    public string Currency { get; private set; } = string.Empty;

    public string? SwiftCode { get; private set; }

    public DateTimeOffset? ScheduledFor { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public PaymentOrderStatus Status { get; private set; }

    public decimal? Commission { get; private set; }

    public string? FailureReason { get; private set; }

    public IReadOnlyCollection<PaymentOrderEvent> AuditEvents => _auditEvents.AsReadOnly();

    public void MoveToPending() => Transition(PaymentOrderStatus.Created, PaymentOrderStatus.Pending, PaymentOrderEventType.Pending, "Payment order is pending.");

    public void StartProcessing() => Transition(PaymentOrderStatus.Pending, PaymentOrderStatus.Processing, PaymentOrderEventType.Processing, "Payment order processing started.");

    public void Complete(decimal commission)
    {
        if (commission < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(commission), "Commission cannot be negative.");
        }

        Transition(PaymentOrderStatus.Processing, PaymentOrderStatus.Completed, PaymentOrderEventType.Completed, "Payment order completed.");
        Commission = commission;
    }

    public void Fail(string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        Transition(PaymentOrderStatus.Processing, PaymentOrderStatus.Failed, PaymentOrderEventType.Failed, "Payment order failed.");
        FailureReason = reason.Trim();
    }

    public void Cancel() => Transition(PaymentOrderStatus.Pending, PaymentOrderStatus.Cancelled, PaymentOrderEventType.Cancelled, "Payment order cancelled.");

    protected virtual void ValidateTypeSpecificInvariants(PaymentOrderDraft draft)
    {
    }

    protected static void RequireSwiftCode(string? swiftCode)
    {
        if (string.IsNullOrWhiteSpace(swiftCode))
        {
            throw new ArgumentException("International payment orders require a SWIFT code.", nameof(swiftCode));
        }

        var normalized = swiftCode.Trim();
        if (normalized.Length is not 8 and not 11 || !normalized.All(char.IsLetterOrDigit))
        {
            throw new ArgumentException("SWIFT code must contain 8 or 11 alphanumeric characters.", nameof(swiftCode));
        }
    }

    protected static void RequireFutureSchedule(DateTimeOffset? scheduledFor, DateTimeOffset createdAt)
    {
        if (scheduledFor is null || scheduledFor <= createdAt || scheduledFor <= DateTimeOffset.UtcNow)
        {
            throw new ArgumentOutOfRangeException(nameof(scheduledFor), "Scheduled payment orders require a future execution date.");
        }
    }

    private static void ValidateBaseInvariants(PaymentOrderDraft draft)
    {
        if (draft.Id == Guid.Empty)
        {
            throw new ArgumentException("Payment order id is required.", nameof(draft));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(draft.SourceAccount);
        ArgumentException.ThrowIfNullOrWhiteSpace(draft.TargetAccount);

        if (string.Equals(draft.SourceAccount.Trim(), draft.TargetAccount.Trim(), StringComparison.Ordinal))
        {
            throw new ArgumentException("Source and target accounts must be different.", nameof(draft));
        }

        if (draft.Amount <= 0 || draft.Amount > MaximumAmount)
        {
            throw new ArgumentOutOfRangeException(nameof(draft), $"Amount must be greater than zero and no greater than {MaximumAmount}.");
        }

        if (string.IsNullOrWhiteSpace(draft.Currency) || draft.Currency.Trim().Length != 3)
        {
            throw new ArgumentException("Currency must be a three-character code.", nameof(draft));
        }

        if (draft.CreatedAt == default)
        {
            throw new ArgumentException("Creation timestamp is required.", nameof(draft));
        }
    }

    private void Transition(PaymentOrderStatus expected, PaymentOrderStatus next, PaymentOrderEventType eventType, string description)
    {
        if (Status != expected)
        {
            throw new InvalidOperationException($"Cannot transition a {Status} payment order to {next}.");
        }

        Status = next;
        AddAuditEvent(eventType, description, DateTimeOffset.UtcNow);
    }

    private void AddAuditEvent(PaymentOrderEventType eventType, string description, DateTimeOffset occurredAt) =>
        _auditEvents.Add(new PaymentOrderEvent(Id, eventType, occurredAt, description));
}
