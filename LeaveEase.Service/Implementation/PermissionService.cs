using AutoMapper;
using LeaveEase.Entity.Models;
using LeaveEase.Entity.ViewModel;
using LeaveEase.Repository.Interfaces;
using LeaveEase.Service.Helper;
using LeaveEase.Service.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LeaveEase.Service.Implementation
{
    public class PermissionService: IPermissionService
    {

        private readonly IPermissionRepository _permissionRepository;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IHomeRepository _homeRepository;

        private readonly IMapper _mapper;
        public PermissionService(IPermissionRepository permissionRepository,IMapper mapper,IHttpContextAccessor httpContextAccessor,IHomeRepository homeRepository)
        {
            _permissionRepository = permissionRepository;
            _httpContextAccessor = httpContextAccessor;
            _homeRepository = homeRepository;
            _mapper = mapper;
        }


       

        public async Task<RolePermissionViewModel> GetPermissionsByRole()
        {
            try
            {
                List<TblRole> tblRoles = await _permissionRepository.GetPermissionsByRole();

                UserInformationViewModel userInformationViewModel = UserInfoHelper.GetUserInfo(_httpContextAccessor.HttpContext);
               
                List<TblPermission> tblPermissions = await _permissionRepository.GetPermissions();

                List<PermissionViewModel> Permissions = tblPermissions
                                  .GroupBy(p => p.PermissionName)
                                  .Select(g => new PermissionViewModel
                                   {
                                       PermissionName = g.Key,
                                       RolePermissions = tblRoles.Select(role =>
                                       {
                                           var match = g.FirstOrDefault(r => r.Role == role.RoleId);
                                           return new RolePermissionMap
                                           {
                                               PermissionId=match?.PermissionId??0,
                                               RoleId = role.RoleId,
                                               CanView = match?.CanView ?? false,
                                               CanAddEdit = match?.CanAddEdit ?? false,
                                               CanDelete = match?.CanDelete ?? false
                                           };
                                       }).ToList()
                                   }).ToList();

                RolePermissionViewModel rolePermissionViewModel = new RolePermissionViewModel
                {
                    roleViewModels = tblRoles.Select(role => new RoleViewModel
                    {
                        RoleId = role.RoleId,
                        RoleName = role.RoleName
                    }).ToList(),
                    Permissions= Permissions,
                    RoleName =  userInformationViewModel.Role
                };
               

                return rolePermissionViewModel;
            }
            catch
            {
                RolePermissionViewModel rolePermissionViewModel = new RolePermissionViewModel();
                return rolePermissionViewModel;
            }
           
        }


      

        public async Task<bool> UpdateRolePermissions(List<RolePermissionMap> permissionUpdates)
        {
            try
            {

                List<int> permissionIds = permissionUpdates.Select(p => p.PermissionId).Distinct().ToList();


                List<TblPermission> permissions = await _permissionRepository.GetPermissionsbyPremissonid(permissionIds);

                UserInformationViewModel userInformationViewModel = UserInfoHelper.GetUserInfo(_httpContextAccessor.HttpContext);
                foreach (var update in permissionUpdates)
                {
                    var permission = permissions
                        .FirstOrDefault(p => p.PermissionId == update.PermissionId && p.Role == update.RoleId);

                    if (permission != null)
                    {
                        permission.CanView = update.CanView;
                        permission.CanAddEdit = update.CanAddEdit;
                        permission.CanDelete = update.CanDelete;
                        permission.UpdatedDate = DateTime.Now;
                        permission.UpdatedById = userInformationViewModel.UserId;
                        permission.UpdatedByName = userInformationViewModel.Name;
                    }
                }
                await _permissionRepository.UpdatePermissions(permissions);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
