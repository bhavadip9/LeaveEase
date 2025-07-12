using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeaveEase.Entity.ViewModel;

namespace LeaveEase.Service.Interfaces
{
    public interface ILeaveRequestService
    {
        Task<bool> AddLeaveRequest(LeaveRequestViewModel leaveRequestViewModel);

       Task<PaginationViewModel<LeaveRequestViewModel>> GetLeaveRequestList( int page, int pageSize, string Status, string LeaveDates, string SortbyFromdate, string SortbyTodate, string SortbyLeavetype,string AppliedDate);

       Task<LeaveRequestViewModel> GetLeaveRequest(int LeaveId);

         Task<bool> EditLeaveRequest(LeaveRequestViewModel leaveRequestViewModel);

        Task<bool> DeleteLeaveRequest(int LeaveId);

        // Task<bool> AnyLeaveExist(int Employeeid, DateOnly FormDate, DateOnly ToDate);
        Task<bool> AnyLeaveExist(int employeeId, DateOnly fromDate, DateOnly toDate, int? excludeLeaveId = null);
    }
}
