namespace Wallet.Gateway
{
    public class EcbClientOptions
    {
        public string BaseUrl { get; init; } = "https://www.ecb.europa.eu";
        public string Path { get; init; } = "/stats/eurofxref/eurofxref-daily.xml";
        public int TimeoutSeconds { get; init; } = 5;
    }
}
