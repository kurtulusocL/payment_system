using PaymentSystem.Domain.Events.Abstract;

namespace PaymentSystem.Domain.Events
{
    public class PaymentCompletedEvent:IDomainEvent
    {
        public int PaymentId { get; }
        public string UserId { get; }
        public decimal Amount { get; }
        public int CurrencyId { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public PaymentCompletedEvent(int paymentId, string userId, decimal amount, int currencyId)
        {
            PaymentId = paymentId;
            UserId = userId;
            Amount = amount;
            CurrencyId = currencyId;
        }
    }
}
