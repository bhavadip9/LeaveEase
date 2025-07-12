
using LeaveEase.Entity.ViewModel;

namespace LeaveEase.Service.Interfaces
{
    public interface ILoginService
    {
        UserRegisterViewModel UserLogin(LoginViewModel loginVM);
        ResetPassWordViewModel CheckUserExistByEmail(string email);

        Task<bool> UpdatePassword(ResetPassWordViewModel resetPassWordViewModel);
    }
}
