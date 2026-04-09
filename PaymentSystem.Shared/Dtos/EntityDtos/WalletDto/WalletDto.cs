
namespace PaymentSystem.Shared.Dtos.EntityDtos.WalletDto
{
    public class WalletDto
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; } = string.Empty;
    }
}
