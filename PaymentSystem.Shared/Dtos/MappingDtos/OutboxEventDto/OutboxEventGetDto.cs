
namespace PaymentSystem.Shared.Dtos.MappingDtos.OutboxEventDto
{
    public class OutboxEventGetDto
    {
        public int Id { get; set; }
        public string EntityType { get; set; }
        public string EventType { get; set; }
        public string Payload { get; set; }
        public bool IsProcessed { get; set; } = false;
        public DateTime ProcessedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DateTime? SuspendedDate { get; set; }
    }
}
