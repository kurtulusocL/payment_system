
namespace PaymentSystem.Domain.ValueObject
{
    public class Money
    {
        public decimal Amount { get; private set; }
        public int CurrencyId { get; private set; }

        private Money() { }

        public Money(decimal amount, int currencyId)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative.", nameof(amount));

            Amount = Math.Round(amount, 2);
            CurrencyId = currencyId;
        }

        public Money Add(Money other)
        {
            EnsureSameCurrency(other);
            return new Money(Amount + other.Amount, CurrencyId);
        }

        public Money Subtract(Money other)
        {
            EnsureSameCurrency(other);
            return new Money(Amount - other.Amount, CurrencyId);
        }

        public bool IsGreaterThan(Money other)
        {
            EnsureSameCurrency(other);
            return Amount > other.Amount;
        }

        private void EnsureSameCurrency(Money other)
        {
            if (CurrencyId != other.CurrencyId)
                throw new InvalidOperationException("Cannot operate on different currencies.");
        }
    }
}
