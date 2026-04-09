
namespace PaymentSystem.Shared.Dtos.MappingDtos.MerchantDtos
{
    public class MerchantGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? TaxNumber { get; set; }
        public Guid ApiKey { get; set; } = Guid.NewGuid();
        public int MerchantStatusId { get; set; }
        public int PaymentCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DateTime? SuspendedDate { get; set; }
    }
}
