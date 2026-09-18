namespace PaymentOrders.Domain.Orders.Patterns;

public interface ICommissionStrategy
{
    decimal Calculate(PaymentOrder order);
}
