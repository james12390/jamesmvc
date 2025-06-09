using jamesmvc.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
namespace jamesmvc.Services
{
   

    public class MailtrapEmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public MailtrapEmailSender(IOptions<EmailSettings> options)
        {
            _emailSettings = options.Value;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            mail.To.Add(email);

            using var smtp = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
            {
                Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SenderPassword),
                EnableSsl = true,
                Timeout = 20000
            };

            return smtp.SendMailAsync(mail);
        }
    }
}
