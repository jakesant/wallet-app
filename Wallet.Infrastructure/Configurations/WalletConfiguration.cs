using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Wallet.Infrastructure.Configurations
{
    public class WalletConfiguration : IEntityTypeConfiguration<WalletAccount>
    {
        public void Configure(EntityTypeBuilder<WalletAccount> builder)
        {
            builder.HasKey(w => w.Id);

            builder.Property(w => w.Balance)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(w => w.Currency)
                   .HasMaxLength(3)
                   .IsRequired();

            builder.Property(w => w.CreatedAtUtc)
                   .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.Property(w => w.UpdatedAtUtc)
                   .HasDefaultValueSql("SYSUTCDATETIME()");
        }
    }
}
