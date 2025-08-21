using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wallet.Infrastructure.Configurations;
using Wallet.Domain.Entities;

namespace Wallet.Infrastructure.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));

        return new AppDbContext(optionsBuilder.Options);
    }

    public DbSet<ExchangeRate> ExchangeRates => Set<ExchangeRate>();
    public DbSet<WalletAccount> WalletAccounts => Set<WalletAccount>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ExchangeRateConfiguration());
        modelBuilder.ApplyConfiguration(new WalletConfiguration());
    }
}
