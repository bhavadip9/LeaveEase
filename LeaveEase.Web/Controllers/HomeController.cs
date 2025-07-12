
using System.Threading.Tasks;
using LeaveEase.Entity.Constants;
using LeaveEase.Entity.ViewModel;
using LeaveEase.Service.Helper;
using LeaveEase.Service.Interfaces;
using LeaveEase.Service.Middleware;
using Microsoft.AspNetCore.Mvc;
using NLog.Filters;


namespace LeaveEase.Web.Controllers
{
    [ServiceFilter(typeof(PermissionFilter))]
    public class HomeController : Controller
    {
      

        private readonly IHomeService _homeService;

        private readonly IUserService _userService;

        private readonly IImageService _imageService;
       

        public HomeController( IHomeService homeService,IUserService userService,IImageService imageService)
        {  
            _homeService = homeService;
            _userService = userService;
            _imageService = imageService;
        }

     

        /// <summary>
        /// Serves the default view for the Index page.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> that renders the default view for the Index page.</returns>
        public async Task<IActionResult> Index()
        {
            ViewBag.TimeList = EnumHelper.GetTimeFilter();
            DashboardViewModel dashboardViewModel =await _homeService.GetDashBoard();
            return View(dashboardViewModel);
        }
        public async Task<IActionResult> DashboardData(string filter)
        {
            DashboardViewModel dashboardViewModel =await _homeService.GetDashBoard();

            return PartialView("~/Views/Home/_DashboardData.cshtml", dashboardViewModel);
        }


        /// <summary>
        /// Retrieves the user's profile data and displays it in the profile view.
        /// </summary>
        /// <remarks>This method asynchronously fetches the user's profile data using the home service and
        /// passes it to the view.</remarks>
        /// <returns>An <see cref="IActionResult"/> that renders the profile view populated with the user's profile data.</returns>
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            try
            {
                UserRegisterViewModel userRegisterViewModel = await _homeService.GetUserProfileData();
                userRegisterViewModel.AdminList = userRegisterViewModel.AdminList;
                return View(userRegisterViewModel);
            }
            catch
            {
                UserRegisterViewModel userRegisterViewModel = new UserRegisterViewModel();
                TempData["error"] = Messages.ErrorMessageSomthingWrong;
                return View(userRegisterViewModel);
            }  
        }

        [HttpPost]
        public async Task<IActionResult> Profile(UserRegisterViewModel userRegisterVM)
        {
            try
            {
                UserRegisterViewModel IsExist = _userService.EditUserExise(userRegisterVM.Email, userRegisterVM.MobileNumber!, userRegisterVM.UserId ?? 0);
                if (string.IsNullOrEmpty(IsExist.Email) || string.IsNullOrEmpty(IsExist.MobileNumber))
                {
                    UserRegisterViewModel userRegisterViewModel = await _userService.GetUserDetailUsingId(userRegisterVM.UserId ?? 0);
                    userRegisterViewModel = await _userService.GetRegistrationData();
                    if (string.IsNullOrEmpty(IsExist.Email) && string.IsNullOrEmpty(IsExist.MobileNumber))
                    {
                        TempData["error"] = Messages.ErrorMessageAlreadyExist;
                        return View(userRegisterViewModel);
                    }
                    if (string.IsNullOrEmpty(IsExist.Email))
                    {
                        TempData["error"] = Messages.ErrorMessageAlreadyEmailExist;
                        return View(userRegisterViewModel);
                    }
                    if (string.IsNullOrEmpty(IsExist.MobileNumber))
                    {
                        TempData["error"] = Messages.ErrorMessageAlreadyPhoneExist;
                        return View(userRegisterViewModel);
                    }
                }
                if (userRegisterVM.ProfilePicture != null)
                {
                    var path = _imageService.Upload(userRegisterVM.ProfilePicture, userRegisterVM.FirstName);
                    userRegisterVM.ProfileImage = $"{Request.Scheme}://{Request.Host}/{path}";
                }


                bool IsSuccess = await _userService.EditRegistration(userRegisterVM);
                if (IsSuccess)
                {
                    TempData["success"] = Messages.SuccessMessageUpdated;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    UserRegisterViewModel userRegister =await _userService.GetRegistrationData();
                    userRegisterVM.BirthDate = DateOnly.FromDateTime(DateTime.Now);
                    TempData["error"] = Messages.ErrorMessageSomthingWrong;
                    return View(userRegister);
                }
            }
            catch
            {
                UserRegisterViewModel userRegister = new UserRegisterViewModel();
                TempData["error"] = Messages.ErrorMessageSomthingWrong;
                return View(userRegister);
            }  
        }


       
        public async Task<IActionResult> ProfileData()
        {
            try
            {
                UserRegisterViewModel userRegisterViewModel = await _homeService.GetUserProfileData();
                return Json(userRegisterViewModel);
            }
            catch
            {
                UserRegisterViewModel userRegister = new UserRegisterViewModel();
                TempData["error"] = Messages.ErrorMessageSomthingWrong;
                return Json(userRegister);
            }         
        }




        /// <summary>
        /// Displays the Change Password view, pre-populated with the user's email address.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> that renders the Change Password view.</returns>
       
        [HttpGet]
        public IActionResult ChangePassword()
        {
            try
            {
                ChangePasswordViewModel changePasswordViewModel = new ChangePasswordViewModel();
                changePasswordViewModel.Email = _homeService.GetUserDetail().Email!;
                return View(changePasswordViewModel);
            }
            catch
            {
                ChangePasswordViewModel changePasswordViewModel = new ChangePasswordViewModel();
                TempData["error"] = Messages.ErrorMessageSomthingWrong;
                return View(changePasswordViewModel);
            }           
        }

        public IActionResult ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["error"] = Messages.ErrorMessageModelInvalid;
                    return View(changePasswordViewModel);
                }

                ChangePasswordViewModel changePasswordView = _homeService.ChangePassword(changePasswordViewModel);
                if (string.IsNullOrEmpty(changePasswordView.OldPassword))
                {
                    TempData["error"] = Messages.ErrorMessageOldPassWordIncorrect;      
                    return View(changePasswordView);
                }
                if (string.IsNullOrEmpty(changePasswordView.NewPassword))
                {
                    TempData["error"] = Messages.ErrorMessagePassWordReset;
                    return View(changePasswordViewModel);
                }
                if (string.IsNullOrEmpty(changePasswordView.ConfirmPassword))
                {
                    TempData["error"] = Messages.ErrorMessageSomthingWrong;
                    return View(changePasswordViewModel);
                }

                TempData["success"] = Messages.SuccessMessagePasswordUpdated;
                return RedirectToAction("Logout", "Login");
            }
            catch
            {
                TempData["success"] = Messages.ErrorMessageSomthingWrong;
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult LogoutRequest()
        {
            try
            {
                return PartialView("~/Views/Home/_LogoutPV.cshtml");
            }
            catch
            {
                return Json(new { success = false, message = Messages.ErrorMessageSomthingWrong });
            }         
        }



        /// <summary>
        /// Returns the error view for handling application errors.
        /// </summary>
        /// <remarks>This action is typically invoked when an unhandled error occurs in the application.
        /// It is decorated with attributes to disable response caching and enforce custom authorization.</remarks>
        /// <returns>An <see cref="IActionResult"/> that renders the error view.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}

