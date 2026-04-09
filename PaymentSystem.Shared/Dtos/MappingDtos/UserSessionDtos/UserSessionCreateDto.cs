
namespace PaymentSystem.Shared.Dtos.MappingDtos.UserSessionDtos
{
    public class UserSessionCreateDto
    {
        public string Username { get; set; }
        public DateTime LoginDate { get; set; }
        public DateTime? LogoutDate { get; set; }
        public bool IsOnline { get; set; }
        public int? OnlineDurationSeconds { get; set; }
        public string AppUserId { get; set; }
    }
}
