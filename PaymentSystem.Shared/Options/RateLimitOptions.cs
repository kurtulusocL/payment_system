
namespace PaymentSystem.Shared.Options
{
    public class RateLimitOptions
    {
        public PolicySettings Web { get; set; } = new();
        public PolicySettings SignalR { get; set; } = new();
    }
}
