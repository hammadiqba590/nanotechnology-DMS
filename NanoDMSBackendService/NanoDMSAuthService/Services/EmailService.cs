using System.Net.Mail;
using System.Net;

namespace NanoDMSAuthService.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            // Ensure values are not null before parsing or using them
            var smtpServer = emailSettings["SmtpServer"] ?? throw new ArgumentNullException(nameof(emailSettings), "SmtpServer is not configured.");
            var portString = emailSettings["Port"] ?? throw new ArgumentNullException(nameof(emailSettings), "Port is not configured.");
            var senderEmail = emailSettings["SenderEmail"] ?? throw new ArgumentNullException(nameof(emailSettings), "SenderEmail is not configured.");
            var appPassword = emailSettings["AppPassword"] ?? throw new ArgumentNullException(nameof(emailSettings), "AppPassword is not configured.");
            var senderName = emailSettings["SenderName"] ?? throw new ArgumentNullException(nameof(emailSettings), "SenderName is not configured.");

            using var client = new SmtpClient(smtpServer, int.Parse(portString))
            {
                Credentials = new NetworkCredential(senderEmail, appPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }
    }

}
