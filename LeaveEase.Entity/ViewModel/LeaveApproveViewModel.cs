

using System.ComponentModel.DataAnnotations;
using LeaveEase.Entity.Constants;
using LeaveEase.Entity.Enum;

namespace LeaveEase.Entity.ViewModel
{
    public class LeaveApproveViewModel
    {
        public int LeaveId { get; set; }
        public int EmployeeId { get; set; }

        public int ActualLeave { get; set; }

        public int TotalLeave { get; set; }
        public int ApprovedBy { get; set; }
        public string? EmployeeName { get; set; }


        [Required(ErrorMessage = LeaveMessage.RequiredFromDate)]
        public DateOnly FromDate { get; set; }

        public string? LeaveTypeString { get; set; }

        [Required(ErrorMessage = LeaveMessage.RequiredLeaveType)]
        public LeaveType LeaveType { get; set; }

        [Required(ErrorMessage = LeaveMessage.RequiredToDate)]
        public DateOnly ToDate { get; set; }

        [Required(ErrorMessage = LeaveMessage.RequiredReason)]
        public string? Reason { get; set; }


        public string? Remark { get; set; }

        public DateTime AppliedDate { get; set; }

        public DateTime CreateDate { get; set; }

        [Required(ErrorMessage = LeaveMessage.RequiredStatus)]
        public Status Status { get; set; } 

       
    }
}
