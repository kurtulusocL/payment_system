using System.ComponentModel.DataAnnotations;

namespace PaymentSystem.Domain.Entities.Base
{
    public class BaseEntity : IEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DateTime? SuspendedDate { get; set; }
    }
}
