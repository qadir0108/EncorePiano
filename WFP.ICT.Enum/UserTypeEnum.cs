using System.ComponentModel;

namespace WFP.ICT.Enum
{
    public enum UserTypeEnum
    {
        [Description("Admin")]
        Admin = 1,
        [Description("Employee")]
        Employee = 2,
        [Description("Private")]
        Private = 3,
        [Description("Dealer")]
        Dealer = 4,
        [Description("Manufacturer")]
        Manufacturer = 5
    }
}
