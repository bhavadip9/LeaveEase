
using LeaveEase.Entity.ViewModel;
using Microsoft.AspNetCore.Http;

namespace LeaveEase.Service.Interfaces
{
    public interface ISessionService
    {
         void SetUser(HttpContext httpContext, UserRegisterViewModel user);

        UserRegisterViewModel? GetUser(HttpContext httpContext);

        void ClearSession(HttpContext httpContext);

    }
}
