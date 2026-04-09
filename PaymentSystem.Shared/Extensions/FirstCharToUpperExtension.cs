
namespace PaymentSystem.Shared.Extensions
{
    public static class FirstCharToUpperExtension
    {
        public static string FirstCharToUpper(this string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return char.ToUpperInvariant(input[0]) + input.Substring(1);
        }
    }
}
