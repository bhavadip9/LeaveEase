using System;
using System.Collections.Generic;

namespace LeaveEase.Entity.Models;

public partial class TblLeaveRequest
{
    public int LeaveId { get; set; }

    public int EmployeeId { get; set; }

    public DateOnly FromDate { get; set; }

    public DateOnly ToDate { get; set; }

    public string? Reason { get; set; }

    public DateTime AppliedDate { get; set; }

    public DateTime CreateDate { get; set; }

    public string Status { get; set; } = null!;

    public int? CreateById { get; set; }

    public string? CreateByName { get; set; }

    public DateTime? UpdateDate { get; set; }

    public int? UpdateById { get; set; }

    public string? UpdateByName { get; set; }

    public DateTime? DeleteDate { get; set; }

    public int? DeleteById { get; set; }

    public string? DeleteByName { get; set; }

    public bool IsActive { get; set; }

    public string LeaveType { get; set; } = null!;

    public int? ActualLeave { get; set; }

    public int? TotalLeave { get; set; }

    public virtual TblUser? CreateBy { get; set; }

    public virtual TblUser? DeleteBy { get; set; }

    public virtual TblUser Employee { get; set; } = null!;

    public virtual ICollection<TblLeaveApprove> TblLeaveApproves { get; set; } = new List<TblLeaveApprove>();

    public virtual TblUser? UpdateBy { get; set; }
}
