using LeaveEase.Entity.Enum;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Net.Mime.MediaTypeNames;

namespace LeaveEase.Service.Helper
{
    public class EnumHelper
    {
        public static List<SelectListItem> GetStatusSelectList()
        {
            return Enum.GetValues(typeof(Status))
                       .Cast<Status>()
                       .Select(e => new SelectListItem
                       {
                           Text = e.ToString(),  
                           Value = e.ToString()   
                       })
                       .ToList();
        }
        public static List<SelectListItem> GetTimeFilter()
        {
            return Enum.GetValues(typeof(TimeFilter))
                       .Cast<TimeFilter>()
                       .Select(e => new SelectListItem
                       {
                           Text  = DisplayNameHelper.GetDisplayName(e),  
                           Value = e.ToString()   
                       })
                       .ToList();
        }
    }
}


