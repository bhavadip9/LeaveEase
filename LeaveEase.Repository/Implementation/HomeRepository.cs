using LeaveEase.Entity.Models;
using LeaveEase.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace LeaveEase.Repository.Implementation
{
    public class HomeRepository : IHomeRepository
    {
        private readonly LeaveEaseDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeRepository(LeaveEaseDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor= httpContextAccessor;

        }

        /// <summary>
        /// Determines whether the specified email address exists in the active user records.
        /// </summary>
        /// <remarks>This method performs a case-insensitive comparison of the email address against
        /// active user records. Ensure that the provided email address is valid and formatted correctly.</remarks>
        /// <param name="email">The email address to check for existence. This parameter cannot be null or empty.</param>
        /// <returns><see langword="true"/> if the email address exists in the active user records; otherwise, <see
        /// langword="false"/>.</returns>
       
        public List<T> GetEntities<T>() where T : class
        {
            try
            {
                return _context.Set<T>().ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}\n{ex.InnerException?.Message}");
                return new List<T>();
            }
        }



        /// <summary>
        /// Add new User
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
       
        public async Task<TblUser> AddRegistration(TblUser userData)
        {
            try
            {
                await _context.AddAsync(userData);
                await _context.SaveChangesAsync();
                return userData;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return new TblUser();
            }
        }


        /// <summary>
        /// Retrieves the name of a role based on its unique identifier.
        /// </summary>
        /// <param name="RoleID">The unique identifier of the role to retrieve.</param>
        /// <returns>The name of the role associated with the specified <paramref name="RoleID"/>. Returns <see langword="null"/>
        /// if the role does not exist or an error occurs.</returns>
        public string GetRoleName(int? RoleID)
       {
            try
            {
                
                    return _context.TblRoles.FirstOrDefault(c => c.RoleId == RoleID)!.RoleName!;
               
            }
            catch (Exception)
            {
                return null!;
            }
         }    
    }
}