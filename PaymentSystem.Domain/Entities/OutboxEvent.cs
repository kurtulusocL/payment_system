using PaymentSystem.Domain.Entities.Base;

namespace PaymentSystem.Domain.Entities
{
    public class OutboxEvent : BaseEntity
    {
        public string EntityType { get; set; }
        public string EventType { get; set; }    // Added / Modified / Deleted
        public string Payload { get; set; }      // JSON
        public bool IsProcessed { get; set; } = false;
        public DateTime? ProcessedDate { get; set; }
    }
}
