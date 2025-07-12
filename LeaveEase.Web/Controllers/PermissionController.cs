using System.Net.NetworkInformation;
using System.Threading.Tasks;
using LeaveEase.Entity.ViewModel;
using LeaveEase.Entity.Constants;
using LeaveEase.Service.Interfaces;
using LeaveEase.Service.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace LeaveEase.Web.Controllers
{
    [ServiceFilter(typeof(PermissionFilter))]
    public class PermissionController : Controller
    {

        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }
        public async Task<IActionResult> Index()
        {
           RolePermissionViewModel rolePermissionViewModel=await _permissionService.GetPermissionsByRole();
           return View(rolePermissionViewModel);
        }
        public async Task<IActionResult> SavePermissions([FromBody] List<RolePermissionMap> permissionUpdates)
        {
            try
            {
                if (permissionUpdates == null)
                {
                    TempData["error"] = Messages.ErrorMessageUpdatePermissions;
                    return RedirectToAction("Index");
                }
                bool isUpdated = await _permissionService.UpdateRolePermissions(permissionUpdates);

                if (isUpdated)
                {
                    TempData["success"] = Messages.SuccessMessageUpdatePermissions;
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["error"] = Messages.ErrorMessageUpdatePermissions;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = Messages.ErrorMessageUpdatePermissions;
                return RedirectToAction("Index");
            }
        }
    }
}
