

using AutoMapper;
using LeaveEase.Entity.Models;
using LeaveEase.Entity.ViewModel;
using LeaveEase.Repository.Interfaces;
using LeaveEase.Service.Interfaces;
using LeaveEase.Service.Utills;


namespace LeaveEase.Service.Implementation
{
    public class LoginService:ILoginService
    {
        private readonly ILoginRepository _loginRepository;

        private readonly IMapper _mapper;

        public LoginService(ILoginRepository loginRepository,IMapper mapper)
        {
            _loginRepository = loginRepository;
            _mapper = mapper;
        }


        /// <summary>
        /// Login time Check User is Exist or not otheewich give register message
        /// </summary>
        /// <param name="loginVM"></param>
        /// <returns></returns>
        public UserRegisterViewModel UserLogin(LoginViewModel loginVM)
        {

            TblUser userData = _loginRepository.GetUserByEmail(loginVM.Email);

            UserRegisterViewModel userRegister = _mapper.Map<UserRegisterViewModel>(userData);

            if (userData != null)
            {
                var VerifyPassword = PasswordUtills.VerifyPassword(loginVM.Password, userData.Password);
                if (VerifyPassword)
                {
                    return userRegister;
                }
                else
                {
                    userRegister.Password = string.Empty;
                    return userRegister;
                }
            }
            return null;

        }


        public ResetPassWordViewModel CheckUserExistByEmail(string email)
        {
            TblUser userData = _loginRepository.GetUserByEmail(email);

            ResetPassWordViewModel  resetPassWordViewModel=new ResetPassWordViewModel();
            if (userData!=null)
            {
                if (!string.IsNullOrEmpty(userData.Email))
                {
                    resetPassWordViewModel.Email = email;
                    return resetPassWordViewModel;
                }
                else
                {
                    resetPassWordViewModel.Email = string.Empty;
                    return resetPassWordViewModel;
                }
            }
            {
                resetPassWordViewModel.Email = string.Empty;
                return resetPassWordViewModel;
            }
            
        }

        public async Task<bool> UpdatePassword(ResetPassWordViewModel resetPassWordViewModel)
        {
            try
            {
                if (resetPassWordViewModel.Email == null)
                {
                    return false;
                }
                TblUser userData = _loginRepository.GetUserByEmail(resetPassWordViewModel.Email);
                bool IsExistPaaWord = PasswordUtills.VerifyPassword(resetPassWordViewModel.NewPassword, userData.Password);
                if (IsExistPaaWord)
                {
                    return false;
                }
               
                userData.Password = PasswordUtills.HashPassword(resetPassWordViewModel.NewPassword);
                _loginRepository.UpdatePassword(userData);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
