using System.Security.Cryptography;
using LeaveEase.Entity.ViewModel;
using LeaveEase.Entity.Constants;
using LeaveEase.Service.Interfaces;
using LeaveEase.Service.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Threading.Tasks;


namespace LeaveEase.Web.Controllers
{
    [ServiceFilter(typeof(PermissionFilter))]
    public class UserController : Controller
    {
        private IUserService _userService;

        private IHomeService _homeService;

        private readonly IImageService _imageService;

        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        private readonly IEmailService _emailService;

        public UserController(IUserService userService, IHomeService homeService, IImageService imageService, IRazorViewEngine viewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider, IEmailService emailService)
        {
            _userService = userService;
            _homeService = homeService;
            _imageService = imageService;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _emailService = emailService;
           
        }

      
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// If User Id then get user data
        /// </summary>
        /// <returns></returns>
      
        [HttpGet]
        public async Task<IActionResult> Registration(int Id)
        {
            if (Id == 0)
            {
                UserRegisterViewModel userRegisterViewModel = await _userService.GetRegistrationData();
                userRegisterViewModel.UserId = Id;
                userRegisterViewModel.BirthDate =null;
                return View(userRegisterViewModel);
            }
            else
            {
                UserRegisterViewModel userRegisterViewModel = await _userService.GetUserDetailUsingId(Id);
                UserRegisterViewModel userRegisterVM = await _userService.GetRegistrationData();
                userRegisterViewModel.RoleVM = userRegisterVM.RoleVM;
                userRegisterViewModel.AdminList = userRegisterVM.AdminList;
                return View(userRegisterViewModel);
            }
        }



        /// <summary>
        /// Submit Registration From
        /// </summary>
        /// <param name="userRegisterVM"></param>
        /// <returns></returns>

        [HttpPost]
        public async Task<IActionResult> Registration(UserRegisterViewModel userRegisterVM)
        {
            if (userRegisterVM.UserId == 0)
            {
                //Check Model Validation
                if (!ModelState.IsValid)
                {
                    UserRegisterViewModel userRegister = await _userService.GetRegistrationData();
                    userRegister.UserId = 0;
                    TempData["error"] = Messages.ErrorMessageModelInvalid;
                    return View(userRegister);
                }


                //Check Old Email Is Available
                UserRegisterViewModel IsExist = _userService.IsUserExist(userRegisterVM.Email!,userRegisterVM.MobileNumber!);
                if (string.IsNullOrEmpty(IsExist.Email) || string.IsNullOrEmpty(IsExist.MobileNumber))
                {
                    UserRegisterViewModel userRegister = await _userService.GetRegistrationData();
                    userRegister.UserId = 0;
                    if (string.IsNullOrEmpty(IsExist.Email) && string.IsNullOrEmpty(IsExist.MobileNumber))
                    {                              
                        TempData["error"] = Messages.ErrorMessageAlreadyExist;
                        return View(userRegister);
                    }
                    if(string.IsNullOrEmpty(IsExist.Email))
                    {                                            
                        TempData["error"] = Messages.ErrorMessageAlreadyEmailExist;
                        return View(userRegister);
                    }
                    if(string.IsNullOrEmpty(IsExist.MobileNumber))
                    {                  
                        TempData["error"] = Messages.ErrorMessageAlreadyPhoneExist;
                        return View(userRegister);
                    }

                }

                //If Image is Upload then store in local storage otherwise store one demo image
                if (userRegisterVM.ProfilePicture != null)
                {
                    var path = _imageService.Upload(userRegisterVM.ProfilePicture, userRegisterVM.FirstName);
                    userRegisterVM.ProfileImage = $"{Request.Scheme}://{Request.Host}/{path}";
                }
                else
                {
                    userRegisterVM.ProfileImage = $"{Request.Scheme}://{Request.Host}/Img/male-icon.jpg";
                }
                //Add UseRegistration
                bool IsSuccess = _userService.AddRegistration(userRegisterVM);


                if (IsSuccess)
                {
                    string resetToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
                    string callbackUrl = Url.Action("ResetPassWord", "Login", new { token = resetToken, Email = userRegisterVM.Email }, Request.Scheme)!;
                    userRegisterVM.ResetLink = callbackUrl;

                    string emailHtml = await RenderViewToStringAsync("WelcomeEmail", userRegisterVM);
                    _emailService.SendEmail(userRegisterVM.Email, "Welcome to LeaveEase", emailHtml);
                    TempData["success"] = Messages.SuccessMessageRegistration;
                    //return Json(new { success = true, message = Messages.SuccessMessageRegistration });
                     return RedirectToAction("Index", "Home");
                }
                else
                {
                    UserRegisterViewModel userRegister = await _userService.GetRegistrationData();
                    userRegister.UserId = 0;
                    TempData["error"] = Messages.ErrorMessageSomthingWrong;
                    return View(userRegister);
                }
            }
            else
            {
                // Check Old Email Is Available
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
               

                bool IsSuccess =await _userService.EditRegistration(userRegisterVM);
                if (IsSuccess)
                {
                   
                    TempData["success"] = Messages.SuccessMessageUpdated;
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    UserRegisterViewModel userRegister = await _userService.GetRegistrationData();
                    TempData["error"] = Messages.ErrorMessageSomthingWrong;
                    return View(userRegister);
                }

            }

        }


