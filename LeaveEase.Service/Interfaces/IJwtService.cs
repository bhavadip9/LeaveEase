
using System.Security.Claims;


namespace LeaveEase.Service.Interfaces
{
    public interface IJwtService
    {
        string GenerateJwtToken(string name, string email, string RoleName, int UserID);
         ClaimsPrincipal? ValidateToken(string token);
    }
}
