
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using LeaveEase.Entity.Constants;
using LeaveEase.Entity.ViewModel;
using LeaveEase.Service.Helper;
using LeaveEase.Service.Interfaces;
using LeaveEase.Service.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace LeaveEase.Web.Controllers
{
    [ServiceFilter(typeof(PermissionFilter))]
    public class LeaveApproveController : Controller
    {

        private readonly ILeaveApprovedService _leaveApprovedService;

        public LeaveApproveController(ILeaveApprovedService leaveApprovedService)
        {
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
            LeaveApproveViewModel leaveApproveViewModel =await _leaveApprovedService.GetLeaveRequest(LeaveId);
            return View(leaveApproveViewModel);
        }

        public async Task<IActionResult> LeaveApprovePost(LeaveApproveViewModel leaveApproveViewModel)
        {
            bool IsSuccess=await _leaveApprovedService.UpdateLeaveRequest(leaveApproveViewModel);
            if (IsSuccess)
            {
                TempData["success"] = Messages.SuccessMessageEditleaveRequest; 
            }
            else
            {
                TempData["error"] = Messages.ErrorMessageLeaveNotUpdate; 
            }
            return RedirectToAction("Index");
        }
      

        public async Task<IActionResult> LeaveApproveList(string search,int page, int pagesize, string Status, string LeaveDates,string SortbyName,string SortbyFromdate,string SortbyTodate,string SortbyLeavetype)
        {
            try
            {
                PaginationViewModel<LeaveApproveViewModel> LeaveReqestList = await _leaveApprovedService.GetAllLeaveRequestList(search, page, pagesize, Status, LeaveDates, SortbyName, SortbyFromdate, SortbyTodate, SortbyLeavetype);
                ViewBag.SortByName = SortbyName;
                ViewBag.SortByFromDate = SortbyFromdate;
                ViewBag.SortByToDate = SortbyTodate;
                ViewBag.SortByLeaveType = SortbyLeavetype;
                return PartialView("~/Views/LeaveApprove/_LeaveApprovedListPV.cshtml", LeaveReqestList);
            }
            catch(Exception Ex)
            {
                PaginationViewModel<LeaveApproveViewModel> LeaveReqestList = new PaginationViewModel<LeaveApproveViewModel>();
                TempData["error"] = Messages.ErrorMessageSomthingWrong;
                return PartialView("~/Views/LeaveApprove/_LeaveApprovedListPV.cshtml", LeaveReqestList);
            }           
        }

        public IActionResult LeaveCancel(int LeaveId)
        {
            try
            {
                LeaveApproveViewModel leaveApproveViewModel = new LeaveApproveViewModel();
                leaveApproveViewModel.LeaveId = LeaveId;

                return PartialView("~/Views/LeaveApprove/_CancelLeave.cshtml", leaveApproveViewModel);
            }
            catch
            {
                return Json(new { success = false, message = Messages.ErrorMessageSomthingWrong });
            }         
        }
        public async Task<IActionResult> LeaveCancelPost(int LeaveId)
        {
            try
            {
                bool IsSuccess= await _leaveApprovedService.CancelLeaveRequest(LeaveId);

                if (IsSuccess)
                {
                    return Json(new { success = true, message = Messages.SuccessMessageCancelRequest });
                    
                }
                else
                {
                    return Json(new { success = false, message = Messages.ErrorMessageSomthingWrong });
                }  
            }
            catch
            {
                return Json(new { success = false, message = Messages.ErrorMessageSomthingWrong });
            }
        }
    }
}
