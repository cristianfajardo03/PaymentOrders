namespace PaymentOrders.Domain.Orders;

public class NationalPaymentOrder : PaymentOrder
{
    protected internal NationalPaymentOrder()
    {
    }

    protected internal NationalPaymentOrder(PaymentOrderDraft draft)
        : base(draft, OrderType.National)
    {
    }
}
