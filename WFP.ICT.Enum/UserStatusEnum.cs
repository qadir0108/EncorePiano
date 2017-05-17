using System.ComponentModel;

namespace WFP.ICT.Enum
{
    public enum UserStatusEnum
    {
        [Description("Active")]
        Active = 0,
        [Description("Locked")]
        Locked = 1,
        [Description("Expired")]
        Expired = 2
    }
}
