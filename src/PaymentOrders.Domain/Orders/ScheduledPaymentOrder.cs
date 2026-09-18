namespace PaymentOrders.Domain.Orders;

public class ScheduledPaymentOrder : PaymentOrder
{
    protected internal ScheduledPaymentOrder()
    {
    }

    protected internal ScheduledPaymentOrder(PaymentOrderDraft draft)
        : base(draft, OrderType.Scheduled)
    {
    }

    protected override void ValidateTypeSpecificInvariants(PaymentOrderDraft draft) =>
        RequireFutureSchedule(draft.ScheduledFor, draft.CreatedAt);
}
