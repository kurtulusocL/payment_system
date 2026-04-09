using Microsoft.AspNetCore.Authentication;

namespace PaymentSystem.Infrastructure.Constants.Options
{
    public class ApiKeyAuthenticationOptions: AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "ApiKey";
        public const string HeaderName = "X-Api-Key";
    }
}
