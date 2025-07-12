
//using System;
//using System.Collections.Generic;

//namespace LeaveEase.Entity.ViewModel
//{
//    public class DashboardViewModel
//    {
//        public string RoleName { get; set; } = string.Empty;

//        public int MonthlyLeaveCount { get; set; } = 0;
//        public List<string> MonthlyLeaveList { get; set; } = new List<string>();

//        public int TotalLeaveCount { get; set; } = 0;
//        public int TotalEmployees { get; set; } = 0;
//        public int TotalActiveEmployees { get; set; } = 0;
//        public int TotalDepartments { get; set; } = 0;

//        public int TotalLeaveRequests { get; set; } = 0;
//        public int TotalLeaveApproved { get; set; } = 0;
//        public int TotalLeaveRejected { get; set; } = 0;
//        public int TotalLeavePending { get; set; } = 0;
//        public int TotalLeaveCancelled { get; set; } = 0;
//        public int ActiveLeave { get; set; } = 0;

//        public List<UpComingLeave> UpComingLeaves { get; set; } = new List<UpComingLeave>();
//    }

//    public class UpComingLeave
//    {
//        public string EmployeeName { get; set; } = string.Empty;
//        public string LeaveDates { get; set; } = string.Empty;
//    }
//}
namespace LeaveEase.Entity.ViewModel
{
    public class DashboardViewModel
    {
        public string RoleName { get; set; }

        public int TotalEmployees { get; set; }
        public int TotalActiveEmployees { get; set; }

        // New fields
        public int MonthlyLeavesCount { get; set; }
        public int TotalLeaveDays { get; set; }
        public List<DateTime> MonthlyLeaveDates { get; set; }

        public int PendingLeaveRequestsCount { get; set; }

        public List<UpcomingLeaveViewModel> UpcomingLeaves { get; set; }

        public int LeavesTakenThisMonth { get; set; }
        public int DepartmentsCount { get; set; }
    }
}
public class UpcomingLeaveViewModel
{
    public string EmployeeName { get; set; }
    public string LeaveDateRange { get; set; }
}
