using Microsoft.EntityFrameworkCore;
using PaymentOrders.Application.Contracts;
using PaymentOrders.Application.Ports;
using PaymentOrders.Domain.Orders;
using PaymentOrders.Infrastructure.Persistence;

namespace PaymentOrders.Infrastructure.Repositories;

public sealed class EfPaymentOrderRepository(PaymentOrdersDbContext dbContext) : IPaymentOrderRepository
{
    public async Task AddAsync(PaymentOrder order, CancellationToken cancellationToken) =>
        await dbContext.PaymentOrders.AddAsync(order, cancellationToken);

    public Task<PaymentOrder?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken) =>
        dbContext.PaymentOrders
            .Include(order => order.AuditEvents)
            .SingleOrDefaultAsync(order => order.Id == orderId, cancellationToken);

    public async Task<PagedResult<PaymentOrder>> ListAsync(PaymentOrderFilter filter, CancellationToken cancellationToken)
    {
        var page = Math.Max(filter.Page, 1);
        var pageSize = Math.Clamp(filter.PageSize, 1, 100);
        IQueryable<PaymentOrder> query = dbContext.PaymentOrders.AsNoTracking();

        if (filter.OrderType is OrderType orderType)
        {
            query = query.Where(order => order.Type == orderType);
        }

        if (filter.Status is PaymentOrderStatus status)
        {
            query = query.Where(order => order.Status == status);
        }

        if (filter.CreatedFrom is DateTimeOffset createdFrom)
        {
            query = query.Where(order => order.CreatedAt >= createdFrom);
        }

        if (filter.CreatedTo is DateTimeOffset createdTo)
        {
            query = query.Where(order => order.CreatedAt <= createdTo);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(order => order.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<PaymentOrder>(items, totalCount, page, pageSize);
    }

    public async Task<IReadOnlyList<PaymentOrderEvent>> GetEventsAsync(Guid orderId, CancellationToken cancellationToken) =>
        await dbContext.PaymentOrderEvents
            .AsNoTracking()
            .Where(@event => @event.PaymentOrderId == orderId)
            .OrderBy(@event => @event.OccurredAt)
            .ToListAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}
