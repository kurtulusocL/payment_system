
namespace PaymentSystem.Shared.Dtos.EntityDtos.TransactionDtos
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Reference { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
