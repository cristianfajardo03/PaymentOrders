using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PaymentOrders.Infrastructure.Persistence;

public sealed class PaymentOrdersDbContextFactory : IDesignTimeDbContextFactory<PaymentOrdersDbContext>
{
    public PaymentOrdersDbContext CreateDbContext(string[] args)
    {
        var provider = Environment.GetEnvironmentVariable("Database__Provider")
            ?? PaymentOrdersDbContextOptions.SqliteProvider;
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__PaymentOrders")
            ?? "Data Source=payment-orders.local.db";
        var options = new DbContextOptionsBuilder<PaymentOrdersDbContext>();

        PaymentOrdersDbContextOptions.Configure(options, provider, connectionString);

        return new PaymentOrdersDbContext(options.Options);
    }
}
