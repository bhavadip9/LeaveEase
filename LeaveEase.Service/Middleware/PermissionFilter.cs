
using LeaveEase.Entity.Models;
using LeaveEase.Entity.ViewModel;
using LeaveEase.Repository.Interfaces;
using LeaveEase.Service.Helper;
using LeaveEase.Service.Implementation;
using LeaveEase.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LeaveEase.Service.Middleware
{
    public class PermissionFilter : IActionFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IJwtService _jwtService;

        public PermissionFilter(
            IHttpContextAccessor httpContextAccessor,
            IPermissionRepository permissionRepository,
            IJwtService jwtService)
        {
            _httpContextAccessor = httpContextAccessor;
            _permissionRepository = permissionRepository;
            _jwtService = jwtService;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Get token from cookie and validate
            CookieService cookieService = new CookieService();
            string token = cookieService.GetJWTToken(context.HttpContext.Request)!;
            var principal = _jwtService.ValidateToken(token ?? "");

            if (principal == null)
            {
                context.Result = new RedirectToActionResult("Login", "Login", null);
                return;
            }

            context.HttpContext.User = principal;

            // Get user info from cookie
            UserInformationViewModel userInfo = UserInfoHelper.GetUserInfo(context.HttpContext);
            if (userInfo == null || string.IsNullOrEmpty(userInfo.Role))
            {
                context.Result = new RedirectToActionResult("Logout", "Login", null);
                return;
            }

            string role = userInfo.Role;

            //  Fetch all permissions for the role
            List<TblPermission> rolePermissions = _permissionRepository.GetAllPermissionByRole(role);

            string controllerName = context.ActionDescriptor.RouteValues["controller"] ?? "";
            string actionName = context.ActionDescriptor.RouteValues["action"] ?? "";

            // Match controller to permission name
            TblPermission matchedPermission = rolePermissions.FirstOrDefault(p => p.PermissionName == controllerName)!;

            if (matchedPermission == null || !matchedPermission.CanView)
            {
                var controller = context.Controller as Controller;
                if (controller != null)
                {
                    controller.TempData["ToastrMessage"] = "Permission Denied";
                    controller.TempData["ToastrType"] = "error";
                }

                string referer = context.HttpContext.Request.Headers["Referer"].ToString();

                if (!string.IsNullOrWhiteSpace(referer))
                    context.Result = new RedirectResult(referer);
                else
                    context.Result = new RedirectToActionResult("Error", "Home", null);
                return;


            }

            //  Store permission flags
            context.HttpContext.Items["CanView"] = matchedPermission.CanView;
            context.HttpContext.Items["CanAddEdit"] = matchedPermission.CanAddEdit;
            context.HttpContext.Items["CanDelete"] = matchedPermission.CanDelete;
            context.HttpContext.Items["Permissioncontroller"] = rolePermissions
                .Where(p => p.CanView)
                .Select(p => p.PermissionName)
                .ToList();


            context.HttpContext.Items["Registration"] = rolePermissions
                .Any(p => p.PermissionName == "User" && p.CanAddEdit);

            // Block non-GET if no write permission
            if (context.HttpContext.Request.Method != "GET" && (!matchedPermission.CanAddEdit || !matchedPermission.CanDelete))
            {
                if (context.HttpContext.Request.Headers["Accept"].ToString().Contains("application/json"))
                {
                    context.Result = new JsonResult(new { error = "Permission Denied" }) { StatusCode = StatusCodes.Status403Forbidden };
                }
                else
                {
                    context.HttpContext.Response.Headers.Add("X-Toastr-Message", "Permission Denied");
                    context.HttpContext.Response.Headers.Add("X-Toastr-Type", "error");
                    context.Result = new RedirectResult(context.HttpContext.Request.Headers["Referer"].ToString());
                }

                return;
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No post-action logic required
        }
    }
}