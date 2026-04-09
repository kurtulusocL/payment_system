namespace PaymentSystem.Shared.Dtos.AuthDtos
{
    public class AdminChangePasswordDto
    {
        public string TargetUserId { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
