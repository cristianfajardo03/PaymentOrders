using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentOrders.Application.Ports;
using PaymentOrders.Application.Services;
using PaymentOrders.Domain.Orders.Patterns;
using PaymentOrders.Infrastructure;
using PaymentOrders.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
var databaseProvider = builder.Configuration["Database:Provider"] ?? PaymentOrdersDbContextOptions.SqliteProvider;
var connectionString = builder.Configuration.GetConnectionString("PaymentOrders")
    ?? "Data Source=payment-orders.local.db";

builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddPaymentOrdersInfrastructure(connectionString, databaseProvider);
builder.Services.AddScoped<IPaymentOrderFactory, PaymentOrderFactory>();
builder.Services.AddScoped<ICommissionStrategyFactory, CommissionStrategyFactory>();
builder.Services.AddScoped<PaymentOrderService>();

var app = builder.Build();

app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    var problem = Results.Problem(
        title: "An unexpected error occurred.",
        detail: app.Environment.IsDevelopment() ? error?.Message : null,
        statusCode: StatusCodes.Status500InternalServerError);
    await problem.ExecuteAsync(context);
}));

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PaymentOrdersDbContext>();
    await dbContext.Database.MigrateAsync();
}

await app.RunAsync();

public partial class Program;
