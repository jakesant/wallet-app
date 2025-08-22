namespace Wallet.Domain.Entities
{
    public class WalletAccount
    {
        public long Id { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; } = string.Empty;

        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
