using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using MySqlConnector;
using PaymentOrders.Application.Ports;
using PaymentOrders.Infrastructure.Persistence;

namespace PaymentOrders.Infrastructure.Repositories;

public sealed class EfIdempotencyStore(PaymentOrdersDbContext dbContext) : IIdempotencyStore
{
    public Task<Guid?> GetOrderIdAsync(string key, CancellationToken cancellationToken) =>
        dbContext.IdempotencyRecords
            .AsNoTracking()
            .Where(record => record.Key == key.Trim())
            .Select(record => (Guid?)record.PaymentOrderId)
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<bool> TryRegisterAsync(string key, Guid orderId, DateTimeOffset createdAt, CancellationToken cancellationToken)
    {
        var normalizedKey = key.Trim();

        if (await dbContext.IdempotencyRecords.AnyAsync(record => record.Key == normalizedKey, cancellationToken))
        {
            return false;
        }

        dbContext.IdempotencyRecords.Add(new IdempotencyRecord(normalizedKey, orderId, createdAt));

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException exception) when (
            exception.InnerException is SqliteException { SqliteErrorCode: 19 }
            or MySqlException { Number: 1062 })
        {
            dbContext.ChangeTracker.Clear();
            return false;
        }
    }
}
