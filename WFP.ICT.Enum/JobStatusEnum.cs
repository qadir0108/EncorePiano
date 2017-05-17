using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace WFP.ICT.Enum
{
    public enum JobStatusEnum
    {
        [Description("Booked")]
        B = 1,

        [Description("Verified")]
        V = 2,

        [Description("Ready for dispatch")]
        R = 3,

        [Description("Dispatched")]
        D = 4,

        [Description("Checked")]
        C = 5,

        [Description("POD completed")]
        P = 6,

        [Description("Exported")]
        E = 7,

        [Description("Finished")]
        F = 8
    }
}
