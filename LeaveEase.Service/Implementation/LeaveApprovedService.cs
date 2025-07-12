
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
    public class LeaveApprovedService: ILeaveApprovedService
    {

        private readonly ILeaveApprovedRepository _leaveApprovedRepository;

        private readonly ILeaveRequestRepository _leaveRequestRepository;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IHomeRepository _homeRepository;

        private readonly IMapper _mapper;

        public LeaveApprovedService(ILeaveApprovedRepository leaveApprovedRepository, ILeaveRequestRepository leaveRequestRepository, IHomeRepository homeRepository, IMapper mapper,IHttpContextAccessor httpContextAccessor)
            {
                _leaveApprovedRepository = leaveApprovedRepository;
                _leaveRequestRepository = leaveRequestRepository;
                _homeRepository = homeRepository;
                _mapper = mapper;
               _httpContextAccessor= httpContextAccessor;
        }



        public async Task<PaginationViewModel<LeaveApproveViewModel>> GetAllLeaveRequestList(string search, int page, int pageSize, string Status, string LeaveDates,string SortbyName,string SortbyFromdate,string SortbyTodate,string SortbyLeavetype)
        {
            try
            {

                //Login Person Detail
                UserInformationViewModel userInformationViewModel = UserInfoHelper.GetUserInfo(_httpContextAccessor.HttpContext);
                string UserRole = userInformationViewModel.Role!;

                //Create Object so Use In this Method
                List<TblLeaveRequest> userList = new List<TblLeaveRequest>();
                if (UserRole == RoleConstant.Admin)
                {
                     int UserId = userInformationViewModel.UserId;
                     userList = await _leaveApprovedRepository.GetAllLeaveRequestList(UserId);
                }
                else
                {      
                    userList = await _leaveApprovedRepository.GetAllLeaveRequestList(null);
                }

                 var tableCount = 0;

                List<LeaveApproveViewModel> LeaveApprovedViewModel = userList.Select(c => new LeaveApproveViewModel
                {
                    LeaveId = c.LeaveId,
                    FromDate = c.FromDate,
                    ToDate = c.ToDate,
                    Reason = c.Reason,
                    AppliedDate = c.AppliedDate,
                    LeaveTypeString = DisplayNameHelper.GetDisplayName(Enum.Parse<Entity.Enum.LeaveType>(c.LeaveType)),
                    Status = Enum.Parse<Entity.Enum.Status>(c.Status),
                    EmployeeName = c.CreateByName

                }).ToList();



                
                if (!string.IsNullOrEmpty(Status))
                {
                    LeaveApprovedViewModel = LeaveApprovedViewModel.Where(u => u.Status.ToString().Contains(Status, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                if (!string.IsNullOrEmpty(search))
                {
                    LeaveApprovedViewModel = LeaveApprovedViewModel.Where(u => u.EmployeeName!.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                LeaveApprovedViewModel = LeaveApprovedViewModel.OrderByDescending(u => u.FromDate).ToList();


                switch (SortbyName)
                {
                    case OrderByConstant.asc_by:
                        LeaveApprovedViewModel = LeaveApprovedViewModel.OrderBy(u => u.EmployeeName).ToList();
                        break;
                    case OrderByConstant.dec_by:
                        LeaveApprovedViewModel = LeaveApprovedViewModel.OrderByDescending(u => u.EmployeeName).ToList();
                        break;
                }

                switch (SortbyFromdate)
                {
                    case OrderByConstant.asc_by:
                        LeaveApprovedViewModel = LeaveApprovedViewModel.OrderBy(u => u.FromDate).ToList();
                        break;
                    case OrderByConstant.dec_by:
                        LeaveApprovedViewModel = LeaveApprovedViewModel.OrderByDescending(u => u.FromDate).ToList();
                        break;
                }

                switch (SortbyTodate)
                {
                    case OrderByConstant.asc_by:
                        LeaveApprovedViewModel = LeaveApprovedViewModel.OrderBy(u => u.ToDate).ToList();
                        break;
                    case OrderByConstant.dec_by:
                        LeaveApprovedViewModel = LeaveApprovedViewModel.OrderByDescending(u => u.ToDate).ToList();
                        break;
                }

                switch (SortbyLeavetype)
                {
                    case OrderByConstant.asc_by:
                        LeaveApprovedViewModel = LeaveApprovedViewModel.OrderBy(u => u.LeaveTypeString).ToList();
                        break;
                    case OrderByConstant.dec_by:
                        LeaveApprovedViewModel = LeaveApprovedViewModel.OrderByDescending(u => u.LeaveTypeString).ToList();
                        break;
                }

                if (!string.IsNullOrEmpty(LeaveDates) && LeaveDates != "all")
                {
                    DateTime now = DateTime.Now;
                    if (int.TryParse(LeaveDates, out int selectedMonth) && selectedMonth >= 1 && selectedMonth <= 12)
                    {
                        var startOfSelectedMonth = new DateTime(now.Year, selectedMonth, 1);
                        var endOfSelectedMonth = startOfSelectedMonth.AddMonths(1).AddDays(-1);

                        LeaveApprovedViewModel = LeaveApprovedViewModel
                            .Where(o => o.FromDate.ToDateTime(TimeOnly.MinValue) >= startOfSelectedMonth
                                     && o.FromDate.ToDateTime(TimeOnly.MinValue) <= endOfSelectedMonth)
                            .ToList();
                    }
                }
                tableCount = LeaveApprovedViewModel.Count;

                LeaveApprovedViewModel = LeaveApprovedViewModel.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                PaginationViewModel<LeaveApproveViewModel> LeaveApproved = new PaginationViewModel<LeaveApproveViewModel>
                {
                    LeaveApproved = LeaveApprovedViewModel,
                    TotalCount = tableCount,
                    PageIndex = page,
                    PageSize = pageSize,
                };

                return LeaveApproved;
            }
            catch
            {
                return null!;
            }
        }

        public async Task<LeaveApproveViewModel> GetLeaveRequest(int LeaveId)
        {
            try
            {
                //Get Leave Detail Useing Leave Id
                TblLeaveRequest tblLeaveRequest = await _leaveRequestRepository.GetLeaveRequest(LeaveId);

                //TblLeaveRequest to Map LeaveApproveViewModel
                LeaveApproveViewModel leaveApproveViewModel = _mapper.Map<LeaveApproveViewModel>(tblLeaveRequest);
                if(tblLeaveRequest.Status != StatusConstant.Pending)
                {
                    leaveApproveViewModel.Remark = await _leaveApprovedRepository.GetLeaveApprove(LeaveId);
                }
                return leaveApproveViewModel;
            }catch
            {
                return null!;
            }

        }


        public async Task<bool> UpdateLeaveRequest(LeaveApproveViewModel leaveApproveViewModel)
        {
            try
            {
                //Get Leave Request Data
                TblLeaveRequest tblLeaveRequest = await _leaveRequestRepository.GetLeaveRequest(leaveApproveViewModel.LeaveId);

                //Login Person detail
                UserInformationViewModel userInformationViewModel = UserInfoHelper.GetUserInfo(_httpContextAccessor.HttpContext);


                TblLeaveApprove tblLeaveApprove = new TblLeaveApprove();
                tblLeaveApprove.LeaveRequestId = leaveApproveViewModel.LeaveId;
                tblLeaveApprove.ApprovedBy= userInformationViewModel.UserId;
                tblLeaveApprove.Remark = leaveApproveViewModel.Remark;
                tblLeaveApprove.Status = leaveApproveViewModel.Status.ToString();
                tblLeaveApprove.IsActive = true;
                tblLeaveApprove.CreateById = userInformationViewModel.UserId;
                tblLeaveApprove.CreateByName = userInformationViewModel.Name;
               

                bool IsSuccess = await _leaveApprovedRepository.AddLeaveApprove(tblLeaveApprove);

                if (IsSuccess)
                {
                   
                    tblLeaveRequest.UpdateDate = DateTime.UtcNow;                   
                    tblLeaveRequest.UpdateByName = userInformationViewModel.Name;
                    tblLeaveRequest.UpdateById = userInformationViewModel.UserId;
                    tblLeaveRequest.Status = leaveApproveViewModel.Status.ToString();
                    await _leaveRequestRepository.AddEditLeaveRequest(tblLeaveRequest);
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> CancelLeaveRequest(int LeaveId)
        {
            try
            {
                TblLeaveRequest tblLeaveRequest = await _leaveRequestRepository.GetLeaveRequest(LeaveId);
                UserInformationViewModel userInformationViewModel = UserInfoHelper.GetUserInfo(_httpContextAccessor.HttpContext);
              
               
                TblLeaveApprove tblLeaveApprove = new TblLeaveApprove();
                tblLeaveApprove.LeaveRequestId = LeaveId;
                tblLeaveApprove.ApprovedBy = userInformationViewModel.UserId;
                tblLeaveApprove.Status = "Cancelled";
                tblLeaveApprove.IsActive = true;
                tblLeaveApprove.DeleteById = userInformationViewModel.UserId;
                tblLeaveApprove.DeleteByName = userInformationViewModel.Name;
                tblLeaveApprove.DeleteDate=DateTime.UtcNow;

                bool IsSuccess = await _leaveApprovedRepository.AddLeaveApprove(tblLeaveApprove);

                if (IsSuccess)
                {                  
                    tblLeaveRequest.UpdateDate = DateTime.UtcNow;
                    tblLeaveRequest.UpdateById = userInformationViewModel.UserId;
                    tblLeaveRequest.UpdateByName = userInformationViewModel.Name;
                    tblLeaveRequest.Status = "Cancelled";
                    await _leaveRequestRepository.AddEditLeaveRequest(tblLeaveRequest);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
