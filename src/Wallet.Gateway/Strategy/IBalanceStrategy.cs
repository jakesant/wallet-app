using Wallet.Domain.Entities;

namespace Wallet.Infrastructure.Strategy
{
    public interface IBalanceStrategy
    {
        Task AdjustAsync(WalletAccount wallet, decimal amount, CancellationToken cancellationToken = default);
    }
}
