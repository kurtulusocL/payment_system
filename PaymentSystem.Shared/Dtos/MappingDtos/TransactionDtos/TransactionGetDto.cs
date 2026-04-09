
namespace PaymentSystem.Shared.Dtos.MappingDtos.TransactionDtos
{
    public class TransactionGetDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string? Reference { get; set; }
        public int WalletId { get; set; }
        public int? PaymentId { get; set; }
        public int CurrencyId { get; set; }
        public int TransactionTypeId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DateTime? SuspendedDate { get; set; }
    }
}
