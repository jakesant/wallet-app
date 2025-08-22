using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Wallet.Gateway.DTOs;
using Xunit;

namespace Wallet.Tests.Controllers
{
    public class WalletAccountsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public WalletAccountsControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateWallet_ShouldReturn201()
        {
            var response = await _client.PostAsync("/api/walletaccounts?currency=EUR", null);
            response.EnsureSuccessStatusCode();

            var dto = await response.Content.ReadFromJsonAsync<WalletDto>();
            Assert.NotNull(dto);
            Assert.Equal("EUR", dto!.Currency);
        }

        [Fact]
        public async Task AdjustBalance_ShouldReturn200()
        {
            var createResponse = await _client.PostAsync("/api/walletaccounts?currency=EUR", null);
            var wallet = await createResponse.Content.ReadFromJsonAsync<WalletDto>();

            var response = await _client.PostAsync($"/api/walletaccounts/{wallet!.Id}/adjustbalance?amount=50&currency=EUR&strategy=AddFunds", null);
            response.EnsureSuccessStatusCode();

            var dto = await response.Content.ReadFromJsonAsync<WalletDto>();
            Assert.NotNull(dto);
            Assert.Equal("EUR", dto!.Currency);
            Assert.Equal(50, dto.Balance);
        }
    }
}
