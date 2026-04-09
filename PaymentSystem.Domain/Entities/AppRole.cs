using Microsoft.AspNetCore.Identity;
using PaymentSystem.Domain.Entities.Base;

namespace PaymentSystem.Domain.Entities
{
    public class AppRole : IdentityRole, IEntity
    {
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DateTime? SuspendedDate { get; set; }
    }
}
