using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nelibur.ObjectMapper;
using WFP.ICT.Data.Entities;
using WFP.ICT.Enum;
using WFP.ICT.Enums;
using System.ComponentModel.DataAnnotations;

namespace WFP.ICT.Web.Models
{
    public class OrderVm
    {
        public string Id { get; set; }
        public string OrderDate { get; set; }
        public string OrderNumber { get; set; }
        public string OrderType { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public string CallerFirstName { get; set; }
        [Required(ErrorMessage = "Last name is required")]
        public string CallerLastName { get; set; }

        public string CallerName
        {
            get { return CallerFirstName + " " + CallerLastName; }
        }
        
        [Required(ErrorMessage = "Contact is required")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string CallerPhoneNumber { get; set; }

        
        [Required(ErrorMessage = "Alternate Contact is required")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string CallerAlternatePhone { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress,ErrorMessage ="Please Enter Valid Email")]
        public string CallerEmail { get; set; }
        [Required(ErrorMessage = "Payment option is required")]
        public string PaymentOption { get; set; }

        [Required(ErrorMessage = "Office staff details are required")]
        public string OfficeStaffDetails { get; set; }

        [Required(ErrorMessage = "Collectable amount is required")]
        [Range(1, Double.MaxValue,ErrorMessage ="Collectable amount is required")]
        public double CollectableAmount { get; set; }

        [Required(ErrorMessage = "Online payment details are required")]
        public string OnlinePaymentDetails { get; set; }

        public string Customer { get; set; }
        public string PickupAddressString { get; set; }
        public string DeliveryAddressString { get; set; }
        public string PreferredPickupDateTime { get; set; }
        public string PickupDate { get; set; }
        public string DeliveryDate { get; set; }
        public string Shuttle { get; set; }

        public string DeliverForm { get; set; }


        public AddressVm PickupAddress { get; set; }
        public AddressVm DeliveryAddress { get; set; }
        public List<PianoVm> Pianos { get; set; }
        public List<PianoChargesVm> Charges { get; set; }
        public string PickupTicket { get; set; }

        //Fields for dealer and manufacturer
        public string ThirdParty { get; set; }
        public bool IsBilledThirdParty { get; set; }

        [Required(ErrorMessage = "Sales order number is required")]
        public string SalesOrderNumber { get; set; }

        [Required(ErrorMessage = "Carried by details are required")]
        public  string CarriedBy { get; set; }

        [Required(ErrorMessage = "Dealer is required")]
        public string Dealer { get; set; }

        public int OrderPlacementType { get; set; }

        public OrderVm()
        {
            Charges = new List<PianoChargesVm>();
            Pianos = new List<PianoVm>();
        }
        public static AddressVm PopulateAddress(Address address)
        {
            return new AddressVm
            {
                Name = address.Name,
                Address1 = address.Address1,
                City = address.City,
                State = address.State,
                Stairs = address.NumberStairs,
                Turns = address.NumberTurns,
                PostCode = address.PostCode,
                PhoneNumber = address.PhoneNumber,
                AlternateContact = address.AlternateContact,
                AlternatePhone = address.AlternatePhone,
                Warehouse = address.WarehouseId.ToString(),
            };
        }


    }
}