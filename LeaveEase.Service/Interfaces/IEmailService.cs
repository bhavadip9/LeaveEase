

namespace LeaveEase.Service.Interfaces
{
    public interface IEmailService
    {
        public void SendEmail(string toEmail, string subject, string htmlBody);

 
    }
}
