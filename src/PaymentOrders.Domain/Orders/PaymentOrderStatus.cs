namespace PaymentOrders.Domain.Orders;

public enum PaymentOrderStatus
{
    Created = 1,
    Pending = 2,
    Processing = 3,
    Completed = 4,
    Failed = 5,
    Cancelled = 6
}
