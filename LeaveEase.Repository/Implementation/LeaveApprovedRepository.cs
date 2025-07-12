
using LeaveEase.Entity.Models;
using LeaveEase.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeaveEase.Repository.Implementation
{
    public class LeaveApprovedRepository: ILeaveApprovedRepository
    {
        private readonly LeaveEaseDbContext _context;

        public LeaveApprovedRepository(LeaveEaseDbContext context)
        {
            _context = context;
        }



        /// <summary>
        /// Retrieves a list of leave requests for employees reporting to the specified user.
        /// </summary>
        /// <remarks>This method queries the database to retrieve leave requests and includes related
        /// leave approval data. Ensure that the <paramref name="LoginUser"/> parameter is valid and corresponds to a
        /// reporting person.</remarks>
        /// <param name="LoginUser">The identifier of the user for whom the leave requests are being retrieved. This should correspond to the
        /// reporting person of the employees.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of  <see
        /// cref="TblLeaveRequest"/> objects associated with employees reporting to the specified user. If an error
        /// occurs, an empty list is returned.</returns>
        public async Task<List<TblLeaveRequest>> GetAllLeaveRequestList(int? UserId)
        {
            try
            {
                if (UserId == null)
                {
                    return await _context.TblLeaveRequests.Include(c => c.TblLeaveApproves).ToListAsync();
                }

                return await _context.TblLeaveRequests.Include(c=>c.TblLeaveApproves).Where(c=>c.Employee.ReportingPerson ==UserId).ToListAsync();
            }
            catch (Exception)
            {
                return new List<TblLeaveRequest>();
            }
        }
    

        public async Task<string> GetLeaveApprove(int LeaveId)
        {
            try
            {
                TblLeaveApprove? tblLeaveApprove = await _context.TblLeaveApproves.FirstOrDefaultAsync(c => c.LeaveRequestId == LeaveId);
                string LeaveRemark= null!;
                if (tblLeaveApprove != null)
                {
                    LeaveRemark = tblLeaveApprove.Remark ?? string.Empty;
                }
                return LeaveRemark!;
            }
            catch
            {
                return null!;
            }
        }


        /// <summary>
        /// Adds a leave approval record to the database asynchronously.
        /// </summary>
        /// <remarks>This method attempts to save the provided leave approval entity to the database. If a
        /// database update  error occurs, the method logs the exception details and returns <see
        /// langword="false"/>.</remarks>
        /// <param name="tblLeaveApprove">The leave approval entity to be added. This parameter must not be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the record is successfully added; otherwise, <see langword="false"/>.</returns>
        public async Task<bool> AddLeaveApprove(TblLeaveApprove tblLeaveApprove)
        {
            try
            {
                await _context.AddAsync(tblLeaveApprove);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return false;
            }
        }

    }
}
