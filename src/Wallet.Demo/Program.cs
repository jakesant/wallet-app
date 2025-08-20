using Microsoft.EntityFrameworkCore;
using Wallet.Gateway;
using Wallet.Gateway.Interfaces;
using Wallet.Infrastructure;
using Wallet.Infrastructure.Data;
using Wallet.Infrastructure.Entities;
using Wallet.Infrastructure.Repository;

try
{
    using var http = new HttpClient();

    var options = new EcbClientOptions
    {
        BaseUrl = "https://www.ecb.europa.eu",
        Path = "/stats/eurofxref/eurofxref-daily.xml",
        TimeoutSeconds = 5
    };

    IEcbClient ecb = new EcbClient(options, http);

    var snapshot = await ecb.GetLatestAsync();

    Console.WriteLine($"Base: {snapshot.BaseCurrency}");
    Console.WriteLine($"Date: {snapshot.TimestampUtc:yyyy-MM-dd}");
    Console.WriteLine($"Currencies: {snapshot.Rates.Count}");

    foreach (var code in new[] { "USD", "GBP", "JPY" })
    {
        if (snapshot.Rates.TryGetValue(code, out var rate))
            Console.WriteLine($"1 EUR = {rate} {code}");
    }

    var conn = "Server=localhost;Database=WalletTaskDb;Trusted_Connection=True;TrustServerCertificate=True;"; //move to options
    var dbOptions = new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlServer(conn)
        .Options;

    using var db = new AppDbContext(dbOptions);
    var upserter = new ExchangeRateRepository(db);
    var rates = snapshot.Rates.Select(r =>
        new ExchangeRate
        {
            Date = DateOnly.FromDateTime(snapshot.TimestampUtc),
            BaseCurrency = snapshot.BaseCurrency,
            CounterCurrency = r.Key,
            Rate = r.Value
        });

    await upserter.UpsertRatesAsync(rates);

    Console.WriteLine("The exchange rates have been saved to the DB.");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
}
