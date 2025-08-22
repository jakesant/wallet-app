using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Domain;
using Wallet.Domain.Entities;
using Wallet.Gateway.DTOs;
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

        public async Task<WalletDto> AdjustBalanceAsync(long walletId, decimal amount, string currency, string strategy, CancellationToken cancellationToken = default)
        {
            if (amount <= 0) throw new ArgumentException("Adjustment amount must be positive.");

            var wallet = await _wallets.GetByIdAsync(walletId, cancellationToken)
                ?? throw new KeyNotFoundException("Wallet not found.");

            decimal finalAmount = amount;
            if (!string.Equals(wallet.Currency, currency, StringComparison.OrdinalIgnoreCase))
            {
                var rate = await _exchangeRates.GetRateAsync(currency, wallet.Currency, DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);
                if (rate == null)
                    throw new InvalidOperationException($"No exchange rate found for {currency} -> {wallet.Currency}");

                finalAmount = amount * rate.Value;
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
                var rate = await _exchangeRates.GetRateAsync(wallet.Currency, targetCurrency, DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);
                if (rate == null)
                    throw new InvalidOperationException($"No exchange rate found for {wallet.Currency} -> {targetCurrency}");

                balance = balance * rate.Value;
                currency = targetCurrency;
            }

            return new WalletDto(wallet.Id, balance, currency);
        }
    }
}
