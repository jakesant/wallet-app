using Wallet.Domain.Entities;

namespace Wallet.Infrastructure.Repository
{
    public interface IWalletAccountRepository
    {
        Task<WalletAccount?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task CreateAsync(WalletAccount wallet, CancellationToken cancellationToken = default);
        Task UpdateAsync(WalletAccount wallet, CancellationToken cancellationToken = default);
    }
}