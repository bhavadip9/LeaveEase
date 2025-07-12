using System;
using System.Collections.Generic;

namespace LeaveEase.Entity.Models;

public partial class TblPermission
{
    public int PermissionId { get; set; }

    public string PermissionName { get; set; } = null!;

    public bool CanView { get; set; }

    public bool CanAddEdit { get; set; }

    public bool CanDelete { get; set; }

    public int Role { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedById { get; set; }

    public string? CreatedByName { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedById { get; set; }

    public string? UpdatedByName { get; set; }

    public DateTime? DeletedDate { get; set; }

    public int? DeletedById { get; set; }

    public string? DeletedByName { get; set; }

    public bool IsActive { get; set; }

    public virtual TblUser? CreatedBy { get; set; }

    public virtual TblUser? DeletedBy { get; set; }

    public virtual TblRole RoleNavigation { get; set; } = null!;

    public virtual TblUser? UpdatedBy { get; set; }
}
