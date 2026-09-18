using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PaymentOrders.Application.Ports;
using PaymentOrders.Infrastructure;
using PaymentOrders.Infrastructure.Persistence;

namespace PaymentOrders.Api.Tests;

public sealed class ApiStartupTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Swagger_is_available_without_an_external_database()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/swagger/index.html");

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public void MySql_provider_can_be_selected_without_connecting_during_service_registration()
    {
        var services = new ServiceCollection();
        services.AddPaymentOrdersInfrastructure(
            "Server=localhost;Port=3306;Database=payment_orders;User=root;Password=not-used",
            PaymentOrdersDbContextOptions.MySqlProvider);
        using var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PaymentOrdersDbContext>();

        Assert.Equal("Pomelo.EntityFrameworkCore.MySql", dbContext.Database.ProviderName);
    }

    [Fact]
    public void MySql_migration_is_discoverable_when_the_provider_is_selected()
    {
        var services = new ServiceCollection();
        services.AddPaymentOrdersInfrastructure(
            "Server=localhost;Port=3306;Database=payment_orders;User=root;Password=not-used",
            PaymentOrdersDbContextOptions.MySqlProvider);
        using var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PaymentOrdersDbContext>();

        var migrations = dbContext.Database.GetMigrations();

        Assert.Contains(migrations, migration => migration.EndsWith("_InitialCreateMySql", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Listing_orders_is_available_with_the_local_database()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/payment-orders");

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Initial_migration_is_applied_to_a_new_database()
    {
        var databasePath = Path.Combine(Path.GetTempPath(), $"payment-orders-{Guid.NewGuid():N}.db");

        try
        {
            var services = new ServiceCollection();
            services.AddPaymentOrdersInfrastructure($"Data Source={databasePath};Pooling=False");
            await using (var serviceProvider = services.BuildServiceProvider())
            {
                await using (var scope = serviceProvider.CreateAsyncScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<PaymentOrdersDbContext>();

                    await dbContext.Database.MigrateAsync();
                    var appliedMigrations = await dbContext.Database.GetAppliedMigrationsAsync();

                    Assert.Contains(appliedMigrations, migration => migration.EndsWith("_InitialCreate", StringComparison.Ordinal));
                }
            }
        }
        finally
        {
            File.Delete(databasePath);
        }
    }

    [Fact]
    public async Task Idempotency_store_rejects_a_repeated_key()
    {
        var databasePath = Path.Combine(Path.GetTempPath(), $"payment-orders-{Guid.NewGuid():N}.db");

        try
        {
            var services = new ServiceCollection();
            services.AddPaymentOrdersInfrastructure($"Data Source={databasePath};Pooling=False");
            await using (var serviceProvider = services.BuildServiceProvider())
            {
                await using (var scope = serviceProvider.CreateAsyncScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<PaymentOrdersDbContext>();
                    await dbContext.Database.MigrateAsync();
                    var store = scope.ServiceProvider.GetRequiredService<IIdempotencyStore>();
                    var key = "same-request";

                    var firstRegistration = await store.TryRegisterAsync(key, Guid.NewGuid(), DateTimeOffset.UtcNow, CancellationToken.None);
                    var repeatedRegistration = await store.TryRegisterAsync(key, Guid.NewGuid(), DateTimeOffset.UtcNow, CancellationToken.None);

                    Assert.True(firstRegistration);
                    Assert.False(repeatedRegistration);
                }
            }
        }
        finally
        {
            File.Delete(databasePath);
        }
    }
}
