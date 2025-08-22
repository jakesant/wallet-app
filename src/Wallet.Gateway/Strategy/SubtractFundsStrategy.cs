using Wallet.Domain.Entities;

namespace Wallet.Infrastructure.Strategy
{
    public class SubtractFundsStrategy : IBalanceStrategy
    {
        public Task AdjustAsync(WalletAccount wallet, decimal amount, CancellationToken cancellationToken = default)
        {
            if (wallet.Balance < amount)
                throw new InvalidOperationException("Insufficient funds.");

            wallet.Balance -= amount;
            return Task.CompletedTask;
        }
    }
}
