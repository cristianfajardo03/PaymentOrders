namespace PaymentOrders.Domain.Orders;

public class InternationalPaymentOrder : PaymentOrder
{
    protected internal InternationalPaymentOrder()
    {
    }

    protected internal InternationalPaymentOrder(PaymentOrderDraft draft)
        : base(draft, OrderType.International)
    {
    }

    protected override void ValidateTypeSpecificInvariants(PaymentOrderDraft draft) => RequireSwiftCode(draft.SwiftCode);
}
