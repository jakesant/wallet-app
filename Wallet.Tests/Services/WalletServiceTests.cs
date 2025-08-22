using Moq;
using System.Threading;
using Wallet.Domain.Entities;
using Wallet.Domain.Enums;
using Wallet.Gateway;
using Wallet.Gateway.DTOs;
using Wallet.Infrastructure.Repository;
using Wallet.Infrastructure.Strategy;
using Xunit;
using Microsoft.Extensions.Caching.Memory;

namespace Wallet.Tests.Services
{
    public class WalletServiceTests
    {
        private readonly Mock<ExchangeRateRepository> _exchangeRateRepo;
        private readonly Mock<WalletAccountRepository> _walletRepo;
        private readonly Mock<BalanceStrategyResolver> _resolver;
        private readonly Mock<IBalanceStrategy> _strategy;
        private readonly CacheService _cache;
        private readonly WalletService _service;

        public WalletServiceTests()
        {
            _exchangeRateRepo = new Mock<ExchangeRateRepository>(null!);
            _walletRepo = new Mock<WalletAccountRepository>(null!);
            _resolver = new Mock<BalanceStrategyResolver>(null!);
            _strategy = new Mock<IBalanceStrategy>();
            _cache = new CacheService(new MemoryCache(new MemoryCacheOptions()));

            _resolver.Setup(r => r.Resolve(It.IsAny<BalanceStrategyType>()))
                     .Returns(_strategy.Object);

            _service = new WalletService(_exchangeRateRepo.Object, _walletRepo.Object, _resolver.Object, _cache);
        }

        [Fact]
        public async Task CreateWallet_ShouldReturnWalletDto()
        {
            var dto = await _service.CreateWalletAsync("EUR");
            Assert.Equal("EUR", dto.Currency);
            Assert.Equal(0, dto.Balance);
        }

        [Fact]
        public async Task AdjustBalance_ShouldUseStrategy()
        {
            var wallet = new WalletAccount { Id = 1, Currency = "EUR", Balance = 100m };

            _walletRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(wallet);

            _strategy.Setup(s => s.AdjustAsync(wallet, 50m, It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

            var result = await _service.AdjustBalanceAsync(1, 50m, "EUR", BalanceStrategyType.AddFunds);

            _strategy.Verify(s => s.AdjustAsync(wallet, 50m, It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(1, result.Id);
        }
    }
}
