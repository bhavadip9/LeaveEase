using System;
using System.Collections.Generic;
using System.Linq;

using LeaveEase.Entity.ViewModel;

namespace LeaveEase.Service.Interfaces
{
    public interface IPermissionService
    {
        Task<RolePermissionViewModel> GetPermissionsByRole();
        Task<bool> UpdateRolePermissions(List<RolePermissionMap> permissionUpdates); 
    }
}
