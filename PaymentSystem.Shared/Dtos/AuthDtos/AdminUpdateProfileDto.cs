namespace PaymentSystem.Shared.Dtos.AuthDtos
{
    public class AdminUpdateProfileDto
    {
        public string TargetUserId { get; set; } = string.Empty;
        public string NameSurname { get; set; } = string.Empty;
        public string? Title { get; set; }
        public DateTime Birthdate { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
