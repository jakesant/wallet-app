using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Domain.Entities;

namespace Wallet.Gateway
{
    public class CacheService
    {
        private readonly IMemoryCache _cache;
        private const string CacheKeyPrefix = "rate_";

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void SetRate(string baseCurrency, string counterCurrency, decimal rate)
        {
            var key = BuildKey(baseCurrency, counterCurrency);
            _cache.Set(key, rate, TimeSpan.FromMinutes(30));
        }

        public decimal? GetRate(string baseCurrency, string counterCurrency)
        {
            var key = BuildKey(baseCurrency, counterCurrency);
            return _cache.TryGetValue<decimal>(key, out var rate) ? rate : null;
        }

        public void SetRates(IEnumerable<ExchangeRate> rates)
        {
            foreach (var r in rates)
            {
                SetRate(r.BaseCurrency, r.CounterCurrency, r.Rate);
            }
        }

        private static string BuildKey(string baseCurrency, string counterCurrency) =>
            $"{CacheKeyPrefix}{baseCurrency}_{counterCurrency}".ToUpperInvariant();
    }
}
