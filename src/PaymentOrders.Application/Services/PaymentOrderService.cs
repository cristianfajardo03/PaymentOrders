using PaymentOrders.Application.Common;
using PaymentOrders.Application.Contracts;
using PaymentOrders.Application.Ports;
using PaymentOrders.Domain.Orders;
using PaymentOrders.Domain.Orders.Patterns;

namespace PaymentOrders.Application.Services;

public sealed class PaymentOrderService(
    IPaymentOrderFactory paymentOrderFactory,
    ICommissionStrategyFactory commissionStrategyFactory,
    IPaymentOrderRepository paymentOrderRepository,
    IIdempotencyStore idempotencyStore,
    IClock clock)
{
    public async Task<CreatePaymentOrderResult> CreateAsync(
        CreatePaymentOrderRequest request,
        string idempotencyKey,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(idempotencyKey);

        var existingOrderId = await idempotencyStore.GetOrderIdAsync(idempotencyKey, cancellationToken);
        if (existingOrderId is Guid orderId)
        {
            return new CreatePaymentOrderResult(orderId, true);
        }

        var builder = paymentOrderFactory.CreateBuilder(request.OrderType);
        var order = builder
            .WithId(Guid.NewGuid())
            .WithSourceAccount(request.SourceAccount)
            .WithTargetAccount(request.TargetAccount)
            .WithAmount(request.Amount)
            .WithCurrency(request.Currency)
            .WithSwiftCode(request.SwiftCode)
            .WithScheduledFor(request.ScheduledFor)
            .WithCreatedAt(clock.UtcNow)
            .Build();

        await paymentOrderRepository.AddAsync(order, cancellationToken);
        var registered = await idempotencyStore.TryRegisterAsync(idempotencyKey, order.Id, clock.UtcNow, cancellationToken);
        if (registered)
        {
            return new CreatePaymentOrderResult(order.Id, false);
        }

        existingOrderId = await idempotencyStore.GetOrderIdAsync(idempotencyKey, cancellationToken);
        if (existingOrderId is Guid concurrentOrderId)
        {
            return new CreatePaymentOrderResult(concurrentOrderId, true);
        }

        throw new InvalidOperationException("The idempotency key could not be registered or resolved.");
    }

    public async Task<PaymentOrderResponse?> GetAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await paymentOrderRepository.GetByIdAsync(orderId, cancellationToken);
        return order is null ? null : Map(order);
    }

    public async Task<PagedResult<PaymentOrderResponse>> ListAsync(PaymentOrderFilter filter, CancellationToken cancellationToken)
    {
        var result = await paymentOrderRepository.ListAsync(filter, cancellationToken);
        return new PagedResult<PaymentOrderResponse>(result.Items.Select(Map).ToList(), result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<bool> CancelAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await paymentOrderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null)
        {
            return false;
        }

        order.Cancel();
        await paymentOrderRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ProcessAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await paymentOrderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null)
        {
            return false;
        }

        if (order.Status == PaymentOrderStatus.Created)
        {
            order.MoveToPending();
        }

        order.StartProcessing();
        var commissionStrategy = commissionStrategyFactory.GetFor(order.Type);
        var commission = commissionStrategy.Calculate(order);
        order.Complete(commission);

        await paymentOrderRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<PaymentOrderEventResponse>?> GetEventsAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var exists = await paymentOrderRepository.GetByIdAsync(orderId, cancellationToken);
        if (exists is null)
        {
            return null;
        }

        var events = await paymentOrderRepository.GetEventsAsync(orderId, cancellationToken);
        return events.Select(@event => new PaymentOrderEventResponse(@event.Id, @event.Type, @event.OccurredAt, @event.Description)).ToList();
    }

    private static PaymentOrderResponse Map(PaymentOrder order) => new(
        order.Id,
        order.Type,
        order.Status,
        order.SourceAccount,
        order.TargetAccount,
        order.Amount,
        order.Currency,
        order.SwiftCode,
        order.ScheduledFor,
        order.Commission,
        order.FailureReason,
        order.CreatedAt);
}
