
namespace PaymentSystem.Application.Constants.Services.Abstract
{
    public interface IMailService
    {
        Task SendConfirmCodeAsync(string toEmail, string confirmCode);
        Task SendLoginConfirmCodeAsync(string toEmail, string confirmCode);
    }
}
