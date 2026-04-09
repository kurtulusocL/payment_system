namespace PaymentSystem.Shared.Dtos.AuthDtos
{
    public class LoginConfirmCodeDto
    {
        public string Email { get; set; } = string.Empty;
        public int LoginConfirmCode { get; set; }
    }
}
