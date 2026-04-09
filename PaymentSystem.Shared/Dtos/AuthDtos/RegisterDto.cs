
namespace PaymentSystem.Shared.Dtos.AuthDtos
{
    public class RegisterDto
    {
        public string NameSurname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Title { get; set; }
        public DateTime Birthdate { get; set; }
    }
}
