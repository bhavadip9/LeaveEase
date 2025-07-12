
using LeaveEase.Entity.Models;

namespace LeaveEase.Repository.Interfaces
{
    public interface ILeaveApprovedRepository
    {

        Task<List<TblLeaveRequest>> GetAllLeaveRequestList(int? UserId);
       Task<string> GetLeaveApprove(int LeaveId);
        Task<bool> AddLeaveApprove(TblLeaveApprove tblLeaveApprove);
       
    }
}
