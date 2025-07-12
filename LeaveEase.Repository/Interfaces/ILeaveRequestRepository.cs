

using LeaveEase.Entity.Models;

namespace LeaveEase.Repository.Interfaces
{
    public interface ILeaveRequestRepository
    {

       Task<TblLeaveRequest> AddEditLeaveRequest(TblLeaveRequest tblLeaveRequest);
       Task<List<TblLeaveRequest>> GetLeaveRequestList(int LoginUser);
       Task<TblLeaveRequest> GetLeaveRequest(int LeaveId);
    }
}
