using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Gateway.Interfaces;
using Wallet.Domain.Entities;
using Wallet.Infrastructure.Repository;

namespace Wallet.Demo.Jobs
{
    public class RateUpdateJob : IJob
    {
        private readonly IEcbClient _ecbClient;
        private readonly ExchangeRateRepository _repository;

        public RateUpdateJob(IEcbClient ecbClient, ExchangeRateRepository repository)
        {
            _ecbClient = ecbClient;
            _repository = repository;
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

            Console.WriteLine($"Background job: Updated {snapshot.Rates.Count} rates at {DateTime.UtcNow}");
        }
    }
}
