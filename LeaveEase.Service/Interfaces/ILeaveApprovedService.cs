using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeaveEase.Entity.ViewModel;

namespace LeaveEase.Service.Interfaces
{
    public interface ILeaveApprovedService
    {
       Task<LeaveApproveViewModel> GetLeaveRequest(int LeaveId);

      Task<bool> UpdateLeaveRequest(LeaveApproveViewModel leaveApproveViewModel);
        Task<bool> CancelLeaveRequest(int LeaveId);
        Task<PaginationViewModel<LeaveApproveViewModel>> GetAllLeaveRequestList(string search, int page, int pageSize, string Status, string LeaveDates, string SortbyName, string SortbyFromdate, string SortbyTodate, string SortbyLeavetype);
        
        }
}
