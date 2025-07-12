

using System.ComponentModel.DataAnnotations;
using LeaveEase.Entity.Constants;
using LeaveEase.Entity.Enum;

namespace LeaveEase.Entity.ViewModel
{
    public class LeaveRequestViewModel
    {

        public int LeaveId { get; set; }
        public int EmployeeId { get; set; }

        public string? Role { get; set; }

        [Required(ErrorMessage = LeaveMessage.RequiredFromDate)]
        public DateOnly FromDate { get; set; }

        [Required(ErrorMessage = LeaveMessage.RequiredToDate)]
        public DateOnly ToDate { get; set; }

        public string? LeaveTypeString { get; set; }

        [Required(ErrorMessage = LeaveMessage.RequiredLeaveType)]
        public LeaveType LeaveType { get; set; } 
       
        public int? TotalLeave { get; set; }
        public int? ActualLeave { get; set; }
       

        [Required(ErrorMessage = LeaveMessage.RequiredReason)]
        public string Reason { get; set; } = string.Empty;

        public DateTime AppliedDate { get; set; }

        public DateTime CreateDate { get; set; }

        [Required(ErrorMessage = LeaveMessage.RequiredStatus)]
        public string Status { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
