using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentOrders.Domain.Orders;

namespace PaymentOrders.Infrastructure.Persistence.Configurations;

public sealed class PaymentOrderConfiguration : IEntityTypeConfiguration<PaymentOrder>
{
    public void Configure(EntityTypeBuilder<PaymentOrder> builder)
    {
        builder.ToTable("payment_orders");
        builder.HasKey(order => order.Id);
        builder.Property(order => order.Id).ValueGeneratedNever();
        builder.Property(order => order.Type).HasConversion<int>().ValueGeneratedNever();
        builder.Property(order => order.Status).HasConversion<int>();
        builder.Property(order => order.Amount).HasPrecision(18, 2);
        builder.Property(order => order.Commission).HasPrecision(18, 2);
        builder.Property(order => order.SourceAccount).HasMaxLength(64).IsRequired();
        builder.Property(order => order.TargetAccount).HasMaxLength(64).IsRequired();
        builder.Property(order => order.Currency).HasMaxLength(3).IsRequired();
        builder.Property(order => order.SwiftCode).HasMaxLength(11);
        builder.Property(order => order.ScheduledFor).HasConversion<UtcDateTimeOffsetConverter>();
        builder.Property(order => order.FailureReason).HasMaxLength(500);
        builder.Property(order => order.CreatedAt).HasConversion<UtcDateTimeOffsetConverter>().IsRequired();

        builder.HasDiscriminator(order => order.Type)
            .HasValue<NationalPaymentOrder>(OrderType.National)
            .HasValue<InternationalPaymentOrder>(OrderType.International)
            .HasValue<ScheduledPaymentOrder>(OrderType.Scheduled);

        builder.HasMany(order => order.AuditEvents)
            .WithOne()
            .HasForeignKey(@event => @event.PaymentOrderId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(order => order.AuditEvents).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
