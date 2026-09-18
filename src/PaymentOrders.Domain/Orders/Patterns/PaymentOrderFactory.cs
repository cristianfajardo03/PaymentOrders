namespace PaymentOrders.Domain.Orders.Patterns;

public sealed class PaymentOrderFactory : IPaymentOrderFactory
{
    public IPaymentOrderBuilder CreateBuilder(OrderType orderType)
    {
        // El estudiante debe seleccionar aquí el subtipo concreto adecuado y entregar un Builder configurado, sin filtrar esas clases hacia Application ni Api.
        if (!Enum.IsDefined(orderType))
        {
            throw new ArgumentOutOfRangeException(
                nameof(orderType),
                orderType,
                "Tipo de orden no manejado.");
        }

        return new PaymentOrderBuilder(orderType);
    }
}
