using Wallet.Domain.Entities;

namespace Wallet.Infrastructure.Strategy
{
    public class ForceSubtractFundsStrategy : IBalanceStrategy
    {
        public Task AdjustAsync(WalletAccount wallet, decimal amount, CancellationToken cancellationToken = default)
        {
            wallet.Balance -= amount;
            return Task.CompletedTask;
        }
    }
}
