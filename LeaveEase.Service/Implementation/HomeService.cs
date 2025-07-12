using AutoMapper;
using LeaveEase.Entity.Enum;
using LeaveEase.Entity.Models;
using LeaveEase.Entity.ViewModel;
using LeaveEase.Repository.Interfaces;
using LeaveEase.Entity.Constants;
using LeaveEase.Service.Helper;
using LeaveEase.Service.Interfaces;
using LeaveEase.Service.Utills;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace LeaveEase.Service.Implementation
{
    public class HomeService : IHomeService
    {
        private readonly IHomeRepository _homeRepository;

        private readonly ILoginRepository _loginRepository;

        private readonly ILeaveRequestRepository _leaveRequestRepository;

        private readonly ILeaveApprovedRepository _leaveApprovedRepository;

        private readonly IUserRepository _userRepository;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public HomeService(IHomeRepository homeRepository, ILoginRepository loginRepository, IMapper mapper, IUserRepository userRepository,IHttpContextAccessor httpContextAccessor, ILeaveRequestRepository leaveRequestRepository, ILeaveApprovedRepository leaveApprovedRepository)
        {
            _homeRepository = homeRepository;
            _loginRepository = loginRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _leaveRequestRepository = leaveRequestRepository;
            _leaveApprovedRepository = leaveApprovedRepository;
        }


        //public async Task<DashboardViewModel> GetDashBoard()
        //{
        //    DashboardViewModel dashboardViewModel = new DashboardViewModel();
        //    UserInformationViewModel userInformationViewModel = UserInfoHelper.GetUserInfo(_httpContextAccessor.HttpContext);
        //    dashboardViewModel.RoleName = userInformationViewModel.Role;


        //    if (userInformationViewModel.Role == RoleConstant.SuperAdmin)
        //    {
        //        List<TblLeaveRequest> leaveRequest = await _leaveApprovedRepository.GetAllLeaveRequestList(null);
        //        dashboardViewModel.TotalEmployees = leaveRequest.Count();
        //        dashboardViewModel.TotalActiveEmployees = leaveRequest.Count() - leaveRequest.Where(c => c.Status == StatusConstant.Approved).Count();
        //    }
        //    else
        //    {
        //        List<TblLeaveRequest> leaveRequests = await _leaveApprovedRepository.GetAllLeaveRequestList(userInformationViewModel.UserId);
        //        dashboardViewModel.TotalEmployees = leaveRequests.Count();
        //        dashboardViewModel.TotalActiveEmployees = leaveRequests.Count() - leaveRequests.Where(c => c.Status == StatusConstant.Approved).Count();
        //    }

        //    return dashboardViewModel;
        //}

        public async Task<DashboardViewModel> GetDashBoard()
        {
            DashboardViewModel dashboardViewModel = new DashboardViewModel();
            UserInformationViewModel userInformationViewModel = UserInfoHelper.GetUserInfo(_httpContextAccessor.HttpContext);
            dashboardViewModel.RoleName = userInformationViewModel.Role;

            List<TblLeaveRequest> leaveRequests;
            List<TblUser> UserList;

            if (userInformationViewModel.Role == RoleConstant.SuperAdmin)
            {
                leaveRequests = await _leaveApprovedRepository.GetAllLeaveRequestList(null);
                UserList = await _userRepository.GetUserList(null, null, RoleConstant.SuperAdmin);
                dashboardViewModel.TotalEmployees = UserList.Count();
                var employeeIds = UserList.Select(e => e.UserId).ToList();
                dashboardViewModel.TotalActiveEmployees = UserList.Count() - leaveRequests.Where(lr => employeeIds.Contains(lr.EmployeeId) && lr.Status == StatusConstant.Approved && lr.IsActive).Count();
            }
            else if(userInformationViewModel.Role == RoleConstant.Admin)
            {
                leaveRequests = await _leaveApprovedRepository.GetAllLeaveRequestList(userInformationViewModel.UserId);
                UserList = await _userRepository.GetUserList(null, userInformationViewModel.UserId,null);
                dashboardViewModel.TotalEmployees = UserList.Count();
                var employeeIds = UserList.Select(e => e.UserId).ToList();
                dashboardViewModel.TotalActiveEmployees = UserList.Count() - leaveRequests.Where(lr => employeeIds.Contains(lr.EmployeeId) && lr.Status == StatusConstant.Approved && lr.IsActive).Count();
            }
            else
            {
                leaveRequests = await _leaveRequestRepository.GetLeaveRequestList(userInformationViewModel.UserId);
            }

            var currentMonthLeaves = leaveRequests.Where(l => l.FromDate.Month == DateTime.Now.Month && l.Status == StatusConstant.Approved).ToList();
            dashboardViewModel.MonthlyLeavesCount = currentMonthLeaves.Count;

           
            dashboardViewModel.TotalLeaveDays = currentMonthLeaves.Sum(l =>
                         Enumerable.Range(0, (l.ToDate.ToDateTime(TimeOnly.MinValue) - l.FromDate.ToDateTime(TimeOnly.MinValue)).Days + 1)
                         .Select(offset => l.FromDate.ToDateTime(TimeOnly.MinValue).AddDays(offset))
                         .Count(date => date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                        );
 
            dashboardViewModel.MonthlyLeaveDates = currentMonthLeaves
                             .SelectMany(l => Enumerable.Range(0, (l.ToDate.ToDateTime(TimeOnly.MinValue) - l.FromDate.ToDateTime(TimeOnly.MinValue)).Days + 1)
                             .Select(offset => l.FromDate.ToDateTime(TimeOnly.MinValue).AddDays(offset)))
                             .Where(date => date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                             .Distinct()
                             .ToList();

            // Pending leave requests count  
            dashboardViewModel.PendingLeaveRequestsCount = leaveRequests.Count(l => l.Status == StatusConstant.Pending);

            // Upcoming leaves (leaves starting after today)  
            //dashboardViewModel.UpcomingLeaves = leaveRequests
            //    .Where(l => l.FromDate.ToDateTime(TimeOnly.MinValue) > DateTime.Now.Date && l.Status == StatusConstant.Approved)
            //    .Select(l => new UpcomingLeaveViewModel
            //    {
            //        EmployeeName = l.Employee.FirstName,
            //        LeaveDateRange = FormatDateRange(l.FromDate.ToDateTime(TimeOnly.MinValue), l.ToDate.ToDateTime(TimeOnly.MinValue))
            //    })
            //    .ToList();

            // Leaves taken this month (approved leaves in current month)  
            dashboardViewModel.LeavesTakenThisMonth = currentMonthLeaves.Count;

            // Departments count (fetch or calculate accordingly)  
            dashboardViewModel.DepartmentsCount = 5;

            return dashboardViewModel;
        }

        // Helper to format date range nicely  
        private string FormatDateRange(DateTime start, DateTime end)
        {
            if (start == end)
                return start.ToString("MMM d");
            else
                return $"{start:MMM d}–{end:MMM d}";
        }

        public string GetRoleName(int Roleid)
        {
            return _homeRepository.GetRoleName(Roleid);
        }


        public UserInformationViewModel GetUserDetail()
        {
            try
            {
                UserInformationViewModel userInformationViewModel = UserInfoHelper.GetUserInfo(_httpContextAccessor.HttpContext);
                return userInformationViewModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}\n{ex.InnerException?.Message}");
                return null!;
            }
        }


        /// <summary>
        /// Retrieves the profile data of the currently logged-in user.
        /// </summary>
        /// <remarks>This method fetches the user ID of the currently logged-in user, retrieves the
        /// corresponding  user details from the database, and maps them to a <see cref="UserRegisterViewModel"/>.  If
        /// an error occurs during the process, an empty view model is returned instead of propagating  the
        /// exception.</remarks>
        /// <returns>A <see cref="UserRegisterViewModel"/> object containing the user's profile information,  including role,
        /// department, and profile image. If the user is not found, an empty  <see cref="UserRegisterViewModel"/> is
        /// returned.</returns>
        public async Task<UserRegisterViewModel> GetUserProfileData()
        {

            try
            {
                UserInformationViewModel userInformationViewModel = UserInfoHelper.GetUserInfo(_httpContextAccessor.HttpContext);            
                int UserId = userInformationViewModel.UserId;     
                TblUser? userList = await _userRepository.GetUserDetailUsingId(UserId);
                List<TblUser> userListAdmin = await _userRepository.GetUserList(RoleConstant.Admin, null,null);

              
                UserRegisterViewModel user = _mapper.Map<UserRegisterViewModel>(userList);
                user.Role = userList.Role;

                List<UserRegisterViewModel> userRegisterViewModels = userListAdmin.Select(d => new UserRegisterViewModel
                {
                    UserId = d.UserId,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                }).ToList();

                user.AdminList = userRegisterViewModels;
                user.ReportingPerson = userList.ReportingPerson;

                if (Enum.TryParse<Department>(userList.Department, out var department))
                {
                    user.Department = department;
                }  
                user.ProfileImage = userList.ProfileImg;

                return user;
            }
            catch
            {
                return new UserRegisterViewModel();
            }

        }


      


        /// <summary>
        /// Updates the user's password if the provided old password is correct.
        /// </summary>
        /// <remarks>This method verifies the old password before updating the user's password. If the old
        /// password does not match,  the <c>OldPassword</c> property of the input model is cleared, and the model is
        /// returned without making any changes. If an exception occurs during the operation, the method returns
        /// <c>null</c>.</remarks>
        /// <param name="changePasswordViewModel">An object containing the user's email, old password, and new password.</param>
        /// <returns>A <see cref="ChangePasswordViewModel"/> object with the updated password information if the operation
        /// succeeds;  otherwise, returns the input model with the <c>OldPassword</c> property cleared if the old
        /// password is incorrect,  or <c>null</c> if an error occurs.</returns>
       
        public ChangePasswordViewModel ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            try
            {
                TblUser userData = _loginRepository.GetUserByEmail(changePasswordViewModel.Email);

                //Check Old Password and new input password valid
                bool CheckOldPassword = PasswordUtills.VerifyPassword(changePasswordViewModel.OldPassword, userData.Password);
                if (!CheckOldPassword)
                {
                    changePasswordViewModel.OldPassword = string.Empty;
                    return changePasswordViewModel;
                }

                //Check New Password and old password same then give validation
                bool CheckSamePassword = PasswordUtills.VerifyPassword(changePasswordViewModel.NewPassword, userData.Password);
                if (CheckSamePassword)
                {
                    changePasswordViewModel.NewPassword = string.Empty;
                    return changePasswordViewModel;
                }
                string NewPasswod = PasswordUtills.HashPassword(changePasswordViewModel.NewPassword);
                userData.Password = NewPasswod;
                bool IsChange= _loginRepository.UpdatePassword(userData);
                if (!IsChange)
                {
                    changePasswordViewModel.ConfirmPassword = string.Empty;
                    return changePasswordViewModel;
                }
                return changePasswordViewModel;
            }
            catch (Exception)
            {
                return null!;
            }
        }
    }
}
