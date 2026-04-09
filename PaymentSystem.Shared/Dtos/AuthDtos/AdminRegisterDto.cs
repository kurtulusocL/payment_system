namespace PaymentSystem.Shared.Dtos.AuthDtos
{
    public class AdminRegisterDto
    {
        public string NameSurname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Title { get; set; }
        public DateTime Birthdate { get; set; }
        public List<string> SelectedRoles { get; set; } = new();
    }
}
