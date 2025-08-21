using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Wallet.Infrastructure.Data;
using Wallet.Infrastructure.Entities;

namespace Wallet.Infrastructure.Repository
{
    public class ExchangeRateRepository
    {
        private readonly AppDbContext _db;

        public ExchangeRateRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task UpsertRatesAsync(IEnumerable<ExchangeRate> rates, CancellationToken cancellationToken = default)
        {
            if (!rates.Any())
                return;

            var sql = @"
                MERGE INTO ExchangeRates AS target
                USING (VALUES {0}) AS source (Date, BaseCurrency, CounterCurrency, Rate)
                ON target.Date = source.Date AND target.CounterCurrency = source.CounterCurrency
                WHEN MATCHED THEN
                    UPDATE SET target.Rate = source.Rate, target.BaseCurrency = source.BaseCurrency, target.UpdatedAtUtc = SYSUTCDATETIME()
                WHEN NOT MATCHED THEN
                    INSERT (Date, BaseCurrency, CounterCurrency, Rate, CreatedAtUtc, UpdatedAtUtc)
                    VALUES (source.Date, source.BaseCurrency, source.CounterCurrency, source.Rate, SYSUTCDATETIME(), SYSUTCDATETIME());";

            var parameters = new List<SqlParameter>();
            var valuesList = new List<string>();
            int i = 0;

            foreach (var rate in rates)
            {
                var pDate = new SqlParameter($"@pDate{i}", rate.Date);
                var pBase = new SqlParameter($"@pBase{i}", rate.BaseCurrency);
                var pCurrency = new SqlParameter($"@pCurrency{i}", rate.CounterCurrency);
                var pRate = new SqlParameter($"@pRate{i}", rate.Rate);

                parameters.AddRange(new[] { pDate, pBase, pCurrency, pRate });
                valuesList.Add($"(@pDate{i}, @pBase{i}, @pCurrency{i}, @pRate{i})");

                i++;
            }

            var finalSql = string.Format(sql, string.Join(", ", valuesList));

            await _db.Database.ExecuteSqlRawAsync(finalSql, parameters.ToArray(), cancellationToken);
        }
    }
}
