
namespace PaymentSystem.Shared.Dtos.MappingDtos.PaymentDtos
{
    public class PaymentGetDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string IdempotencyKey { get; set; }
        public string? Description { get; set; }
        public string? MaskedCardNumber { get; set; }
        public string UserId { get; set; }
        public int MerchantId { get; set; }
        public int CurrencyId { get; set; }
        public int PaymentStatusId { get; set; }
        public int TransactionCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DateTime? SuspendedDate { get; set; }
    }
}
