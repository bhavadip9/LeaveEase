
using AutoMapper;
using LeaveEase.Entity.Models;
using LeaveEase.Entity.ViewModel;
using LeaveEase.Repository.Interfaces;
using LeaveEase.Entity.Constants;
using LeaveEase.Service.Helper;
using LeaveEase.Service.Interfaces;
using Microsoft.AspNetCore.Http;


namespace LeaveEase.Service.Implementation
{
    public class LeaveRequestService: ILeaveRequestService
    {

        private readonly ILeaveRequestRepository _leaveRequestRepository;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IHomeRepository _homeRepository;

        private readonly IMapper _mapper;

        public LeaveRequestService(ILeaveRequestRepository leaveRequestRepository, IMapper mapper, IHomeRepository homeRepository,IHttpContextAccessor httpContextAccessor)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _homeRepository = homeRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> AddLeaveRequest(LeaveRequestViewModel leaveRequestViewModel)
        {
            try
            {
 
                TblLeaveRequest tblLeaveRequest = _mapper.Map<TblLeaveRequest>(leaveRequestViewModel);
                tblLeaveRequest.CreateDate = DateTime.UtcNow;
                tblLeaveRequest.AppliedDate = DateTime.UtcNow;

                UserInformationViewModel userInformationViewModel = UserInfoHelper.GetUserInfo(_httpContextAccessor.HttpContext);
                tblLeaveRequest.CreateByName = userInformationViewModel.Name;
                tblLeaveRequest.CreateById = userInformationViewModel.UserId;
                tblLeaveRequest.EmployeeId = userInformationViewModel.UserId;

                tblLeaveRequest.IsActive = true;

                await _leaveRequestRepository.AddEditLeaveRequest(tblLeaveRequest);

                return true;
            }
            catch
            {
                return false;
            }
         
        }

       

        public async Task<bool> AnyLeaveExist(int employeeId, DateOnly fromDate, DateOnly toDate, int? excludeLeaveId = null)
        {
            try
            {
                var leaveRequests = await _leaveRequestRepository.GetLeaveRequestList(employeeId);

                var isExist = leaveRequests.Any(leave =>
                    leave.Status != StatusConstant.Cancelled &&
                    leave.IsActive &&
                    leave.FromDate <= toDate &&
                    leave.ToDate >= fromDate &&
                    (!excludeLeaveId.HasValue || leave.LeaveId != excludeLeaveId.Value)
                );

                return isExist;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n{ex.InnerException?.Message}");
                return false;
            }
        }
        public async Task<bool> EditLeaveRequest(LeaveRequestViewModel leaveRequestViewModel)
        {
            try
            {
                TblLeaveRequest tblLeaveRequest = await _leaveRequestRepository.GetLeaveRequest(leaveRequestViewModel.LeaveId);
                UserInformationViewModel userInformationViewModel = UserInfoHelper.GetUserInfo(_httpContextAccessor.HttpContext);
               
                tblLeaveRequest.FromDate = leaveRequestViewModel.FromDate;
                tblLeaveRequest.ToDate = leaveRequestViewModel.ToDate;
                tblLeaveRequest.Reason = leaveRequestViewModel.Reason;
                tblLeaveRequest.LeaveType = leaveRequestViewModel.LeaveType.ToString();
                tblLeaveRequest.UpdateDate = DateTime.UtcNow;
                tblLeaveRequest.UpdateByName = userInformationViewModel.Name;
                tblLeaveRequest.UpdateById = userInformationViewModel.UserId;          
                tblLeaveRequest.Status = leaveRequestViewModel.Status;
                tblLeaveRequest.EmployeeId = userInformationViewModel.UserId;
                tblLeaveRequest.IsActive = true;


                await _leaveRequestRepository.AddEditLeaveRequest(tblLeaveRequest);

                return true;
            }
            catch
            {
                return false;
            }
         
        }

