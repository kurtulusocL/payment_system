
namespace PaymentSystem.Shared.Dtos.EntityDtos.PaymentDtos
{
    public class InitiatePaymentDto
    {
        public int MerchantId { get; set; }
        public decimal Amount { get; set; }
        public int CurrencyId { get; set; }
        public string CardNumber { get; set; } = string.Empty;
        public string IdempotencyKey { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
