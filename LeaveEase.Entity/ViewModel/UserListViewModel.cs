

namespace LeaveEase.Entity.ViewModel
{
    public class UserListViewModel
    {
        public int UserId { get; set; }

        public string? FirstName { get; set; }

        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? MobileNumber { get; set; }

        public string Address { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public DateOnly? BirthDate { get; set; }

        public string? RoleName { get; set; }

        public string CreateBy { get; set; } = string.Empty;

        public string ReportingPerson { get; set; } = string.Empty;


    }
}
