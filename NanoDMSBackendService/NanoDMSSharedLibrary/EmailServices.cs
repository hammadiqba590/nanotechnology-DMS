using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NanoDMSSharedLibrary
{
    using System.Net;
    using System.Net.Mail;

    public class EmailServices
    {
        private readonly EmailSettings _settings;

        public EmailServices(EmailSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            using var smtpClient = new SmtpClient(_settings.SmtpServer, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.SenderEmail, _settings.AppPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Optionally: log, rethrow, or custom handling
                throw new InvalidOperationException($"Failed to send email to {toEmail}. Reason: {ex.Message}", ex);
            }
        }
    }

}
