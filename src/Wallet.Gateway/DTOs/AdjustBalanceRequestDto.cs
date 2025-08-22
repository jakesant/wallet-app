namespace Wallet.Gateway.DTOs
{
    public class AdjustBalanceRequestDto
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Strategy { get; set; } = string.Empty;
    }
}
