using Quartz;
using Wallet.Domain.Entities;
using Wallet.Gateway;
using Wallet.Gateway.Interfaces;
using Wallet.Infrastructure.Repository;

namespace Wallet.Demo.Jobs
{
    public class RateUpdateJob : IJob
    {
        private readonly IEcbClient _ecbClient;
        private readonly ExchangeRateRepository _repository;
        private readonly CacheService _cache;

        public RateUpdateJob(IEcbClient ecbClient, ExchangeRateRepository repository, CacheService cacheService)
        {
            _ecbClient = ecbClient;
            _repository = repository;
            _cache = cacheService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var snapshot = await _ecbClient.GetLatestAsync(context.CancellationToken);

            var rates = snapshot.Rates.Select(kvp => new ExchangeRate
            {
                Date = DateOnly.FromDateTime(snapshot.TimestampUtc),
                BaseCurrency = snapshot.BaseCurrency,
                CounterCurrency = kvp.Key,
                Rate = kvp.Value
            });

            await _repository.UpsertRatesAsync(rates, context.CancellationToken);
            _cache.SetRates(rates);

            Console.WriteLine($"Background job: Updated {snapshot.Rates.Count} rates and cache at {DateTime.UtcNow}");
        }
    }
}
