
namespace PaymentSystem.Shared.Dtos.MappingDtos.CurrencyDtos
{
    public class CurrencyGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string? Symbol { get; set; }
        public string? Description { get; set; }
        public int PaymentCount { get; set; }
        public int WalletCount { get; set; }
        public int TransactionCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DateTime? SuspendedDate { get; set; }
    }
}
