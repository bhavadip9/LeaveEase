

namespace LeaveEase.Entity.ViewModel
{
    public class PermissionViewModel
    {

        public int PermissionId { get; set; }
        public string PermissionName { get; set; } = null!;
        public bool IsChecked { get; set; } 
        public bool CanView { get; set; } 
        public bool CanAddEdit { get; set; } 
        public bool CanDelete { get; set; }

        public List<RolePermissionMap>? RolePermissions { get; set; }
    }
}
