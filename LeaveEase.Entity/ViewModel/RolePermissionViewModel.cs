

namespace LeaveEase.Entity.ViewModel
{
    public class RolePermissionViewModel
    {

        public List<PermissionViewModel> Permissions { get; set; }

        public string? RoleName { get; set; }
        public List<RoleViewModel> roleViewModels { get; set; }
    }
}
