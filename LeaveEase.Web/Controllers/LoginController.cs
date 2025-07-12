using System.Data;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using LeaveEase.Entity.Constants;
using LeaveEase.Entity.ViewModel;
using LeaveEase.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json.Linq;

namespace LeaveEase.Web.Controllers
{
    public class LoginController : Controller
    {

        private readonly ILoginService _loginService;

        private readonly IJwtService _jwtService;

        private readonly ICookieService _cookieService;

        private readonly ISessionService _sessionService;

        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        private readonly IHomeService _homeService;

        private readonly IEmailService _emailService;

        public LoginController(ILoginService loginService, IJwtService jwtService, ICookieService cookieService, ISessionService sessionService, IEmailService emailService, IRazorViewEngine viewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider, IHomeService homeService)
        {
            _loginService = loginService;
            _jwtService = jwtService;
            _cookieService = cookieService;
            _sessionService = sessionService;
            _emailService = emailService;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _homeService = homeService;
        }


        /// <summary>
        /// First method in Website start
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {

            //Check User Save cookie or not if save then Go to home page otherwise Show Login page  
            if (!string.IsNullOrEmpty(Request.Cookies["UserData"]))
            {
                UserRegisterViewModel UserData = _sessionService.GetUser(HttpContext)!;
                _sessionService.SetUser(HttpContext, UserData!);

                string RoleName = _homeService.GetRoleName(UserData!.Role);

                var token = _jwtService.GenerateJwtToken(UserData.FirstName, UserData.Email, RoleName, UserData.UserId??0);
                _cookieService.SaveJWTToken(Response, token);
              
                return RedirectToAction("Index", "Home");
            }
            else
            {
                _sessionService.ClearSession(HttpContext);
                _cookieService.ClearCookies(HttpContext);
            }

            return View();
        }

        /// <summary>
        /// Login Post method
        /// </summary>
        /// <param name="loginVM"></param>
        /// <returns></returns>
        public IActionResult Login(LoginViewModel loginViewModel)
        
        {
            if (ModelState.IsValid)
            {

                UserRegisterViewModel userData = _loginService.UserLogin(loginViewModel);
                if (userData != null)
                {
                    if (string.IsNullOrEmpty(userData.Email))
                    {
                        TempData["error"] = Messages.ErrorMessageEmail;
                        return View(loginViewModel);
                    }

                    if (string.IsNullOrEmpty(userData.Password))
                    {
                        TempData["error"] = Messages.ErrorMessagePassword;
                        return View(loginViewModel);
                    }
                }
                else
                {
                    TempData["error"] = Messages.ErrorMessageRegister;
                    return View(loginViewModel);
                }

                string RoleName = _homeService.GetRoleName(userData.Role);
                string token = _jwtService.GenerateJwtToken(userData.FirstName, userData.Email, RoleName, userData.UserId ?? 0);

                _cookieService.SaveJWTToken(Response, token);

                if (loginViewModel.RememberMe)
                {
                    _cookieService.SaveRememberMe(Response, userData);
                }

                // Store User details in Session
                _sessionService.SetUser(HttpContext, userData);
              
                 return RedirectToAction("Index", "Home");
            }
            else
            {               
                return View(loginViewModel);
            }
        }


        /// <summary>
        /// Logout Method
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            //     Clear out all the Cookie data
            _cookieService.ClearCookies(HttpContext);

            //     Clear out all the Session data
            _sessionService.ClearSession(HttpContext);
            return RedirectToAction("Login");
        }

        /// <summary>
        /// Forget Get Method
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ForgetPassword(string? Email)
        {
            LoginViewModel resetPassWordViewModel = new LoginViewModel();
            resetPassWordViewModel.Email = Email!;
            return View(resetPassWordViewModel);
        }


