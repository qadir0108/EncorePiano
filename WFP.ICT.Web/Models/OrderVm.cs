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
        public string OrderMedium { get; set; }

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

        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string CallerAlternatePhone { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress,ErrorMessage ="Please Enter Valid Email")]
        public string CallerEmail { get; set; }
        [Required(ErrorMessage = "Payment option is required")]
        public string PaymentOption { get; set; }
        public string OfficeStaffDetails { get; set; }
        public double CollectableAmount { get; set; }
        public string OnlinePaymentDetails { get; set; }

        public string Notes { get; set; }
        public string Customer { get; set; }
        public string Customers { get; set; }
        public string PickupAddressString { get; set; }
        public string DeliveryAddressString { get; set; }
        public string PreferredPickupDateTime { get; set; }
        public string PickupDate { get; set; }
        public string DeliveryDate { get; set; }
        public string Shuttle { get; set; }

        public AddressVm PickupAddress { get; set; }
        public AddressVm DeliveryAddress { get; set; }
        public List<PianoVm> Pianos { get; set; }
        public List<PianoServiceVm> Services { get; set; }
        public string PickupTicket { get; set; }

        public OrderVm()
        {
            Services = new List<PianoServiceVm>();
            Pianos = new List<PianoVm>();
        }
        public static OrderVm FromOrder(PianoOrder order, IEnumerable<SelectListItem> PianoTypesList)
        {
            var pickupAddress = TinyMapper.Map<AddressVm>(order.PickupAddress).AddressToString;
            var deliveryAddress = TinyMapper.Map<AddressVm>(order.DeliveryAddress).AddressToString;

            var orderVM = new OrderVm()
            {
                Id = order.Id.ToString(),
                OrderDate = order.CreatedAt.ToString(),
                OrderNumber = order.OrderNumber,
                OrderType = ((OrderTypeEnum)order.OrderType).ToString(),
                OrderMedium = ((OrderMediumEnum)order.OrderMedium).ToString(),
                CallerFirstName = order.CallerFirstName,
                CallerLastName = order.CallerLastName,
                CallerPhoneNumber = order.CallerPhoneNumber,
                CallerEmail = order.CallerEmail,
                PreferredPickupDateTime = order.PreferredPickupDateTime?.ToString(StringConstants.TimeStampFormatSlashes),
                Notes = order.Notes,
                PickupAddressString = pickupAddress,
                DeliveryAddressString = deliveryAddress,
                PickupDate = order.PickupDate?.ToString(),
                DeliveryDate = order.DeliveryDate?.ToString(),
                Customer = order.Customer != null ? order.Customer.AccountCode + " " + order.Customer.Name : ""
            };

            orderVM.Pianos = order.Pianos.OrderByDescending(x => x.CreatedAt).Select(
                x => new PianoVm()
                {
                    Id = x.Id,
                    OrderId = order.Id,
                    PianoType = PianoTypesList.FirstOrDefault(y => y.Value == x.PianoTypeId.ToString()).Text,
                    PianoName = x.Name,
                    PianoColor = x.Color,
                    PianoModel = x.Model,
                    PianoMake = x.Make,
                    SerialNumber = x.SerialNumber,
                    IsBench = x.IsBench,
                    IsBoxed = x.IsBoxed,
                    IsStairs = x.IsPlayer
                }).ToList();
            orderVM.Services = order.OrderCharges.OrderBy(x => x.Id).Select(
               x => new PianoServiceVm()
               {
                   Id = x.Id.ToString(),
                  // ServiceCode = x.PianoCharges.ChargesCode.ToString(),
                 //  ServiceType = ((ChargesTypeEnum)x.PianoCharges.ChargesType).ToString(),
                  // ServiceDetails = x.PianoCharges.ChargesDetails,
                   ServiceCharges = x.Amount.ToString()
               }).ToList();

            return orderVM;
        }

    }
}