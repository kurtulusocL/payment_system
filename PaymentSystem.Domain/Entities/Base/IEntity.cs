
namespace PaymentSystem.Domain.Entities.Base
{
    public interface IEntity
    {
        bool IsActive { get; set; }
        bool IsDeleted { get; set; }
        DateTime CreatedDate { get; set; }
        DateTime? UpdatedDate { get; set; }
        DateTime? DeletedDate { get; set; }
        public DateTime? SuspendedDate { get; set; }
    }
}
