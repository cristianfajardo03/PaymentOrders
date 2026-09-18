using Microsoft.EntityFrameworkCore;
using PaymentOrders.Domain.Orders;

namespace PaymentOrders.Infrastructure.Persistence;

public sealed class PaymentOrdersDbContext(DbContextOptions<PaymentOrdersDbContext> options) : DbContext(options)
{
    public DbSet<PaymentOrder> PaymentOrders => Set<PaymentOrder>();

    public DbSet<PaymentOrderEvent> PaymentOrderEvents => Set<PaymentOrderEvent>();

    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentOrdersDbContext).Assembly);
    }

    public override int SaveChanges()
    {
        EnsureAuditEventsAreAppendOnly();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        EnsureAuditEventsAreAppendOnly();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void EnsureAuditEventsAreAppendOnly()
    {
        var invalidEntry = ChangeTracker.Entries<PaymentOrderEvent>()
            .FirstOrDefault(entry => entry.State is EntityState.Modified or EntityState.Deleted);

        if (invalidEntry is not null)
        {
            throw new InvalidOperationException("Payment-order audit events are immutable and append-only.");
        }
    }
}
