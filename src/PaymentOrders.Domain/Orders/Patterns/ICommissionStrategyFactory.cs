namespace PaymentOrders.Domain.Orders.Patterns;

public interface ICommissionStrategyFactory
{
    ICommissionStrategy GetFor(OrderType orderType);
}
