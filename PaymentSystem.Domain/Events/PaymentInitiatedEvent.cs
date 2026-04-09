using PaymentSystem.Domain.Events.Abstract;

namespace PaymentSystem.Domain.Events
{
    public class PaymentInitiatedEvent:IDomainEvent
    {
        public int PaymentId { get; }
        public string IdempotencyKey { get; }
        public decimal Amount { get; }
        public int CurrencyId { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public PaymentInitiatedEvent(int paymentId, string idempotencyKey, decimal amount, int currencyId)
        {
            PaymentId = paymentId;
            IdempotencyKey = idempotencyKey;
            Amount = amount;
            CurrencyId = currencyId;
        }
    }
}
