using Microsoft.Extensions.DependencyInjection;
using Wallet.Domain.Enums;
using Wallet.Gateway.Interfaces;

namespace Wallet.Infrastructure.Strategy
{
    public class BalanceStrategyResolver : IBalanceStrategyResolver
    {
        private readonly IServiceProvider _provider;

        public BalanceStrategyResolver(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IBalanceStrategy Resolve(BalanceStrategyType strategyType)
        {
            return strategyType switch
            {
                BalanceStrategyType.ForceSubtractFunds => _provider.GetRequiredService<ForceSubtractFundsStrategy>(),
                BalanceStrategyType.SubtractFunds => _provider.GetRequiredService<SubtractFundsStrategy>(),
                BalanceStrategyType.AddFunds => _provider.GetRequiredService<AddFundsStrategy>(),
                _ => throw new ArgumentOutOfRangeException(nameof(strategyType), $"Unknown balance strategy type: {strategyType}")
            };
        }
    }
}
