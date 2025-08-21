using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Domain;
using Wallet.Infrastructure.Repository;
using Wallet.Infrastructure.Strategy;

namespace Wallet.Gateway
{
    public class WalletService
    {
        private readonly ExchangeRateRepository _exchangeRates;
        private readonly WalletAccountRepository _wallets;
        private readonly BalanceStrategyResolver _resolver;

        public WalletService(
            ExchangeRateRepository exchangeRates,
            WalletAccountRepository wallets,
            BalanceStrategyResolver resolver)
        {
            _exchangeRates = exchangeRates;
            _wallets = wallets;
            _resolver = resolver;
        }

        public async Task AdjustBalanceAsync(long walletId, decimal amount, string strategy, CancellationToken ct = default)
        {
            if (amount <= 0) throw new ArgumentException("Adjustment amount must be positive.");

            var wallet = await _wallets.GetByIdAsync(walletId, ct)
                ?? throw new InvalidOperationException("Wallet not found.");

            var strat = _resolver.Resolve(strategy);
            await strat.AdjustAsync(wallet, amount, ct);

            await _wallets.UpdateAsync(wallet, ct);
        }
    }
}
