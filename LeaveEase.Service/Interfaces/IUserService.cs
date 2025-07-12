

using LeaveEase.Entity.ViewModel;

namespace LeaveEase.Service.Interfaces
{
    public interface IUserService
    {

        Task<UserRegisterViewModel> GetRegistrationData();

        UserRegisterViewModel IsUserExist(string email, string Phone);

        bool AddRegistration(UserRegisterViewModel userRegisterVM);
        Task<PaginationViewModel<UserListViewModel>> GetUserList(string role,string search, int page, int pageSize,string orderby);

        Task<UserRegisterViewModel> GetUserDetailUsingId(int id);

        UserRegisterViewModel EditUserExise(string Email, string MobilePhone, int userid);

        Task<bool> EditRegistration(UserRegisterViewModel userRegisterVM);

        Task<bool> DeleteUser(int UserID);

        Task<bool> IsAdminUnderEmployee(int UserId);
    }
}
