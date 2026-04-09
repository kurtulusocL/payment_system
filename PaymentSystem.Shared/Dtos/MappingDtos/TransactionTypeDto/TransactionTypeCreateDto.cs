
namespace PaymentSystem.Shared.Dtos.MappingDtos.TransactionTypeDto
{
    public class TransactionTypeCreateDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
