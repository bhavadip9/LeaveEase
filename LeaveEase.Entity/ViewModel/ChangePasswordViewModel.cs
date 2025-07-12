
using System.ComponentModel.DataAnnotations;
using LeaveEase.Entity.Constants;


namespace LeaveEase.Entity.ViewModel
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = MessagesValidation.RequiredOldPassword)]
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = MessagesValidation.RequiredNewPassword)]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = MessagesValidation.PasswordLength)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[a-zA-Z]).{8,}$",
         ErrorMessage = MessagesValidation.PasswordPattern)]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = MessagesValidation.RequiredConfirmPassword)]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = MessagesValidation.PasswordMismatch)]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }
}