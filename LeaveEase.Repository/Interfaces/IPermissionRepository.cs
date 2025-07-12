
using LeaveEase.Entity.Models;

namespace LeaveEase.Repository.Interfaces
{
    public interface IPermissionRepository
    {
        Task<List<TblPermission>> GetPermissionsbyPremissonid(List<int> permissionIds);

        Task UpdatePermissions(List<TblPermission> updatedPermissions);
        List<TblPermission> GetAllPermissionByRole(string Role);

        Task<List<TblRole>> GetPermissionsByRole();

        Task<List<TblPermission>> GetPermissions();
    }
}
