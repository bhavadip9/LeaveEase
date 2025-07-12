

using System.ComponentModel.DataAnnotations;

namespace LeaveEase.Entity.Enum
{
  
      public enum LeaveType
    {
        [Display(Name = "Sick Leave")]
        SickLeave=1,

        [Display(Name = "Marriage Leave")]
        MarriageLeave,

        [Display(Name = "Casual Leave")]
        CasualLeave,

        [Display(Name = "Maternity Leave")]
        MaternityLeave,

        [Display(Name = "Paternity Leave")]
        PaternityLeave
    }


}
