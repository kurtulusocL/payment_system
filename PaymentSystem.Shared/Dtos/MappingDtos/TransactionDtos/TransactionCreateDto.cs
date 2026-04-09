
namespace PaymentSystem.Shared.Dtos.MappingDtos.TransactionDtos
{
    public class TransactionCreateDto
    {
        public decimal Amount { get; set; }
        public string? Reference { get; set; }
        public int WalletId { get; set; }
        public int? PaymentId { get; set; }
        public int CurrencyId { get; set; }
        public int TransactionTypeId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
