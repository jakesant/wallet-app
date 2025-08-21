using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallet.Domain.Entities;

namespace Wallet.Infrastructure.Configurations;

public sealed class ExchangeRateConfiguration : IEntityTypeConfiguration<ExchangeRate>
{
    public void Configure(EntityTypeBuilder<ExchangeRate> b)
    {
        b.ToTable("ExchangeRates", "dbo");

        b.HasKey(x => new { x.Date, x.BaseCurrency, x.CounterCurrency });

        b.Property(x => x.Date)
            .HasColumnType("date");

        b.Property(x => x.BaseCurrency)
            .IsRequired()
            .HasMaxLength(3)
            .IsFixedLength();

        b.Property(x => x.CounterCurrency)
            .IsRequired()
            .HasMaxLength(3)
            .IsFixedLength();

        b.Property(x => x.Rate)
            .HasPrecision(18, 9)
            .IsRequired();

        b.Property(x => x.CreatedAtUtc)
            .HasColumnType("datetime2(3)")
            .HasDefaultValueSql("SYSUTCDATETIME()");

        b.Property(x => x.UpdatedAtUtc)
            .HasColumnType("datetime2(3)")
            .HasDefaultValueSql("SYSUTCDATETIME()");
    }
}
