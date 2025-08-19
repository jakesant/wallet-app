using Wallet.Gateway.Models;

namespace Wallet.Gateway.Interfaces
{
    public interface IEcbClient
    {
        Task<CurrencyRate> GetLatestAsync(CancellationToken cancellationToken = default);
    }
}
