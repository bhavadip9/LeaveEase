
using LeaveEase.Entity.ViewModel;
using Microsoft.AspNetCore.Http;

namespace LeaveEase.Service.Interfaces
{
    public interface ICookieService
    {

         void SaveJWTToken(HttpResponse response, string token);

        string? GetJWTToken(HttpRequest request);

        void SaveRememberMe(HttpResponse response, UserRegisterViewModel user);

        void ClearCookies(HttpContext httpContext);
    }
}
