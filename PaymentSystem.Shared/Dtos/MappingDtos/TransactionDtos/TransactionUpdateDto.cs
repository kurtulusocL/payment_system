
namespace PaymentSystem.Shared.Dtos.MappingDtos.TransactionDtos
{
    public class TransactionUpdateDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string? Reference { get; set; }
        public int WalletId { get; set; }
        public int? PaymentId { get; set; }
        public int CurrencyId { get; set; }
        public int TransactionTypeId { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
