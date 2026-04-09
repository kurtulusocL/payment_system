
namespace PaymentSystem.Shared.Dtos.MappingDtos.WalletDtos
{
    public class WalletGetDto
    {
        public int Id { get; set; }
        public decimal Balance { get; set; } = 0;
        public byte[]? RowVersion { get; set; }
        public string UserId { get; set; }
        public int CurrencyId { get; set; }
        public int TransactionCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DateTime? SuspendedDate { get; set; }
    }
}
