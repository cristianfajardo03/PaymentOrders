using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PaymentOrders.Application.Common;
using PaymentOrders.Application.Ports;
using PaymentOrders.Infrastructure.Persistence;
using PaymentOrders.Infrastructure.Repositories;
using PaymentOrders.Infrastructure.Services;

namespace PaymentOrders.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPaymentOrdersInfrastructure(this IServiceCollection services, string connectionString) =>
        services.AddPaymentOrdersInfrastructure(connectionString, PaymentOrdersDbContextOptions.SqliteProvider);

    public static IServiceCollection AddPaymentOrdersInfrastructure(
        this IServiceCollection services,
        string connectionString,
        string provider)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.AddDbContext<PaymentOrdersDbContext>(options =>
            PaymentOrdersDbContextOptions.Configure(options, provider, connectionString));
        services.AddScoped<IPaymentOrderRepository, EfPaymentOrderRepository>();
        services.AddScoped<IIdempotencyStore, EfIdempotencyStore>();
        services.AddSingleton<IClock, SystemClock>();

        return services;
    }
}
