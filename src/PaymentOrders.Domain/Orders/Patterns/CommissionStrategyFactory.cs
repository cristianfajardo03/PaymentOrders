namespace PaymentOrders.Domain.Orders.Patterns;

public sealed class CommissionStrategyFactory : ICommissionStrategyFactory
{
    public ICommissionStrategy GetFor(OrderType orderType)
    {
        // El estudiante debe seleccionar aquí la estrategia de comisión del tipo recibido; ningún consumidor debe repetir esa decisión.
        return orderType switch
        {
            OrderType.National => new NationalCommissionStrategy(),
            OrderType.International => new InternationalCommissionStrategy(),
            OrderType.Scheduled => new ScheduledCommissionStrategy(),
            _ => throw new ArgumentOutOfRangeException(
                nameof(orderType),
                orderType,
                "Tipo de orden no manejado.")
        };
    }
}
