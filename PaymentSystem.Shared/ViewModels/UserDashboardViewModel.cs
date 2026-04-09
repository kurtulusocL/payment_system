using PaymentSystem.Shared.Dtos.AuthDtos;

namespace PaymentSystem.Shared.ViewModels
{
    public class UserDashboardViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public List<dynamic> Wallets { get; set; } = new();
        public List<dynamic> Payments { get; set; } = new();
        public UserProfileDto? Profile { get; set; }
        public string? SignalRHubUrl { get; set; }
    }
}
