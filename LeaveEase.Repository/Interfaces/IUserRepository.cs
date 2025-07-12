
using LeaveEase.Entity.Models;
using LeaveEase.Entity.ViewModel;

namespace LeaveEase.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<List<TblUser>> GetUserList(string? role, int? UserId, string? LoginUserRole);
        Task<TblUser?> GetUserDetailUsingId(int userid);
        UserRegisterViewModel IsUserExist(string email, string Phone);
        UserRegisterViewModel EditTimeCheckExist(string email, string Phone, int id);
        bool UpdateRegitration(TblUser user);
        string GetUserNameById(int UserId);

       

    }
}
