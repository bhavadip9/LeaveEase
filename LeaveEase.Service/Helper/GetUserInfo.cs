
using System.Security.Claims;
using LeaveEase.Entity.ViewModel;
using Microsoft.AspNetCore.Http;

namespace LeaveEase.Service.Helper
{
    public class UserInfoHelper 
    {
        public static UserInformationViewModel GetUserInfo(HttpContext? context)
        {
            var email = context?.User.FindFirst(ClaimTypes.Email)?.Value;
            var role = context?.User.FindFirst(ClaimTypes.Role)?.Value;
            var name = context?.User.FindFirst(ClaimTypes.Name)?.Value;
            var userIdClaim = context?.User.FindFirst("UserId")?.Value;

            int.TryParse(userIdClaim, out int userId);

            return new UserInformationViewModel
            {
                Email = email,
                Name = name,
                UserId = userId,
                Role=role
            };
        }
    }
}
