
namespace PaymentSystem.Domain.ValueObject
{
    public class CardNumber
    {
        public string MaskedNumber { get; private set; }
        public string LastFourDigits { get; private set; }

        private CardNumber() { }

        public CardNumber(string rawNumber)
        {
            if (string.IsNullOrWhiteSpace(rawNumber))
                throw new ArgumentException("Card number cannot be empty.", nameof(rawNumber));

            var digits = rawNumber.Replace(" ", "").Replace("-", "");

            if (digits.Length < 13 || digits.Length > 19)
                throw new ArgumentException("Card number must be between 13 and 19 digits.", nameof(rawNumber));

            if (!digits.All(char.IsDigit))
                throw new ArgumentException("Card number must contain only digits.", nameof(rawNumber));

            LastFourDigits = digits[^4..];
            MaskedNumber = $"****-****-****-{LastFourDigits}";
        }
    }
}
