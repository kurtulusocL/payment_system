
namespace PaymentSystem.Shared.Dtos.MappingDtos.AuditDtos
{
    public class AuditGetDto
    {
        public int Id { get; set; }
        public string AreaAccessed { get; set; }
        public string? AppUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DateTime? SuspendedDate { get; set; }
    }
}
