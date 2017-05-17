using System;
using WFP.ICT.Enum;

namespace WFP.ICT.Web.Models
{
    public class GatepassRequestedItemVM
    {
        public Guid GatepassRequestItemID { get; set; }
        public Guid GatepassRequestID { get; set; }
        public Guid InventoryItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemMake { get; set; }
        public int ItemStatus { get; set; }
        public string InventoryNumber { get; set; }
        public string SerialNumber { get; set; }
        public string AcquistionDate { get; set; }
        public string AcquistionValue { get; set; }

    }
}