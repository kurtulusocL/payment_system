using PaymentSystem.Domain.Events.Abstract;

namespace PaymentSystem.Domain.Events
{
    public class PaymentFailedEvent:IDomainEvent
    {
        public int PaymentId { get; }
        public string Reason { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public PaymentFailedEvent(int paymentId, string reason)
        {
            PaymentId = paymentId;
            Reason = reason;
        }
    }
}
