

namespace LeaveEase.Entity.ViewModel
{
    public class RolePermissionMap
    {
        public int PermissionId { get; set; }
        public int RoleId { get; set; }
        public bool CanView { get; set; }
        public bool CanAddEdit { get; set; }
        public bool CanDelete { get; set; }
    }
}
