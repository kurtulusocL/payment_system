using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using PaymentSystem.Application.Constants.Services.Abstract;
using PaymentSystem.Shared.Settings;

namespace PaymentSystem.Application.Constants.Services.Concrete
{
    public class MailService:IMailService
    {
        private readonly MailSettings _mailSettings;

        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendConfirmCodeAsync(string toEmail, string confirmCode)
        {
            var subject = "PaymentSystem — Email Confirmation";
            var body = $@"
            <h2>Welcome to PaymentSystem</h2>
            <p>Your confirmation code is:</p>
            <h1 style='color: #2e86c1; letter-spacing: 4px;'>{confirmCode}</h1>
            <p>This code will expire in 10 minutes.</p>";

            await SendAsync(toEmail, subject, body);
        }

        public async Task SendLoginConfirmCodeAsync(string toEmail, string confirmCode)
        {
            var subject = "PaymentSystem — Login Verification";
            var body = $@"
            <h2>Login Verification</h2>
            <p>Your login verification code is:</p>
            <h1 style='color: #2e86c1; letter-spacing: 4px;'>{confirmCode}</h1>
            <p>If you did not attempt to login, please secure your account immediately.</p>
            <p>This code will expire in 10 minutes.</p>";

            await SendAsync(toEmail, subject, body);
        }

        private async Task SendAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.From));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_mailSettings.From, _mailSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
