using System;
using WFP.ICT.Enum;

namespace WFP.ICT.Web.Models
{
    public class OrderVm
    {
        public Guid Id { get; set; }
        public string OrderDate { get; set; }
        public string OrderNumber { get; set; }
        public string PianoType { get; set; }
        public string PianoName { get; set; }
        public string PianoColor { get; set; }
        public string PianoModel { get; set; }
        public string PianoMake { get; set; }
        public string SerialNumber { get; set; }
        public string Notes { get; set; }
        public bool IsBench { get; set; } // W/B OR N/B
        public bool IsBoxed { get; set; }

        public string PickupAddressString { get; set; }
        public string DeliveryAddressString { get; set; }
        public string PickupDate { get; set; }
        public string DeliveryDate { get; set; }

        public AddressVm PickupAddress { get; set; }
        public AddressVm DeliveryAddress { get; set; }
    }
}