using System;
using System.Collections.Generic;

namespace LeaveEase.Entity.Models;

public partial class TblLeaveApprove
{
    public int LeaveApproveId { get; set; }

    public int LeaveRequestId { get; set; }

    public int ApprovedBy { get; set; }

    public string? Remark { get; set; }

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

    public virtual TblUser ApprovedByNavigation { get; set; } = null!;

    public virtual TblUser? CreateBy { get; set; }

    public virtual TblUser? DeleteBy { get; set; }

    public virtual TblLeaveRequest LeaveRequest { get; set; } = null!;

    public virtual TblUser? UpdateBy { get; set; }
}
