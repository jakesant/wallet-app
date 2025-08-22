using Microsoft.AspNetCore.Mvc;
using Wallet.Domain.Enums;
using Wallet.Gateway;
using Wallet.Gateway.DTOs;

namespace Wallet.Demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalletAccountsController : ControllerBase
    {
        private readonly WalletService _walletService;

        public WalletAccountsController(WalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpPost]
        public async Task<ActionResult<WalletDto>> CreateWallet([FromQuery] string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                return BadRequest("Wallet Currency is required.");

            var wallet = await _walletService.CreateWalletAsync(currency);
            return CreatedAtAction(nameof(GetWalletBalance), new { walletId = wallet.Id }, wallet);
        }

        [HttpGet("{walletId:long}")]
        public async Task<ActionResult<WalletDto>> GetWalletBalance(long walletId, [FromQuery] string? currency)
        {
            try
            {
                var balance = await _walletService.GetWalletBalanceAsync(walletId, currency);
                return Ok(balance);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{walletId:long}/adjustbalance")]
        public async Task<ActionResult<WalletDto>> AdjustBalance(
            long walletId,
            [FromQuery] decimal amount,
            [FromQuery] string currency,
            [FromQuery] BalanceStrategyType strategy)
        {
            if (amount <= 0)
                return BadRequest("Amount must be a positive number.");

            try
            {
                var wallet = await _walletService.AdjustBalanceAsync(walletId, amount, currency, strategy);
                return Ok(wallet);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return UnprocessableEntity(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
