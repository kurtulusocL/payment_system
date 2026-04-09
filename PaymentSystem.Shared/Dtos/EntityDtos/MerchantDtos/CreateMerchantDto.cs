
namespace PaymentSystem.Shared.Dtos.EntityDtos.MerchantDtos
{
    public class CreateMerchantDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? TaxNumber { get; set; }
    }
}
