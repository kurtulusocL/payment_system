using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentSystem.Infrastructure.Constants.Options;

namespace PaymentSystem.Infrastructure.Constants.Handlers
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private readonly string _validApiKey;

        public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, IConfiguration configuration) : base(options, logger, encoder)
        {
            _validApiKey = configuration["ApiKey"]
                ?? throw new InvalidOperationException("ApiKey configuration is missing!");
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(ApiKeyAuthenticationOptions.HeaderName, out var apiKeyHeaderValues))
                return Task.FromResult(AuthenticateResult.NoResult());

            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(providedApiKey) || providedApiKey != _validApiKey)
                return Task.FromResult(AuthenticateResult.Fail("Invalid or missing API Key"));

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "ApiClient"),
                new Claim("ApiKeyUser", "true")
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
