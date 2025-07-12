
using LeaveEase.Entity.ViewModel;

namespace LeaveEase.Service.Interfaces

{
    public interface IHomeService
    {
        Task<UserRegisterViewModel> GetUserProfileData();
        UserInformationViewModel GetUserDetail();
        Task<DashboardViewModel> GetDashBoard();
        string GetRoleName(int Roleid);
         ChangePasswordViewModel ChangePassword(ChangePasswordViewModel changePasswordViewModel);
    }
}
