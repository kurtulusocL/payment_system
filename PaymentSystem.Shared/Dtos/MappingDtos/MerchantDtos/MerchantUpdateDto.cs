
namespace PaymentSystem.Shared.Dtos.MappingDtos.MerchantDtos
{
    public class MerchantUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? TaxNumber { get; set; }
        public Guid ApiKey { get; set; } = Guid.NewGuid();
        public int MerchantStatusId { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