        //[HttpPost]
        //public async Task<IActionResult> Registration(UserRegisterViewModel userRegisterVM)
        //{
        //    if (userRegisterVM.UserId == 0)
        //    {
        //        //Check Model Validation
        //        if (!ModelState.IsValid)
        //        {
        //            UserRegisterViewModel userRegister = await _userService.GetRegistrationData();
        //            userRegister.UserId = 0;
        //            TempData["error"] = Messages.ErrorMessageModelInvalid;
        //            return View(userRegister);
        //        }


        //        //Check Old Email Is Available
        //        UserRegisterViewModel IsExist = _userService.IsUserExist(userRegisterVM.Email!,userRegisterVM.MobileNumber!);
        //        if (string.IsNullOrEmpty(IsExist.Email) || string.IsNullOrEmpty(IsExist.MobileNumber))
        //        {
        //            UserRegisterViewModel userRegister = await _userService.GetRegistrationData();
        //            userRegister.UserId = 0;
        //            if (string.IsNullOrEmpty(IsExist.Email) && string.IsNullOrEmpty(IsExist.MobileNumber))
        //            {                              
        //                TempData["error"] = Messages.ErrorMessageAlreadyExist;
        //                return View(userRegister);
        //            }
        //            if(string.IsNullOrEmpty(IsExist.Email))
        //            {                                            
        //                TempData["error"] = Messages.ErrorMessageAlreadyEmailExist;
        //                return View(userRegister);
        //            }
        //            if(string.IsNullOrEmpty(IsExist.MobileNumber))
        //            {                  
        //                TempData["error"] = Messages.ErrorMessageAlreadyPhoneExist;
        //                return View(userRegister);
        //            }

        //        }

        //        //If Image is Upload then store in local storage otherwise store one demo image
        //        if (userRegisterVM.ProfilePicture != null)
        //        {
        //            var path = _imageService.Upload(userRegisterVM.ProfilePicture, userRegisterVM.FirstName);
        //            userRegisterVM.ProfileImage = $"{Request.Scheme}://{Request.Host}/{path}";
        //        }
        //        else
        //        {
        //            userRegisterVM.ProfileImage = $"{Request.Scheme}://{Request.Host}/Img/male-icon.jpg";
        //        }
        //        //Add UseRegistration
        //        bool IsSuccess = _userService.AddRegistration(userRegisterVM);


