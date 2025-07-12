using System.ComponentModel.DataAnnotations;
using LeaveEase.Entity.Constants;
using LeaveEase.Entity.Enum;
using Microsoft.AspNetCore.Http;

namespace LeaveEase.Entity.ViewModel
{
    public class UserRegisterViewModel
    {
        public int? UserId { get; set; }

        [Required(ErrorMessage =MessagesValidation.RequiredFirstName)]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage =MessagesValidation.RequiredLastName)]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = MessagesValidation.RequiredPassword)]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = MessagesValidation.PasswordLength)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[a-zA-Z]).{8,}$",
    ErrorMessage = MessagesValidation.PasswordPattern)]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage =MessagesValidation.RequiredConfirmPassword)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = MessagesValidation.PasswordMismatch)]
        public string ConfirmPassword { get; set; } = null!;


        [Required(ErrorMessage =MessagesValidation.RequiredEmail)]
        [EmailAddress]
        public string Email { get; set; } = null!;



        [Required(ErrorMessage = MessagesValidation.RequiredAddress)]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage =MessagesValidation.RequiredDepartment)]
        public Department Department { get; set; }
        [Required(ErrorMessage =MessagesValidation.RequiredRole)]
        public int Role { get; set; }

        [Required(ErrorMessage =MessagesValidation.RequiredBirthDate)]
        public DateOnly? BirthDate { get; set; }

        [Required(ErrorMessage = MessagesValidation.RequiredMobileNumber)]
        [Phone(ErrorMessage = MessagesValidation.InvalidPhoneNumber)]
        [RegularExpression(@"^\d{10}$", ErrorMessage = MessagesValidation.MobileNumberLength)]
        public string? MobileNumber { get; set; }

        public IFormFile? ProfilePicture { get; set; }

        public string? ProfileImage { get; set; }

        public bool IsActive { get; set; }

        public string? CreateByName { get; set; }
        [Required(ErrorMessage = MessagesValidation.RequiredReportingPerson)]
        public int ReportingPerson { get; set; }
        public List<RoleViewModel> RoleVM { get; set; } = new List<RoleViewModel>();

        public List<UserRegisterViewModel>? AdminList { get; set; }

        public string ResetLink { get; set; } = string.Empty;
    }
}



