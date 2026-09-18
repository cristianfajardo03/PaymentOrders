namespace PaymentOrders.Domain.Orders.Patterns;

public interface IPaymentOrderFactory
{
    IPaymentOrderBuilder CreateBuilder(OrderType orderType);
}
