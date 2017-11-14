using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WFP.ICT.Enum
{
    public enum AdditionalItemStatus
    {
        Unknown, // Not Set
        NotAvailable,
        Missing,
        Loaded, // LoadedOnTruck
        Delivered // DeliveredToCustomer
    }
}
