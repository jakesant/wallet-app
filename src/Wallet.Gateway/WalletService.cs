using Wallet.Domain.Entities;
using Wallet.Domain.Enums;
using Wallet.Gateway.DTOs;
using Wallet.Gateway.Interfaces;
using Wallet.Infrastructure.Repository;

namespace Wallet.Gateway
{
    public class WalletService
    {
        private readonly ExchangeRateRepository _exchangeRates;
        private readonly WalletAccountRepository _wallets;
        private readonly IBalanceStrategyResolver _resolver;
        private readonly CacheService _cache;

        public WalletService(
            ExchangeRateRepository exchangeRates,
            WalletAccountRepository wallets,
            IBalanceStrategyResolver resolver,
            CacheService cache)
        {
            _exchangeRates = exchangeRates;
            _wallets = wallets;
            _resolver = resolver;
            _cache = cache;
        }

        public async Task<WalletDto> AdjustBalanceAsync(long walletId, decimal amount, string currency, BalanceStrategyType strategy, CancellationToken cancellationToken = default)
        {
            if (amount <= 0) throw new ArgumentException("Adjustment amount must be positive.");

            var wallet = await _wallets.GetByIdAsync(walletId, cancellationToken)
                ?? throw new KeyNotFoundException("Wallet not found.");

            decimal finalAmount = amount;
            if (!string.Equals(wallet.Currency, currency, StringComparison.OrdinalIgnoreCase))
            {
                var cachedRate = _cache.GetRate(currency, wallet.Currency);

                if(cachedRate == null)
                {
                    var rate = await _exchangeRates.GetRateAsync(currency, wallet.Currency, DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);
                    if (rate == null)
                        throw new InvalidOperationException($"No exchange rate found for {currency} -> {wallet.Currency}");

                    cachedRate = rate.Value;
                    _cache.SetRate(currency, wallet.Currency, cachedRate.Value);
                }

                finalAmount = amount * cachedRate.Value;
            }

            var strat = _resolver.Resolve(strategy);
            await strat.AdjustAsync(wallet, finalAmount, cancellationToken);

            await _wallets.UpdateAsync(wallet, cancellationToken);
            return new WalletDto(wallet.Id, wallet.Balance, wallet.Currency);
        }

        public async Task<WalletDto> CreateWalletAsync(string currency, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency is required.");

            var wallet = new WalletAccount
            {
                Currency = currency,
                Balance = 0m
            };

            await _wallets.CreateAsync(wallet, cancellationToken);
            return new WalletDto(wallet.Id, wallet.Balance, wallet.Currency);
        }

        public async Task<WalletDto> GetWalletBalanceAsync(long walletId, string? targetCurrency, CancellationToken cancellationToken = default)
        {
            var wallet = await _wallets.GetByIdAsync(walletId, cancellationToken)
                ?? throw new KeyNotFoundException("Wallet not found.");

            decimal balance = wallet.Balance;
            string currency = wallet.Currency;

            if (!string.IsNullOrWhiteSpace(targetCurrency) && targetCurrency != wallet.Currency)
            {
                var cachedRate = _cache.GetRate(wallet.Currency, targetCurrency);
                if (cachedRate == null)
                {
                    var rate = await _exchangeRates.GetRateAsync(wallet.Currency, targetCurrency, DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);
                    if (rate == null)
                        throw new InvalidOperationException($"No exchange rate found for {wallet.Currency} -> {targetCurrency}");

                    cachedRate = rate.Value;
                    _cache.SetRate(wallet.Currency, targetCurrency, cachedRate.Value);
                }

                
                balance = balance * cachedRate.Value;
                currency = targetCurrency;
            }

            return new WalletDto(wallet.Id, balance, currency);
        }
    }
}
