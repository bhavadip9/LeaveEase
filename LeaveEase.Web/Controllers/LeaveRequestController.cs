using System.Net.NetworkInformation;
using LeaveEase.Entity.Constants;
using LeaveEase.Entity.ViewModel;
using LeaveEase.Service.Helper;
using LeaveEase.Service.Implementation;
using LeaveEase.Service.Interfaces;
using LeaveEase.Service.Middleware;
using Microsoft.AspNetCore.Mvc;



namespace LeaveEase.Web.Controllers
{
    [ServiceFilter(typeof(PermissionFilter))]
    public class LeaveRequestController : Controller
    {

        private readonly ILeaveRequestService _leaveRequestService;

        private readonly ILeaveApprovedService _leaveApprovedService;

        private readonly IHomeService _homeService;

        public LeaveRequestController(ILeaveRequestService leaveRequestService, IHomeService homeService, ILeaveApprovedService leaveApprovedService)
        {
            _leaveRequestService = leaveRequestService;
            _homeService = homeService;
            _leaveApprovedService = leaveApprovedService;
        }

        public IActionResult Index()
        {
            ViewBag.StatusList = EnumHelper.GetStatusSelectList();
            return View();
        }

        public async Task<IActionResult> LeaveApprove(int LeaveId)
        {
            if (LeaveId <= 0)
            {
                return NotFound();
            }
            LeaveApproveViewModel leaveApproveViewModel = await _leaveApprovedService.GetLeaveRequest(LeaveId);
            return View(leaveApproveViewModel);
        }


        [HttpGet]
        public async Task<IActionResult> LeaveRequest(int LeaveId)
        {
            try
            {
                if (LeaveId == 0)
                {
                    LeaveRequestViewModel leaveRequestViewModel = new LeaveRequestViewModel();
                    UserInformationViewModel userInformationViewModel = _homeService.GetUserDetail();
                    leaveRequestViewModel.EmployeeId = userInformationViewModel.UserId;
                    leaveRequestViewModel.Role = userInformationViewModel.Role;
                    leaveRequestViewModel.FromDate = DateOnly.Parse(DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"));
                    leaveRequestViewModel.ToDate = DateOnly.Parse(DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"));
                    leaveRequestViewModel.LeaveId = 0;
                    return View(leaveRequestViewModel);
                }
                else
                {
                    LeaveRequestViewModel leaveRequestViewModel = await _leaveRequestService.GetLeaveRequest(LeaveId);
                    return View(leaveRequestViewModel);
                }
            }
            catch (Exception)
            {
                LeaveRequestViewModel leaveRequestViewModel = new LeaveRequestViewModel();
                return View(leaveRequestViewModel);
            }
        }

       
        [HttpPost]
        public async Task<IActionResult> LeaveRequest(LeaveRequestViewModel leaveRequestViewModel)
        {
            if (leaveRequestViewModel.LeaveId == 0)
            {
                if (!ModelState.IsValid)
                {
                    TempData["error"] = Messages.ErrorMessageLeaveRequestFail;
                    return View(leaveRequestViewModel);
                }
                
                bool IsLeaveExist = await _leaveRequestService.AnyLeaveExist(leaveRequestViewModel.EmployeeId, leaveRequestViewModel.FromDate, leaveRequestViewModel.ToDate);
                if (IsLeaveExist)
                {
                    TempData["error"] = Messages.ErrorMessageLeaveAlreadyExist;
                    return View(leaveRequestViewModel);
                }

                bool IsSuccsee = await _leaveRequestService.AddLeaveRequest(leaveRequestViewModel);
                if (IsSuccsee)
                {
                    TempData["success"] = Messages.SuccessMessageAddleaveRequest;
                    return RedirectToAction("Index");
                }
                TempData["error"] = Messages.ErrorMessageSomthingWrong;
                return View(leaveRequestViewModel);
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    TempData["error"] = Messages.ErrorMessageLeaveRequestFail;
                    return View(leaveRequestViewModel);
                }
                bool exists = await _leaveRequestService.AnyLeaveExist(leaveRequestViewModel.EmployeeId, leaveRequestViewModel.FromDate, leaveRequestViewModel.ToDate, leaveRequestViewModel.LeaveId);
                if (exists)
                {
                    TempData["error"] = Messages.ErrorMessageLeaveAlreadyExist; 
                    return View(leaveRequestViewModel);
                }
                bool IsSuccsee = await _leaveRequestService.EditLeaveRequest(leaveRequestViewModel);
                if (IsSuccsee)
                {
                    TempData["success"] = Messages.SuccessMessageEditleaveRequest;
                    return RedirectToAction("Index");
                }
                TempData["error"] = Messages.ErrorMessageSomthingWrong;
                return View(leaveRequestViewModel);
            }
        }

        [HttpGet]
        public IActionResult DeleteLeaveRequest(int LeaveId)
        {
            if (LeaveId==null)
            {
                return Json(new { success = false, message = Messages.ErrorMessageLeaveIdEmpty });
            }
            LeaveRequestViewModel leaveRequestViewModel = new LeaveRequestViewModel();
            leaveRequestViewModel.LeaveId = LeaveId;
            return PartialView("~/Views/LeaveRequest/_DeleteLeaveRequest.cshtml", leaveRequestViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLeaveRequestPost(int LeaveId)
        {
            if (LeaveId != null)
            {
                bool IsDelete= await _leaveRequestService.DeleteLeaveRequest(LeaveId);
                if (!IsDelete)
                {
                    return Json(new { success = false, message = Messages.ErrorMessageBadRequest }); 
                }
                return Json(new { success = true, message = Messages.SuccessMessageDeleted });
               
            }
            else
            {
                return Json(new { success = false, message = Messages.ErrorMessageSomthingWrong });
               
            }
        }

        public async Task<IActionResult> LeaveReqestList( int page, int pagesize,string Status,string LeaveDates,string SortbyFromdate,string SortbyTodate,string SortbyLeavetype,string AppliedDate)
        {
            try
            {
                PaginationViewModel<LeaveRequestViewModel> LeaveReqestList = await _leaveRequestService.GetLeaveRequestList(page, pagesize, Status, LeaveDates, SortbyFromdate, SortbyTodate, SortbyLeavetype, AppliedDate);
                ViewBag.AppliedDate = AppliedDate;
                ViewBag.SortByFromDate = SortbyFromdate;
                ViewBag.SortByToDate = SortbyTodate;
                ViewBag.SortByLeaveType = SortbyLeavetype;
                return PartialView("~/Views/LeaveRequest/_LeaveRequestListPV.cshtml", LeaveReqestList);
            }
            catch
            {
                PaginationViewModel<LeaveRequestViewModel> LeaveReqestList = new PaginationViewModel<LeaveRequestViewModel>();
                return PartialView("~/Views/LeaveRequest/_LeaveRequestListPV.cshtml", LeaveReqestList);
            }       
        }
    }
}
