
using System.ComponentModel.DataAnnotations;
using LeaveEase.Entity.Constants;


namespace LeaveEase.Entity.ViewModel
{
    public class ResetPassWordViewModel
    {
        public string? Email { get; set; }

        [Required(ErrorMessage = MessagesValidation.RequiredNewPassword)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = MessagesValidation.RequiredConfirmPassword)]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = MessagesValidation.PasswordMismatch)]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? Token { get; set; } 

        public string? UrlLink { get; set; }
    }
}

