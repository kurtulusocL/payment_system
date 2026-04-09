using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using PaymentSystem.Domain.Entities.Base;

namespace PaymentSystem.Domain.Entities
{
    public class AppUser : IdentityUser, IEntity
    {
        public string NameSurname { get; set; }
        public string? Title { get; set; }
        public DateTime Birthdate { get; set; }
        public int? ConfirmCode { get; set; }
        public bool IsConfirmCodeActive { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DateTime? SuspendedDate { get; set; }

        public virtual Wallet? Wallet { get; set; }

        [JsonIgnore]
        public virtual ICollection<Audit> Audits { get; set; }

        [JsonIgnore]
        public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();

        [JsonIgnore]
        public virtual ICollection<UserSession> UserSessions { get; set; } = new HashSet<UserSession>();
        public AppUser()
        {
            EmailConfirmed = true;
            PhoneNumberConfirmed = true;
        }
    }
}
