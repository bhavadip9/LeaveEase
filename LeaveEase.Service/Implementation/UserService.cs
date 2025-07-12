
using System.Data;
using AutoMapper;
using LeaveEase.Entity.Enum;
using LeaveEase.Entity.Models;
using LeaveEase.Entity.ViewModel;
using LeaveEase.Repository.Interfaces;
using LeaveEase.Entity.Constants;
using LeaveEase.Service.Helper;
using LeaveEase.Service.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LeaveEase.Service.Implementation
{
    public class UserService: IUserService
    {

        private readonly IUserRepository _userRepository;

        private readonly IHomeRepository _homeRepository;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper, IHomeRepository homeRepository , IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _homeRepository = homeRepository;
            _httpContextAccessor= httpContextAccessor;
        }

        /// <summary>
        /// Retrieves the registration data required for creating a new user.
        /// </summary>
        /// <remarks>The returned <see cref="UserRegisterViewModel"/> includes a collection of roles
        /// represented as <see cref="RoleViewModel"/> objects and the name of the user who is creating the registration
        /// data.</remarks>
        /// <returns>A <see cref="UserRegisterViewModel"/> containing the list of available roles and the name of the user who
        /// initiated the request.</returns>
        public async Task<UserRegisterViewModel> GetRegistrationData()
        {
            //Get All Role
            List<TblRole> tblRoles = _homeRepository.GetEntities<TblRole>();

            //User Information
            UserInformationViewModel userInformationViewModel = UserInfoHelper.GetUserInfo(_httpContextAccessor.HttpContext);

            //Get All Admin User
            List<TblUser> userList =await _userRepository.GetUserList(RoleConstant.Admin,null,null);


            List<UserRegisterViewModel> userRegisterViewModels = userList.Select(d => new UserRegisterViewModel
            {
                UserId = d.UserId,
                FirstName = d.FirstName,
                LastName = d.LastName
            }).ToList();
            string CreateByName = userInformationViewModel.Name!; 
            List<RoleViewModel> roleVMs = tblRoles.Where(c=>c.RoleName!= RoleConstant.SuperAdmin).Select(d => new RoleViewModel
            {
                RoleId = d.RoleId,
                RoleName = d.RoleName!
            }).ToList();
            UserRegisterViewModel userRegisterVM = new UserRegisterViewModel
            {
                RoleVM = roleVMs,
                CreateByName = CreateByName,
                AdminList= userRegisterViewModels
            };

            return userRegisterVM;
        }


        /// <summary>
        /// Determines whether the specified email address exists in the data store.
        /// </summary>
        /// <remarks>This method checks the existence of an email address by querying the underlying data
        /// store. Ensure that the provided email address is properly formatted before calling this method.</remarks>
        /// <param name="email">The email address to check for existence. Cannot be null or empty.</param>
        /// <returns><see langword="true"/> if the email address exists; otherwise, <see langword="false"/>.</returns>
        public UserRegisterViewModel IsUserExist(string email ,string Phone)
        {
            return _userRepository.IsUserExist(email, Phone);
        }



        /// <summary>
        /// Adds a new user registration to the system.
        /// </summary>
        /// <remarks>This method maps the provided user registration details to a database entity and
        /// attempts to save it. If an exception occurs during the operation, the method logs the exception and returns
        /// <see langword="false"/>.</remarks>
        /// <param name="userRegisterVM">An object containing the user's registration details. This parameter must not be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the registration was successfully added; otherwise, <see langword="false"/>.</returns>      
        public bool AddRegistration(UserRegisterViewModel userRegisterVM)
        {
            try
            {
                TblUser user = _mapper.Map<TblUser>(userRegisterVM);
                user.ProfileImg = userRegisterVM.ProfileImage;
                UserInformationViewModel userInformationViewModel = UserInfoHelper.GetUserInfo(_httpContextAccessor.HttpContext); 
                user.CreatedByName = userInformationViewModel.Name;
                user.CreatedById = userInformationViewModel.UserId;
               
                _homeRepository.AddRegistration(user);
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}\n{ex.InnerException?.Message}");
                return false;
            }
        }


        public async Task<PaginationViewModel<UserListViewModel>> GetUserList(string role,string search, int page, int pageSize,string orderby)
        {
            UserInformationViewModel userInformationViewModel = UserInfoHelper.GetUserInfo(_httpContextAccessor.HttpContext);
            int UserId = userInformationViewModel.UserId;
            string UserRole = userInformationViewModel.Role;
 
            List<TblUser> userList = await _userRepository.GetUserList(role,UserId, UserRole);

            var tableCount = 0;
            List<UserListViewModel> UserViewModel = userList.Select(c => new UserListViewModel
            {
                UserId = c.UserId,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                Address = c.Address!,
                Department = c.Department ?? string.Empty, 
                MobileNumber = c.MobileNumber,
                CreateBy = c.CreatedByName ?? string.Empty, 
                RoleName = c.RoleNavigation.RoleName ?? string.Empty, 
                BirthDate = c.BirthDate,
                ReportingPerson= _userRepository.GetUserNameById(c.ReportingPerson)
            }).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                UserViewModel = UserViewModel.Where(u =>
                    (u.FirstName != null && u.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (u.LastName.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (u.Department.Contains(search, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            if (orderby == OrderByConstant.asc_by)
            {
                UserViewModel = UserViewModel.OrderBy(u => u.Department).ToList();
            }
            else if (orderby == OrderByConstant.dec_by)
            {
                UserViewModel = UserViewModel.OrderByDescending(u => u.Department).ToList();
            }

            tableCount = UserViewModel.Count;

            UserViewModel = UserViewModel.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            PaginationViewModel<UserListViewModel> user = new PaginationViewModel<UserListViewModel>
            {
                Users = UserViewModel,
                TotalCount = tableCount,
                PageIndex = page,
                PageSize = pageSize,
                RoleName= role
            };

            return user;
        }

        public async Task<UserRegisterViewModel> GetUserDetailUsingId(int id)
        {
            //Get User Detail Uesing UserId
            TblUser? userList = await _userRepository.GetUserDetailUsingId(id);

            if (userList == null)
            {
                throw new InvalidOperationException($"User with ID {id} not found.");
            }

            //Map User Table to UserRegisterViewModel
            UserRegisterViewModel user = _mapper.Map<UserRegisterViewModel>(userList);
            if (Enum.TryParse<Department>(userList.Department, out var department))
            {
                user.Department = department;
            }
            user.Role = userList.Role;
            user.ProfileImage = userList.ProfileImg;

            return user;
        }

        public UserRegisterViewModel EditUserExise(string Email ,string MobilePhone,int userid)
        {
            return _userRepository.EditTimeCheckExist(Email, MobilePhone, userid);
        }

        public async Task<bool> EditRegistration(UserRegisterViewModel userRegisterViewModel)
        {
            try
            {
                TblUser? userData = await _userRepository.GetUserDetailUsingId(userRegisterViewModel.UserId ?? 0);

                userData.FirstName = userRegisterViewModel.FirstName;
                userData.LastName = userRegisterViewModel.LastName;
                userData.MobileNumber = userRegisterViewModel.MobileNumber!;
                userData.Department = userRegisterViewModel.Department.ToString();
                userData.BirthDate = userRegisterViewModel.BirthDate.HasValue
                    ? userRegisterViewModel.BirthDate.Value
                    : default;
                userData.Email = userRegisterViewModel.Email;
                userData.Role = userRegisterViewModel.Role;
                userData.Address = userRegisterViewModel.Address;
                userData.ReportingPerson = userRegisterViewModel.ReportingPerson;

                UserInformationViewModel userInformationViewModel = UserInfoHelper.GetUserInfo(_httpContextAccessor.HttpContext);
                userData.UpdatedByName = userInformationViewModel.Name;
                userData.UpdatedById = userInformationViewModel.UserId;
                userData.UpdatedDate = DateTime.Now;

                if (userRegisterViewModel.ProfileImage == null)
                {
                    userData.ProfileImg = userData.ProfileImg;
                }
                else
                {
                    userData.ProfileImg = userRegisterViewModel.ProfileImage;
                }

                _userRepository.UpdateRegitration(userData);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}\n{ex.InnerException?.Message}");
                return false;
            }
        }

        public async Task<bool> IsAdminUnderEmployee(int UserId)
        {
             TblUser? userData = await _userRepository.GetUserDetailUsingId(UserId);
            if(userData.IsActive && userData.RoleNavigation.RoleName == RoleConstant.Admin)
            {
                List<TblUser> userList = await _userRepository.GetUserList(RoleConstant.Admin, UserId, null);
                if (userList.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {      
                return false;
            }
        }

        public async Task<bool> DeleteUser(int UserID)
        {
            try
            {
                TblUser? userData = await _userRepository.GetUserDetailUsingId(UserID);

                if (userData == null)
                {
                    throw new InvalidOperationException($"User with ID {UserID} not found.");
                }
                UserInformationViewModel userInformationViewModel = UserInfoHelper.GetUserInfo(_httpContextAccessor.HttpContext);
                userData.DeletedByName = userInformationViewModel.Name;
                userData.DeletedById = userInformationViewModel.UserId;
                userData.DeletedDate = DateTime.Now;
                userData.IsActive = false;

                return _userRepository.UpdateRegitration(userData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}\n{ex.InnerException?.Message}");
                return false;
            }
        }
      
    } 
}
