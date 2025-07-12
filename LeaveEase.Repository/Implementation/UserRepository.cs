
using LeaveEase.Entity.Constants;
using LeaveEase.Entity.Models;
using LeaveEase.Entity.ViewModel;
using LeaveEase.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeaveEase.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly LeaveEaseDbContext _context;

        public UserRepository(LeaveEaseDbContext context)
        {
            _context = context;
        }


        public UserRegisterViewModel IsUserExist(string email, string Phone)
        {
            try
            {
                UserRegisterViewModel userRegisterViewModel = new UserRegisterViewModel();
                if (!string.IsNullOrEmpty(email))
                {
                    bool isExist = _context.TblUsers.Any(c => c.Email.ToLower() == email.ToLower() && c.IsActive);
                    if (isExist)
                    {
                        userRegisterViewModel.Email = string.Empty;
                    }
                    else
                    {
                        userRegisterViewModel.Email = email;
                    }
                }
                if (!string.IsNullOrEmpty(Phone))
                {
                    bool isExist = _context.TblUsers.Any(c => c.MobileNumber == Phone && c.IsActive);
                    if (isExist)
                    {
                        userRegisterViewModel.MobileNumber = string.Empty;
                    }
                    else
                    {
                        userRegisterViewModel.MobileNumber = Phone;
                    }
                }
                return userRegisterViewModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}\n{ex.InnerException?.Message}");
                UserRegisterViewModel userRegisterViewModel = new UserRegisterViewModel();
                return userRegisterViewModel;
            }
        }
        /// <summary>
        /// Retrieves a list of active users filtered by the specified role.
        /// </summary>
        /// <remarks>This method filters users based on their active status and role. It includes users
        /// whose role matches the specified role name or users with the default role (RoleId = 3). The method performs
        /// database queries asynchronously.</remarks>
        /// <param name="role">The name of the role to filter users by. Only users with the specified role or the default role (RoleId = 3)
        /// will be included.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of active users matching
        /// the specified role. If an error occurs, an empty list is returned.</returns>
        public async Task<List<TblUser>> GetUserList(string? role ,int? UserId ,string? LoginUserRole)
        {
            try
            {
                if(role == null)
                {
                    if (LoginUserRole == RoleConstant.SuperAdmin)
                    {
                        return await _context.TblUsers.Include(c => c.RoleNavigation).Where(c => c.IsActive).ToListAsync();
                    }
                    if (UserId != null)
                    {
                        return await _context.TblUsers.Where(c => c.IsActive).Include(c => c.RoleNavigation).Where(c => c.UserId == UserId).ToListAsync();
                    }

                    return await _context.TblUsers.Include(c => c.RoleNavigation).Where(c => c.ReportingPerson == UserId && c.IsActive).ToListAsync();
                }
                if(UserId == null)
                {
                    return await _context.TblUsers.Where(c => c.IsActive).Include(c => c.RoleNavigation).Where(c => c.RoleNavigation.RoleName == role || c.RoleNavigation.RoleName == RoleConstant.SuperAdmin).ToListAsync();
                }
              
                if (LoginUserRole == RoleConstant.SuperAdmin)
                {
                    return await _context.TblUsers.Include(c => c.RoleNavigation).Where(c => c.RoleNavigation.RoleName == role && c.IsActive).ToListAsync();
                }
                

                return await _context.TblUsers.Include(c => c.RoleNavigation).Where(c => c.RoleNavigation.RoleName == role && c.ReportingPerson == UserId &&  c.IsActive).ToListAsync();
            }
            catch (Exception)
            {
                return new List<TblUser>();
            } 
        }


        /// <summary>
        /// Retrieves the details of an active user based on the specified user ID.
        /// </summary>
        /// <remarks>This method queries the database for a user with the specified ID who is marked as
        /// active. If no matching user is found or an error occurs during the operation, the method returns <see
        /// langword="null"/>.</remarks>
        /// <param name="userid">The unique identifier of the user to retrieve. Must be a positive integer.</param>
        /// <returns>A <see cref="TblUser"/> object representing the user's details if the user is found and active;  otherwise,
        /// <see langword="null"/>.</returns>
        public async Task<TblUser?> GetUserDetailUsingId(int userid)
        {
            try
            {
                return await _context.TblUsers.Include(c=>c.RoleNavigation).FirstOrDefaultAsync(c => c.UserId == userid && c.IsActive);
            }
            catch
            {
                return null;
            }
        }

        public string GetUserNameById(int UserId)
        {
            try
            {
                return _context.TblUsers.FirstOrDefault(c => c.UserId == UserId)!.FirstName;
            }
            catch
            {
                return null!;
            }
        }

        public UserRegisterViewModel EditTimeCheckExist(string email,string Phone,int id)
        {
            try
            {
                UserRegisterViewModel userRegisterViewModel = new UserRegisterViewModel();
                if (!string.IsNullOrEmpty(email))
                {
                    bool isExist = _context.TblUsers.Any(c => c.Email.ToLower() == email.ToLower() && c.IsActive && c.UserId != id);
                    if (isExist)
                    {
                        userRegisterViewModel.Email = string.Empty;
                    }
                    else
                    {
                        userRegisterViewModel.Email = email;
                    }
                }
                if (!string.IsNullOrEmpty(Phone))
                {
                    bool isExist = _context.TblUsers.Any(c => c.MobileNumber == Phone && c.IsActive && c.UserId != id);
                    if (isExist)
                    {
                        userRegisterViewModel.MobileNumber = string.Empty;
                    }
                    else
                    {
                        userRegisterViewModel.MobileNumber = Phone;
                    }
                }
                return userRegisterViewModel;
               
            }
            catch
            {
                UserRegisterViewModel userRegisterViewModel = new UserRegisterViewModel();
                return userRegisterViewModel;
            }
           
        }

        public  bool UpdateRegitration(TblUser user)
        {
            try
            {
               
                _context.Update(user);
                _context.SaveChanges();
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
