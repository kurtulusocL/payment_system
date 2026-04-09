
namespace PaymentSystem.Shared.Dtos.EntityDtos.MerchantDtos
{
    public class MerchantResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Guid ApiKey { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
