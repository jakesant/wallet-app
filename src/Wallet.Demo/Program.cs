using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wallet.Gateway;
using Wallet.Gateway.Interfaces;
using Wallet.Infrastructure;
using Wallet.Infrastructure.Data;
using Wallet.Infrastructure.Entities;
using Wallet.Infrastructure.Repository;

try
{
    var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false)
        .AddEnvironmentVariables()
        .Build();


    var services = new ServiceCollection();

    services.AddDbContext<AppDbContext>(opt =>
        opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));
    services.Configure<EcbClientOptions>(config.GetSection("Client"));

    services.AddHttpClient<IEcbClient, EcbClient>(client =>
    {
        client.BaseAddress = new Uri("https://www.ecb.europa.eu");
        client.Timeout = TimeSpan.FromSeconds(5);
    });

    services.AddScoped<ExchangeRateRepository>();

    var provider = services.BuildServiceProvider();

    using var scope = provider.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    var ecb = scope.ServiceProvider.GetRequiredService<IEcbClient>();
    var repo = scope.ServiceProvider.GetRequiredService<ExchangeRateRepository>();

    var snapshot = await ecb.GetLatestAsync();

    Console.WriteLine($"Base: {snapshot.BaseCurrency}");
    Console.WriteLine($"Date: {snapshot.TimestampUtc:yyyy-MM-dd}");
    Console.WriteLine($"Currencies: {snapshot.Rates.Count}");

    foreach (var code in new[] { "USD", "GBP", "JPY" })
    {
        if (snapshot.Rates.TryGetValue(code, out var rate))
            Console.WriteLine($"1 EUR = {rate} {code}");
    }

    var rates = snapshot.Rates.Select(r =>
        new ExchangeRate
        {
            Date = DateOnly.FromDateTime(snapshot.TimestampUtc),
            BaseCurrency = snapshot.BaseCurrency,
            CounterCurrency = r.Key,
            Rate = r.Value
        });

    await repo.UpsertRatesAsync(rates);

    Console.WriteLine("The exchange rates have been saved to the DB.");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
}
