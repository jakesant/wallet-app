using Wallet.Gateway;
using Wallet.Gateway.Models;
using Wallet.Gateway.Interfaces;

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
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
}
