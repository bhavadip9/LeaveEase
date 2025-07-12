using System.Net;
using System.Net.Mail;
using LeaveEase.Entity.ViewModel;
using LeaveEase.Service.Interfaces;
using Microsoft.Extensions.Options;



namespace LeaveEase.Service.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettingViewModel _emailSettings;

        public EmailService(IOptions<EmailSettingViewModel> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }


        /// <summary>
        /// Sends an email with the specified subject and HTML body to the specified recipient.
        /// </summary>
        /// <remarks>This method uses the SMTP protocol to send the email. Ensure that the email settings,
        /// such as the SMTP server, port, credentials, and sender information, are correctly configured before calling
        /// this method.</remarks>
        /// <param name="toEmail">The email address of the recipient. Cannot be null or empty.</param>
        /// <param name="subject">The subject of the email. Cannot be null or empty.</param>
        /// <param name="htmlBody">The HTML content of the email body. Cannot be null or empty.</param>
        public void SendEmail(string toEmail, string subject, string htmlBody)
        {
            using var smtpClient = new SmtpClient(_emailSettings.SmtpServer)
            {
                Port = _emailSettings.SmtpPort,
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            smtpClient.Send(mailMessage);
        }
    }
}

