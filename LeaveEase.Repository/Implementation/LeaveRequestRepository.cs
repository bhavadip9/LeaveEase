
using LeaveEase.Entity.Models;
using LeaveEase.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeaveEase.Repository.Implementation
{
    public class LeaveRequestRepository: ILeaveRequestRepository
    {
        private readonly LeaveEaseDbContext _context;

        public LeaveRequestRepository(LeaveEaseDbContext context)
        {
            _context = context;
        }


        public async Task<TblLeaveRequest> AddEditLeaveRequest(TblLeaveRequest tblLeaveRequest)
        {
            try
            {
                if (tblLeaveRequest.LeaveId == 0)
                {
                    await _context.AddAsync(tblLeaveRequest);
                    await _context.SaveChangesAsync();
                    return tblLeaveRequest;
                }
                else
                {
                     _context.Update(tblLeaveRequest);
                    await _context.SaveChangesAsync();
                    return tblLeaveRequest;
                }
           
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return new TblLeaveRequest();
            }
        }


        /// <summary>
        /// Retrieves a list of leave requests associated with the specified user.
        /// </summary>
        /// <remarks>This method queries the database for leave requests where the <c>EmployeeId</c>
        /// matches the provided user ID. If the operation fails due to an exception, an empty list is returned instead
        /// of propagating the error.</remarks>
        /// <param name="LoginUser">The unique identifier of the user whose leave requests are to be retrieved.</param>
        /// <returns>A list of <see cref="TblLeaveRequest"/> objects representing the leave requests for the specified user. If
        /// an error occurs, an empty list is returned.</returns>
        public async Task<List<TblLeaveRequest>> GetLeaveRequestList(int UserId)
        {
            try
            {
                return await _context.TblLeaveRequests.Where(c=>c.EmployeeId== UserId && c.IsActive).ToListAsync();
            }
            catch (Exception)
            {
                return new List<TblLeaveRequest>();
            }
        }

        /// <summary>
        /// Retrieves a leave request by its unique identifier.
        /// </summary>
        /// <remarks>This method attempts to retrieve the leave request from the database asynchronously.
        /// If an error occurs during the operation, a default <see cref="TblLeaveRequest"/> instance is
        /// returned.</remarks>
        /// <param name="LeaveId">The unique identifier of the leave request to retrieve. Must be a positive integer.</param>
        /// <returns>The <see cref="TblLeaveRequest"/> object corresponding to the specified <paramref name="LeaveId"/>. If no
        /// matching leave request is found, returns a new <see cref="TblLeaveRequest"/> instance.</returns>
        public async Task<TblLeaveRequest> GetLeaveRequest(int LeaveId)
        {
            try
            {
                TblLeaveRequest? tblLeaveRequest = await _context.TblLeaveRequests.FirstOrDefaultAsync(c => c.LeaveId == LeaveId);
                return tblLeaveRequest!;
            }
            catch
            {
                return new TblLeaveRequest();
            } 
        }
        
    
    }
}
