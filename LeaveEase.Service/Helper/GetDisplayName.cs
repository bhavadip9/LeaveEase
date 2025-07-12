
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace LeaveEase.Service.Helper
{
    public class DisplayNameHelper
    {
        public static string GetDisplayName(Enum enumValue)
        {
            var member = enumValue.GetType()
                .GetMember(enumValue.ToString())
                .FirstOrDefault();

            var displayAttribute = member?
                .GetCustomAttribute<DisplayAttribute>();

            return displayAttribute?.Name ?? enumValue.ToString();
        }
    }
}
