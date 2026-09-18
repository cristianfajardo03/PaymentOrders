using PaymentOrders.Domain.Orders;

namespace PaymentOrders.Domain.Tests;

public sealed class PaymentOrderStateMachineTests
{
    [Fact]
    public void Constructor_rejects_an_amount_above_the_business_limit()
    {
        var draft = ValidDraft() with { Amount = PaymentOrder.MaximumAmount + 0.01m };

        Assert.Throws<ArgumentOutOfRangeException>(() => new TestPaymentOrder(draft));
    }

    [Fact]
    public void International_order_rejects_missing_swift_code()
    {
        var draft = ValidDraft() with { SwiftCode = null };

        Assert.Throws<ArgumentException>(() => new TestInternationalPaymentOrder(draft));
    }

    [Fact]
    public void Scheduled_order_rejects_a_date_that_is_not_in_the_future()
    {
        var draft = ValidDraft() with { ScheduledFor = DateTimeOffset.UtcNow.AddMinutes(-1) };

        Assert.Throws<ArgumentOutOfRangeException>(() => new TestScheduledPaymentOrder(draft));
    }

    [Fact]
    public void Pending_order_can_be_cancelled_but_created_order_cannot()
    {
        var order = new TestPaymentOrder(ValidDraft());

        Assert.Throws<InvalidOperationException>(() => order.Cancel());

        order.MoveToPending();
        order.Cancel();

        Assert.Equal(PaymentOrderStatus.Cancelled, order.Status);
        Assert.Contains(order.AuditEvents, @event => @event.Type == PaymentOrderEventType.Cancelled);
    }

    private static PaymentOrderDraft ValidDraft() => new(
        Guid.NewGuid(),
        "001-001-000000001",
        "001-001-000000002",
        250m,
        "usd",
        "BANKCOBB",
        DateTimeOffset.UtcNow.AddDays(1),
        DateTimeOffset.UtcNow);

    private sealed class TestPaymentOrder(PaymentOrderDraft draft) : PaymentOrder(draft, OrderType.National);

    private sealed class TestInternationalPaymentOrder(PaymentOrderDraft draft) : PaymentOrder(draft, OrderType.International)
    {
        protected override void ValidateTypeSpecificInvariants(PaymentOrderDraft orderDraft) =>
            RequireSwiftCode(orderDraft.SwiftCode);
    }

    private sealed class TestScheduledPaymentOrder(PaymentOrderDraft draft) : PaymentOrder(draft, OrderType.Scheduled)
    {
        protected override void ValidateTypeSpecificInvariants(PaymentOrderDraft orderDraft) =>
            RequireFutureSchedule(orderDraft.ScheduledFor, orderDraft.CreatedAt);
    }
}
