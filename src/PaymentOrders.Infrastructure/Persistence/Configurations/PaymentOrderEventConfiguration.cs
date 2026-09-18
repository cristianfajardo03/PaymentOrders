using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentOrders.Domain.Orders;

namespace PaymentOrders.Infrastructure.Persistence.Configurations;

public sealed class PaymentOrderEventConfiguration : IEntityTypeConfiguration<PaymentOrderEvent>
{
    public void Configure(EntityTypeBuilder<PaymentOrderEvent> builder)
    {
        builder.ToTable("payment_order_events");
        builder.HasKey(@event => @event.Id);
        builder.Property(@event => @event.Id).ValueGeneratedNever();
        builder.Property(@event => @event.PaymentOrderId).IsRequired();
        builder.Property(@event => @event.Type).HasConversion<int>().IsRequired();
        builder.Property(@event => @event.OccurredAt).HasConversion<UtcDateTimeOffsetConverter>().IsRequired();
        builder.Property(@event => @event.Description).HasMaxLength(500).IsRequired();
        builder.HasIndex(@event => new { @event.PaymentOrderId, @event.OccurredAt });
    }
}
