using Microsoft.EntityFrameworkCore;
using Wallet.Infrastructure.Configurations;
using Wallet.Infrastructure.Entities;

namespace Wallet.Infrastructure.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ExchangeRate> ExchangeRates => Set<ExchangeRate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ExchangeRateConfiguration());
    }
}
