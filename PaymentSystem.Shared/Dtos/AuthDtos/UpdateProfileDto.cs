
namespace PaymentSystem.Shared.Dtos.AuthDtos
{
    public class UpdateProfileDto
    {
        public string NameSurname { get; set; } = string.Empty;
        public string? Title { get; set; }
        public DateTime Birthdate { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
