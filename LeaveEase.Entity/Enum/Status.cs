

using System.Runtime.Serialization;

namespace LeaveEase.Entity.Enum
{
    public enum Status
    {
        [EnumMember(Value = "Pending")]
        Pending,
        [EnumMember(Value = "Approved")]
        Approved,
        [EnumMember(Value = "Rejected")]
        Rejected,
        [EnumMember(Value = "Cancelled")]
        Cancelled
    }
}
