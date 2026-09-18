using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentOrders.Infrastructure.Persistence;

namespace PaymentOrders.Infrastructure.Persistence.Configurations;

public sealed class IdempotencyRecordConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
{
    public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
    {
        builder.ToTable("idempotency_records");
        builder.HasKey(record => record.Key);
        builder.Property(record => record.Key).HasMaxLength(128).ValueGeneratedNever();
        builder.Property(record => record.PaymentOrderId).IsRequired();
        builder.Property(record => record.CreatedAt).HasConversion<UtcDateTimeOffsetConverter>().IsRequired();
        builder.HasIndex(record => record.PaymentOrderId);
    }
}
