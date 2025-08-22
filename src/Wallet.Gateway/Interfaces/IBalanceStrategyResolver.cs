using Wallet.Domain.Enums;
using Wallet.Infrastructure.Strategy;

namespace Wallet.Gateway.Interfaces
{
    public interface IBalanceStrategyResolver
    {
        IBalanceStrategy Resolve(BalanceStrategyType strategyType);
    }
}