        /// <summary>
        /// Sends a password reset link to the specified email address if the user exists.
        /// </summary>
        /// <remarks>This method generates a unique reset token and constructs a callback URL for
        /// resetting the password.  The reset link is sent to the provided email address using the configured email
        /// service.  If the user does not exist, an error message is displayed.</remarks>
        /// <param name="email">The email address of the user requesting a password reset. Cannot be null or empty.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.  If the user does not exist, returns
        /// a view with an error message.  Otherwise, redirects to the login page after successfully sending the reset
        /// link.</returns>
       
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(LoginViewModel loginViewModel)
        {
            ModelState.Remove("Password");
            if (ModelState.IsValid)
            {
                ResetPassWordViewModel resetPassWordViewModel = _loginService.CheckUserExistByEmail(loginViewModel.Email);
                if (string.IsNullOrEmpty(resetPassWordViewModel.Email))
                {
                    TempData["error"] = Messages.ErrorMessageRegister;
                    return View(loginViewModel);
                }
                else
                {
                    string resetToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
                    string callbackUrl = Url.Action("ResetPassWord", "Login", new { token = resetToken, Email = loginViewModel.Email }, Request.Scheme)!;

                    resetPassWordViewModel.Email = loginViewModel.Email;
                    resetPassWordViewModel.UrlLink = callbackUrl;
                    resetPassWordViewModel.NewPassword = string.Empty;
                    resetPassWordViewModel.ConfirmPassword = string.Empty;

                    string htmlPage = await RenderViewToStringAsync("ResetEmail", resetPassWordViewModel);
                    _emailService.SendEmail(loginViewModel.Email, "Reset Link", htmlPage);
                    TempData["success"] = Messages.SuccessMessageSentMail;
                    return RedirectToAction("Login", "login");
                }
            }
            else
            {
               TempData["error"] = Messages.ErrorMessageBadRequest;
                return View(loginViewModel);
            }
          }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resetPassWordViewModel"></param>
        /// <returns></returns>
        public IActionResult ResetEmail(ResetPassWordViewModel resetPassWordViewModel)
        {
            return View(resetPassWordViewModel);
        }

        /// <summary>
        /// Renders the specified Razor view to a string using the provided model.
        /// </summary>
        /// <remarks>This method uses the configured Razor view engine and service provider to locate and
        /// render the view.  Ensure that the view name is correct and that the necessary services are registered in the
        /// application's dependency injection container.</remarks>
        /// <param name="viewName">The name of the Razor view to render. This must match the view's file name without the file extension.</param>
        /// <param name="model">The model to pass to the view. This can be <see langword="null"/> if the view does not require a model.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous operation. The task result contains the
        /// rendered view as a string.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the specified view cannot be found. The exception message includes the locations that were
        /// searched.</exception>
        private async Task<string> RenderViewToStringAsync(string viewName, object model)
        {
            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };

            var routeData = new RouteData();
            routeData.Values["controller"] = "Login";

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
        /// Displays the password reset view for the specified user.
        /// </summary>
        /// <remarks>This method validates the provided password reset token and email address, and
        /// prepares a  <see cref="ResetPassWordViewModel"/> to be passed to the view. If the token is null, an error 
        /// message is added to <see cref="TempData"/> and a bad request response is returned.</remarks>
        /// <param name="token">The password reset token associated with the user. This value cannot be null.</param>
        /// <param name="email">The email address of the user requesting the password reset.</param>
        /// <returns>An <see cref="IActionResult"/> that renders the password reset view if the token is valid;  otherwise, a
        /// <see cref="BadRequestResult"/> indicating an invalid password reset request.</returns>
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
          
            if (token == null)
            {
                TempData["error"] = Messages.ErrorMessageResetPassword;
                return BadRequest(Messages.ErrorMessageResetPassword) ;
            }
            ResetPassWordViewModel resetPassWordViewModel = new ResetPassWordViewModel();
            resetPassWordViewModel.Token = token;
            resetPassWordViewModel.Email = email;
            
            return View(resetPassWordViewModel);

        }


        /// <summary>
        /// Resets the user's password based on the provided model.
        /// </summary>
        /// <param name="model">The <see cref="ResetPassWordViewModel"/> containing the user's password reset details.</param>
        /// <returns>An <see cref="IActionResult"/> that redirects to the login page upon successful password reset,  or returns
        /// the current view with the model if validation fails.</returns>
        /// <exception cref="Exception">Thrown if an unexpected error occurs during the password reset process.</exception>
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPassWordViewModel resetPassWordViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                  bool IsExicute=  await _loginService.UpdatePassword(resetPassWordViewModel);
                    if (!IsExicute)
                    {
                        TempData["error"] = Messages.ErrorMessagePassWordReset;   
                        return RedirectToAction("ResetPassword", "Login", new { token = resetPassWordViewModel.Token, email = resetPassWordViewModel.Email });
                    }
                    else
                    {
                        TempData["success"] = Messages.SuccessMessagePassWordReset;
                        return RedirectToAction("Login", "login");
                    }
                   
                }
                return View(resetPassWordViewModel);
            }
            catch
            {
                TempData["error"] = Messages.ErrorMessageUnhandled;
                throw new Exception();
            }
        }
    }
}
