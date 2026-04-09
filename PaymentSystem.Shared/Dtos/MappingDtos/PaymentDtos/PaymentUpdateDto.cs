
namespace PaymentSystem.Shared.Dtos.MappingDtos.PaymentDtos
{
    public class PaymentUpdateDto
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
        public DateTime? UpdatedDate { get; set; }
    }
}
