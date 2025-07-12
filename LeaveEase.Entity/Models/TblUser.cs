using System;
using System.Collections.Generic;

namespace LeaveEase.Entity.Models;

public partial class TblUser
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Department { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public string MobileNumber { get; set; } = null!;

    public int Role { get; set; }

    public string? ProfileImg { get; set; }

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

    public int ReportingPerson { get; set; }

    public virtual TblUser? CreatedBy { get; set; }

    public virtual TblUser? DeletedBy { get; set; }

    public virtual ICollection<Holiday> HolidayCreatedBies { get; set; } = new List<Holiday>();

    public virtual ICollection<Holiday> HolidayDeletedBies { get; set; } = new List<Holiday>();

    public virtual ICollection<Holiday> HolidayUpdatedBies { get; set; } = new List<Holiday>();

    public virtual ICollection<TblUser> InverseCreatedBy { get; set; } = new List<TblUser>();

    public virtual ICollection<TblUser> InverseDeletedBy { get; set; } = new List<TblUser>();

    public virtual ICollection<TblUser> InverseReportingPersonNavigation { get; set; } = new List<TblUser>();

    public virtual ICollection<TblUser> InverseUpdatedBy { get; set; } = new List<TblUser>();

    public virtual TblUser ReportingPersonNavigation { get; set; } = null!;

    public virtual TblRole RoleNavigation { get; set; } = null!;

    public virtual ICollection<TblLeaveApprove> TblLeaveApproveApprovedByNavigations { get; set; } = new List<TblLeaveApprove>();

    public virtual ICollection<TblLeaveApprove> TblLeaveApproveCreateBies { get; set; } = new List<TblLeaveApprove>();

    public virtual ICollection<TblLeaveApprove> TblLeaveApproveDeleteBies { get; set; } = new List<TblLeaveApprove>();

    public virtual ICollection<TblLeaveApprove> TblLeaveApproveUpdateBies { get; set; } = new List<TblLeaveApprove>();

    public virtual ICollection<TblLeaveRequest> TblLeaveRequestCreateBies { get; set; } = new List<TblLeaveRequest>();

    public virtual ICollection<TblLeaveRequest> TblLeaveRequestDeleteBies { get; set; } = new List<TblLeaveRequest>();

    public virtual ICollection<TblLeaveRequest> TblLeaveRequestEmployees { get; set; } = new List<TblLeaveRequest>();

    public virtual ICollection<TblLeaveRequest> TblLeaveRequestUpdateBies { get; set; } = new List<TblLeaveRequest>();

    public virtual ICollection<TblPermission> TblPermissionCreatedBies { get; set; } = new List<TblPermission>();

    public virtual ICollection<TblPermission> TblPermissionDeletedBies { get; set; } = new List<TblPermission>();

    public virtual ICollection<TblPermission> TblPermissionUpdatedBies { get; set; } = new List<TblPermission>();

    public virtual ICollection<TblRole> TblRoleCreatedBies { get; set; } = new List<TblRole>();

    public virtual ICollection<TblRole> TblRoleDeletedBies { get; set; } = new List<TblRole>();

    public virtual ICollection<TblRole> TblRoleUpdatedBies { get; set; } = new List<TblRole>();

    public virtual TblUser? UpdatedBy { get; set; }
}
