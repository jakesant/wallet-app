namespace Wallet.Gateway.Models
{
    public class CurrencyRate(
        string baseCurrency,
        DateTime timestampUtc,
        IReadOnlyDictionary<string, decimal> rates)
    {
        public string BaseCurrency { get; } = baseCurrency;
        public DateTime TimestampUtc { get; } = timestampUtc;
        public IReadOnlyDictionary<string, decimal> Rates { get; } = rates;

        public bool TryGet(string currency, out decimal rate) =>
            Rates.TryGetValue(currency, out rate);

    }
}
