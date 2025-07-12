

namespace LeaveEase.Entity.ViewModel
{
  

    public class PaginationViewModel<User>
    {
        public int PageIndex { get; set; } = 0;
        public int TotalCount { get; set; }
        public int PageCount { get; set; }
        public int TotalPages { get; set; }

        public int PageSize { get; set; }

        public string? RoleName { get; set; }

        public List<UserListViewModel>? Users { get; set; }

        public List<LeaveRequestViewModel>? LeaveRequests { get; set; }
        public List<LeaveApproveViewModel>? LeaveApproved { get; set; }
    }
}





