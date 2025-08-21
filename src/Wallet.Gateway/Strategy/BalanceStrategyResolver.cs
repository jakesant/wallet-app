using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Infrastructure.Strategy
{
    public class BalanceStrategyResolver
    {
        private readonly IServiceProvider _provider;

        public BalanceStrategyResolver(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IBalanceStrategy Resolve(string strategyType)
        {
            return strategyType.ToLower() switch
            {
                "forcesubtractfundsstrategy" => _provider.GetRequiredService<ForceSubtractFundsStrategy>(),
                "subtractfundsstrategy" => _provider.GetRequiredService<SubtractFundsStrategy>(),
                "addfundsstrategy" => _provider.GetRequiredService<AddFundsStrategy>(),
                _ => throw new ArgumentOutOfRangeException(nameof(strategyType), $"Unknown balance strategy type: {strategyType}")
            };
        }
    }
}
