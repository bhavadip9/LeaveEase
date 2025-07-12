using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveEase.Entity.Enum
{
    public enum TimeFilter
    {
        [Display(Name = "Today")]
        Today,
        [Display(Name = "This Month")]
        ThisMonth,
        [Display(Name = "This Year")]
        ThisYear,
       
    }
}
