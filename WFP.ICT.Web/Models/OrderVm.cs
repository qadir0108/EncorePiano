using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WFP.ICT.Enum;

namespace WFP.ICT.Web.Models
{
    public class OrderVm
    {
        public string Id { get; set; }
        public string OrderDate { get; set; }
        public string OrderNumber { get; set; }
        public string OrderType { get; set; }
        public string OrderMedium { get; set; }
        public string CallerFirstName { get; set; }
        public string CallerLastName { get; set; }

        public string CallerName
        {
            get { return CallerFirstName + " " + CallerLastName; }
        }

        public string CallerPhoneNumber { get; set; }
        public string CallerEmail { get; set; }

        public string PaymentOption { get; set; }
        public string Notes { get; set; }
        
        public string Customer { get; set; }
        public string Customers { get; set; }
        public string PickupAddressString { get; set; }
        public string DeliveryAddressString { get; set; }
        public string PreferredPickupDateTime { get; set; }
        public string PickupDate { get; set; }
        public string DeliveryDate { get; set; }

        public AddressVm PickupAddress { get; set; }
        public AddressVm DeliveryAddress { get; set; }

        public List<PianoVm> Pianos { get; set; }
        public PianoVm P1 { get; set; }
        public PianoVm P2 { get; set; }
        public PianoVm P3 { get; set; }
        public PianoVm P4 { get; set; }
        public PianoVm P5 { get; set; }

        public List<PianoServiceVm> Services { get; set; }

        public string PickupTicket { get; set; }

    }
}