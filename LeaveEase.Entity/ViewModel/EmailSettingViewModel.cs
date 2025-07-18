﻿
namespace LeaveEase.Entity.ViewModel
{
    public class EmailSettingViewModel
    {
        public string SmtpServer { get; set; }=string.Empty;
        public int SmtpPort { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}