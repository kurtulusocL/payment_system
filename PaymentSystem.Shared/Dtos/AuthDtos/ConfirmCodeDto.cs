
namespace PaymentSystem.Shared.Dtos.AuthDtos
{
    public class ConfirmCodeDto
    {
        public string Email { get; set; } = string.Empty;
        public string ConfirmCode { get; set; } = string.Empty;
    }
}
