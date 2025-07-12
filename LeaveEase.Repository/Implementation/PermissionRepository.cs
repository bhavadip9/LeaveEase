
using LeaveEase.Entity.Constants;
using LeaveEase.Entity.Models;
using LeaveEase.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeaveEase.Repository.Implementation
{
    public class PermissionRepository: IPermissionRepository
    {

        private readonly LeaveEaseDbContext _context;
        public PermissionRepository(LeaveEaseDbContext context)
        {
            _context = context;
        }


      


        /// <summary>
        /// Retrieves a list of permissions associated with the specified role.
        /// </summary>
        /// <remarks>This method filters out permissions with the name "Home" from the result.</remarks>
        /// <param name="RoleId">The unique identifier of the role for which permissions are to be retrieved.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of  <see
        /// cref="TblPermission"/> objects associated with the specified role. If an exception occurs,  an empty list is
        /// returned.</returns>
        public async Task<List<TblPermission>> GetPermissionsbyPremissonid(List<int> permissionIds)
        {
            try
            {
                List<TblPermission> result = await _context.TblPermissions.Where(p => permissionIds.Contains(p.PermissionId)) .ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}\n{ex.InnerException?.Message}");
                return new List<TblPermission>();
            }
        }
        public async Task<List<TblPermission>> GetPermissions()
        {
            try
            {
                List<TblPermission> result = await _context.TblPermissions.Include(c=>c.RoleNavigation).Where(c=>c.PermissionName!= PermissonPage.Home).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}\n{ex.InnerException?.Message}");
                return new List<TblPermission>();

            }
        }
        public async Task<List<TblRole>> GetPermissionsByRole()
        {
            try
            {
                List<TblRole> result = await _context.TblRoles.Include(c=>c.TblPermissions).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}\n{ex.InnerException?.Message}");
                return new List<TblRole>();

            }
        }


        /// <summary>
        /// Updates the specified list of permissions in the database.
        /// </summary>
        /// <remarks>This method applies the updates to the provided permissions and saves the changes to
        /// the database. If a database update error occurs, the exception is logged to the console.</remarks>
        /// <param name="updatedPermissions">A list of <see cref="TblPermission"/> objects representing the updated permissions. Each object must
        /// correspond to an existing record in the database.</param>
        /// <returns></returns>
        public async Task UpdatePermissions(List<TblPermission> updatedPermissions)
        {
            try
            {
                _context.TblPermissions.UpdateRange(updatedPermissions);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
            }
        }

        /// <summary>
        /// Retrieves all permissions associated with the specified role.
        /// </summary>
        /// <remarks>This method queries the database to retrieve permissions linked to the specified
        /// role. If an exception occurs during the operation, the method logs the error and returns an empty
        /// list.</remarks>
        /// <param name="RoleName">The name of the role for which permissions are to be retrieved. Cannot be null or empty.</param>
        /// <returns>A list of <see cref="TblPermission"/> objects representing the permissions associated with the specified
        /// role. Returns an empty list if no permissions are found or if an error occurs.</returns>
        public List<TblPermission> GetAllPermissionByRole(string RoleName)
        {
            try
            {
                List<TblPermission> Permissions = _context.TblPermissions.Where(c => c.RoleNavigation.RoleName == RoleName).ToList();
                return Permissions;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}\n{ex.InnerException?.Message}");
                return new List<TblPermission>();
            }

        }
    }
}
