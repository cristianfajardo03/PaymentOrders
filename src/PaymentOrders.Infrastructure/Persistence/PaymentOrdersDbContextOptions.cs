using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace PaymentOrders.Infrastructure.Persistence;

public static class PaymentOrdersDbContextOptions
{
    public const string SqliteProvider = "Sqlite";
    public const string MySqlProvider = "MySql";

    public static DbContextOptionsBuilder Configure(
        DbContextOptionsBuilder options,
        string provider,
        string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(provider);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        switch (provider.Trim().ToLowerInvariant())
        {
            case "sqlite":
                options.UseSqlite(connectionString, sqliteOptions =>
                    sqliteOptions.MigrationsAssembly("PaymentOrders.Infrastructure"));
                break;

            case "mysql":
                options.UseMySql(
                    connectionString,
                    new MySqlServerVersion(new Version(8, 0, 0)),
                    mySqlOptions => mySqlOptions.MigrationsAssembly("PaymentOrders.Infrastructure.MySqlMigrations"));
                break;

            default:
                throw new InvalidOperationException(
                    $"Unsupported database provider '{provider}'. Use '{SqliteProvider}' or '{MySqlProvider}'.");
        }

        return options;
    }
}
