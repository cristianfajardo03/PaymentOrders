namespace PaymentOrders.Application.Contracts;

public sealed record CreatePaymentOrderResult(Guid OrderId, bool IsIdempotentReplay);
