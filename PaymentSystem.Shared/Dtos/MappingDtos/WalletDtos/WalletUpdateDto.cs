
namespace PaymentSystem.Shared.Dtos.MappingDtos.WalletDtos
{
    public class WalletUpdateDto
    {
        public int Id { get; set; }
        public decimal Balance { get; set; } = 0;
        public byte[]? RowVersion { get; set; }
        public string UserId { get; set; }
        public int CurrencyId { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
