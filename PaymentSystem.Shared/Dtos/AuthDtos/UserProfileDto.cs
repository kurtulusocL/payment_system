
namespace PaymentSystem.Shared.Dtos.AuthDtos
{
    public class UserProfileDto
    {
        public string Id { get; set; } = string.Empty;
        public string NameSurname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime Birthdate { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
