
namespace PaymentSystem.Shared.Dtos.MappingDtos.MerchantStatusDtos
{
    public class MerchantStatusUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