        //        if (IsSuccess)
        //        {
        //            string resetToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        //            string callbackUrl = Url.Action("ResetPassWord", "Login", new { token = resetToken, Email = userRegisterVM.Email }, Request.Scheme)!;
        //            userRegisterVM.ResetLink = callbackUrl;

        //            string emailHtml = await RenderViewToStringAsync("WelcomeEmail", userRegisterVM);
        //            _emailService.SendEmail(userRegisterVM.Email, "Welcome to LeaveEase", emailHtml);
        //            TempData["success"] = Messages.SuccessMessageRegistration;
        //            return RedirectToAction("Index", "Home");
        //        }
        //        else
        //        {
        //            UserRegisterViewModel userRegister = await _userService.GetRegistrationData();
        //            userRegister.UserId = 0;
        //            TempData["error"] = Messages.ErrorMessageSomthingWrong;
        //            return View(userRegister);
        //        }
        //    }
        //    else
        //    {
        //        // Check Old Email Is Available
        //        UserRegisterViewModel IsExist = _userService.EditUserExise(userRegisterVM.Email, userRegisterVM.MobileNumber!, userRegisterVM.UserId ?? 0);
        //        if (string.IsNullOrEmpty(IsExist.Email) || string.IsNullOrEmpty(IsExist.MobileNumber))
        //        {
        //            UserRegisterViewModel userRegisterViewModel = await _userService.GetUserDetailUsingId(userRegisterVM.UserId ?? 0);
        //            userRegisterViewModel = await _userService.GetRegistrationData();
        //            if (string.IsNullOrEmpty(IsExist.Email) && string.IsNullOrEmpty(IsExist.MobileNumber))
        //            {
        //                TempData["error"] = Messages.ErrorMessageAlreadyExist;
        //                return View(userRegisterViewModel);
        //            }
        //            if (string.IsNullOrEmpty(IsExist.Email))
        //            {
        //                TempData["error"] = Messages.ErrorMessageAlreadyEmailExist;
        //                return View(userRegisterViewModel);
        //            }
        //            if (string.IsNullOrEmpty(IsExist.MobileNumber))
        //            {
        //                TempData["error"] = Messages.ErrorMessageAlreadyPhoneExist;
        //                return View(userRegisterViewModel);
        //            } 
        //        }
               

        //        if (userRegisterVM.ProfilePicture != null)
        //        {
        //            var path = _imageService.Upload(userRegisterVM.ProfilePicture, userRegisterVM.FirstName);
        //            userRegisterVM.ProfileImage = $"{Request.Scheme}://{Request.Host}/{path}";
        //        }
               

        //        bool IsSuccess =await _userService.EditRegistration(userRegisterVM);
        //        if (IsSuccess)
        //        {
                   
        //            TempData["success"] = Messages.SuccessMessageUpdated;
        //            return RedirectToAction("Index", "User");
        //        }
        //        else
        //        {
        //            UserRegisterViewModel userRegister = await _userService.GetRegistrationData();
        //            TempData["error"] = Messages.ErrorMessageSomthingWrong;
        //            return View(userRegister);
        //        }

        //    }

        //}

        /// <summary>
        ///  WelcomeMail Method
        /// </summary>
        /// <param name="userRegisterVM"></param>
        /// <returns></returns>
        public IActionResult WelcomeEmail(UserRegisterViewModel userRegisterVM)
        {
            return View(userRegisterVM);
        }


        /// <summary>
        /// View Convert in string and pass in mail method
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private async Task<string> RenderViewToStringAsync(string viewName, object model)
        {
            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };

            var routeData = new RouteData();
            routeData.Values["controller"] = "User";

            var actionContext = new ActionContext(httpContext, routeData, new ActionDescriptor());

            await using var sw = new StringWriter();

            var viewResult = _viewEngine.FindView(actionContext, viewName, false);

            if (!viewResult.Success)
            {
                var searchedLocations = string.Join(Environment.NewLine, viewResult.SearchedLocations);
                throw new InvalidOperationException($"View '{viewName}' not found. Searched locations:\n{searchedLocations}");
            }

