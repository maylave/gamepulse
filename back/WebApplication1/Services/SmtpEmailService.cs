// WebApplication1/Services/SmtpEmailService.cs
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public SmtpEmailService(IOptions<SmtpSettings> smtpOptions)
        {
            _smtpSettings = smtpOptions.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            
            // ✅ ПРАВИЛЬНО: отправитель — из appsettings.json
            email.Sender = MailboxAddress.Parse(_smtpSettings.SenderEmail);
            email.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
            
            // Получатель — тот, кому отправляем
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = $"<p>Ваш код подтверждения: <strong>{body}</strong></p>" +
                           $"<p>Если вы не регистрировались на GamePulse — проигнорируйте это письмо.</p>"
            };

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}