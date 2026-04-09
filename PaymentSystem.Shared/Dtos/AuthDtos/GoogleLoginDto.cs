namespace PaymentSystem.Shared.Dtos.AuthDtos
{
    public class GoogleLoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string ProviderKey { get; set; } = string.Empty;
        public string? NameSurname { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Title { get; set; }
    }
}