            var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };

            var viewContext = new ViewContext(actionContext, viewResult.View, viewDictionary,
                new TempDataDictionary(actionContext.HttpContext, _tempDataProvider), sw, new HtmlHelperOptions());

            await viewResult.View.RenderAsync(viewContext);
            return sw.ToString();
        }


        /// <summary>
        /// Get User List
        /// </summary>
        /// <param name="Role"></param>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
      
        public async Task<IActionResult> GetUserList(string Role,string search, int page, int pageSize, string orderby)
         {
            try
            {
                PaginationViewModel<UserListViewModel> userListData = await _userService.GetUserList(Role, search, page, pageSize, orderby);
                ViewBag.SortBy = orderby;
                return PartialView("~/Views/User/_UserListPV.cshtml", userListData);
            }
            catch
            {
                PaginationViewModel<UserListViewModel> userListData = new PaginationViewModel<UserListViewModel>();
                return PartialView("~/Views/User/_UserListPV.cshtml", userListData);
            }          
        }



        /// <summary>
        /// Renders a partial view for deleting a user.
        /// </summary>
        /// <remarks>If an error occurs during the operation, the partial view is still rendered, but the
        /// <see cref="UserRegisterViewModel"/> will not contain a valid user ID.</remarks>
        /// <param name="UserId">The unique identifier of the user to be deleted. Must be a valid user ID.</param>
        /// <returns>An <see cref="IActionResult"/> that renders the partial view <c>_DeleteUser.cshtml</c>. The view is
        /// populated with a <see cref="UserRegisterViewModel"/> containing the specified <paramref name="UserId"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> DeleteUser(int UserId) 
        {
            try
            {
                UserRegisterViewModel userRegisterViewModel = new UserRegisterViewModel();
                userRegisterViewModel.UserId = UserId;
                bool isAdminUnderEmployee = await _userService.IsAdminUnderEmployee(UserId);
                if (isAdminUnderEmployee)
                {
                    return Json(new { success = false, message = Messages.ErrorMessageAssignAdminToOther });
                   
                }

                return PartialView("~/Views/User/_DeleteUser.cshtml", userRegisterViewModel);
            }
            catch
            {
                UserRegisterViewModel userRegisterViewModel = new UserRegisterViewModel();
                return PartialView("~/Views/User/_DeleteUser.cshtml", userRegisterViewModel);
            }
        }

        /// <summary>
        /// Deletes a user based on the specified user ID.
        /// </summary>
        /// <remarks>This method attempts to delete a user by invoking the <c>DeleteUser</c> method of the
        /// user service. If the deletion is successful, a success message is returned in the JSON response. If the
        /// deletion fails or the user ID is invalid, an error message is returned or the user is redirected.</remarks>
        /// <param name="UserId">The unique identifier of the user to be deleted. Must be a valid, non-null integer.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.  Returns a JSON response with a
        /// success flag and a message if the operation completes successfully or fails. Redirects to the "Index" action
        /// of the "User" controller if the user ID is invalid or an error occurs.</returns>

        [HttpPost]
        public async Task<IActionResult> DeleteUserPost(int UserId)
        {
            try
            {
                if (UserId != null)
                {
                    bool IsSuccess = await _userService.DeleteUser(UserId);
                    if (IsSuccess)
                    {
                        return Json(new { success = true, message = Messages.SuccessMessageDeleted });
                    }
                    else
                    {
                        return Json(new { success = false, message = Messages.ErrorMessageSomthingWrong });

                    }
                }
                else
                {
                    TempData["error"] = Messages.ErrorMessageNotDeleteUser;
                    return RedirectToAction("Index", "User");
                }
            }
            catch
            {
                TempData["error"] = Messages.ErrorMessageNotDeleteUser;
                return RedirectToAction("Index", "User");
            }
            
        }
    }
}
