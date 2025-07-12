using System;
using System.Collections.Generic;

namespace LeaveEase.Entity.Models;

public partial class TblRole
{
    public int RoleId { get; set; }

    public string? RoleName { get; set; }

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

    public virtual ICollection<TblPermission> TblPermissions { get; set; } = new List<TblPermission>();

    public virtual ICollection<TblUser> TblUsers { get; set; } = new List<TblUser>();

    public virtual TblUser? UpdatedBy { get; set; }
}