        public async Task<PaginationViewModel<LeaveRequestViewModel>> GetLeaveRequestList(int page, int pageSize, string Status, string LeaveDates, string SortbyFromdate, string SortbyTodate, string SortbyLeavetype, string AppliedDate)
        {
            try
            {
                UserInformationViewModel userInformationViewModel = UserInfoHelper.GetUserInfo(_httpContextAccessor.HttpContext);
                string UserRole = userInformationViewModel.Role!;

                int Userid = userInformationViewModel.UserId;
                List<TblLeaveRequest> userList = await _leaveRequestRepository.GetLeaveRequestList(Userid);

                var tableCount = 0;

                List<LeaveRequestViewModel> LeaveRequestViewModel = userList.Select(c => new LeaveRequestViewModel
                {
                    LeaveId = c.LeaveId,
                    FromDate = c.FromDate,
                    ToDate = c.ToDate,
                    Reason = c.Reason!,
                    AppliedDate = c.AppliedDate,
                    LeaveTypeString = DisplayNameHelper.GetDisplayName(Enum.Parse<Entity.Enum.LeaveType>(c.LeaveType)),
                    Status = c.Status,
                }).ToList();

                LeaveRequestViewModel = LeaveRequestViewModel.OrderByDescending(u => u.FromDate).ToList();
               
                if (!string.IsNullOrEmpty(Status))
                {
                    LeaveRequestViewModel = LeaveRequestViewModel.Where(u => u.Status.ToString()!.Contains(Status, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                switch (AppliedDate)
                {
                    case OrderByConstant.asc_by:
                        LeaveRequestViewModel = LeaveRequestViewModel.OrderBy(u => u.AppliedDate).ToList();
                        break;
                    case OrderByConstant.dec_by:
                        LeaveRequestViewModel = LeaveRequestViewModel.OrderByDescending(u => u.AppliedDate).ToList();
                        break;
                }

                switch (SortbyFromdate)
                {
                    case OrderByConstant.asc_by:
                        LeaveRequestViewModel = LeaveRequestViewModel.OrderBy(u => u.FromDate).ToList();
                        break;
                    case OrderByConstant.dec_by:
                        LeaveRequestViewModel = LeaveRequestViewModel.OrderByDescending(u => u.FromDate).ToList();
                        break;
                }

                switch (SortbyTodate)
                {
                    case OrderByConstant.asc_by:
                        LeaveRequestViewModel = LeaveRequestViewModel.OrderBy(u => u.ToDate).ToList();
                        break;
                    case OrderByConstant.dec_by:
                        LeaveRequestViewModel = LeaveRequestViewModel.OrderByDescending(u => u.ToDate).ToList();
                        break;
                }

                switch (SortbyLeavetype)
                {
                    case OrderByConstant.asc_by:
                        LeaveRequestViewModel = LeaveRequestViewModel.OrderBy(u => u.LeaveTypeString).ToList();
                        break;
                    case OrderByConstant.dec_by:
                        LeaveRequestViewModel = LeaveRequestViewModel.OrderByDescending(u => u.LeaveTypeString).ToList();
                        break;
                }


              

                if (!string.IsNullOrEmpty(LeaveDates) && LeaveDates != "all")
                {
                    DateTime now = DateTime.Now;

                    if (int.TryParse(LeaveDates, out int selectedMonth) && selectedMonth >= 1 && selectedMonth <= 12)
                    {
                        var startOfSelectedMonth = new DateTime(now.Year, selectedMonth, 1);
                        var endOfSelectedMonth = startOfSelectedMonth.AddMonths(1).AddDays(-1);

                        LeaveRequestViewModel = LeaveRequestViewModel
                            .Where(o => o.FromDate.ToDateTime(TimeOnly.MinValue) >= startOfSelectedMonth && o.FromDate.ToDateTime(TimeOnly.MinValue) <= endOfSelectedMonth)
                            .ToList();
                    }
                }

                tableCount = LeaveRequestViewModel.Count;

                LeaveRequestViewModel = LeaveRequestViewModel.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                PaginationViewModel<LeaveRequestViewModel> LeaveRequest = new PaginationViewModel<LeaveRequestViewModel>
                {
                    LeaveRequests = LeaveRequestViewModel,
                    TotalCount = tableCount,
                    PageIndex = page,
                    PageSize = pageSize,
                };

                return LeaveRequest;
            }
            catch
            {
                return  new PaginationViewModel<LeaveRequestViewModel>
                {
                    LeaveRequests = null,
                    TotalCount = 0,
                    PageIndex = page,
                    PageSize = pageSize,
                };
            }
            
        }

        public async Task<LeaveRequestViewModel> GetLeaveRequest(int LeaveId)
        {
            try
            {
                TblLeaveRequest tblLeaveRequest = await _leaveRequestRepository.GetLeaveRequest(LeaveId);
                LeaveRequestViewModel leaveRequestViewModel = _mapper.Map<LeaveRequestViewModel>(tblLeaveRequest);

                return leaveRequestViewModel;
            }
            catch
            {
                LeaveRequestViewModel leaveRequestViewModel = new LeaveRequestViewModel();
                return leaveRequestViewModel;
            }
          

        }

        public async Task<bool> DeleteLeaveRequest(int LeaveId)
        {
            try
            {
                TblLeaveRequest tblLeaveRequest =  await _leaveRequestRepository.GetLeaveRequest(LeaveId); 

                if (tblLeaveRequest != null)
                {
                    UserInformationViewModel userInformationViewModel = UserInfoHelper.GetUserInfo(_httpContextAccessor.HttpContext);
                    tblLeaveRequest.DeleteByName = userInformationViewModel.Name;
                    tblLeaveRequest.DeleteById = userInformationViewModel.UserId;
                    tblLeaveRequest.DeleteDate = DateTime.Now;
                    tblLeaveRequest.Status = "Cancelled";
                }
                await _leaveRequestRepository.AddEditLeaveRequest(tblLeaveRequest!);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}\n{ex.InnerException?.Message}");
                return false;
            }

        }

    }
}
