
namespace PaymentSystem.Shared.Dtos.MappingDtos.TransactionTypeDto
{
    public class TransactionTypeUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
