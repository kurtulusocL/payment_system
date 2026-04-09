using System.Text.Json.Serialization;
using PaymentSystem.Domain.Entities.Base;

namespace PaymentSystem.Domain.Entities
{
    public class UserSession : BaseEntity
    {
        public string Username { get; set; }
        public DateTime LoginDate { get; set; }
        public DateTime? LogoutDate { get; set; }
        public bool IsOnline { get; set; }
        public int? OnlineDurationSeconds { get; set; }

        public string UserId { get; set; }

        [JsonIgnore]
        public virtual AppUser User { get; set; } = null!;
    }
}
