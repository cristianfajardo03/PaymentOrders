namespace PaymentOrders.Domain.Orders.Patterns;

public sealed class NationalCommissionStrategy : ICommissionStrategy
{
    public decimal Calculate(PaymentOrder order)
    {
        // El estudiante debe calcular la comisión aplicable a una orden nacional sin trasladar esta regla a Application o Api.
        return Math.Round(order.Amount * 0.01m, 2, MidpointRounding.AwayFromZero);
    }
}

public sealed class InternationalCommissionStrategy : ICommissionStrategy
{
    public decimal Calculate(PaymentOrder order)
    {
        // El estudiante debe calcular la comisión aplicable a una orden internacional sin trasladar esta regla a Application o Api.
        return Math.Round(order.Amount * 0.03m, 2, MidpointRounding.AwayFromZero);
    }
}

public sealed class ScheduledCommissionStrategy : ICommissionStrategy
{
    public decimal Calculate(PaymentOrder order)
    {
        // El estudiante debe calcular la comisión aplicable a una orden programada sin trasladar esta regla a Application o Api.
        return Math.Round(order.Amount * 0.005m, 2, MidpointRounding.AwayFromZero);
    }
}
