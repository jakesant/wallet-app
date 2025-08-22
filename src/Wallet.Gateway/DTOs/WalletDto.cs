namespace Wallet.Gateway.DTOs
{
    public class WalletDto
    {
        public WalletDto(long id, decimal balance, string currency)
        {
            Id = id;
            Balance = balance;
            Currency = currency;
        }

        public long Id { get; set; }
        public string Currency { get; set; } = null!;
        public decimal Balance { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
