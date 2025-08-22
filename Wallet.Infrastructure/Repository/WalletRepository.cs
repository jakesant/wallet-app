using Wallet.Infrastructure.Data;
using Wallet.Domain.Entities;

namespace Wallet.Infrastructure.Repository
{
    public class WalletAccountRepository
    {
        private readonly AppDbContext _db;

        public WalletAccountRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<WalletAccount?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _db.WalletAccounts.FindAsync([id], cancellationToken);
        }

        public async Task CreateAsync(WalletAccount wallet, CancellationToken cancellationToken = default)
        {
            _db.WalletAccounts.Add(wallet);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(WalletAccount wallet, CancellationToken cancellationToken = default)
        {
            _db.WalletAccounts.Update(wallet);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
