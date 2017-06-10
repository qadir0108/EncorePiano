using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Nelibur.ObjectMapper;
using WFP.ICT.Data.Entities;
using WFP.ICT.Web.Helpers;
using WFP.ICT.Web.Models;
using PagedList;
using WFP.ICT.Enum;
using WFP.ICT.Enums;

namespace WFP.ICT.Web.Controllers
{
    //[Authorize]
    public class OrdersStatusController : BaseController
    {
        int pageSize = 15;
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        public ActionResult Index()
        {
            var orderVMs = new List<OrderVm>();
            var orders = db.PianoOrders
                .Include(x => x.Customer)
                .Include(x => x.Pianos)
                .Include(x => x.PickupAddress)
                .Include(x => x.DeliveryAddress)
                .Include(x => x.Services)
                .ToList();//.Where(x => x.CreatedBy == LoggedInUser.Id)
            foreach (var order in orders)
            {
                var pickupAddress = TinyMapper.Map<AddressVm>(order.PickupAddress).AddressToString;
                var deliveryAddress = TinyMapper.Map<AddressVm>(order.DeliveryAddress).AddressToString;

                var orderVM = new OrderVm()
                {
                    Id = order.Id.ToString(),
                    OrderDate = order.CreatedAt.ToString(),
                    OrderNumber = order.OrderNumber,
                    OrderMedium = ((OrderMediumEnum)order.OrderMedium).ToString(),
                    CallerFirstName = order.CallerFirstName,
                    CallerLastName = order.CallerLastName,
                    CallerPhoneNumber = order.CallerPhoneNumber,
                    CallerEmail = order.CallerEmail,
                    PreferredPickupDateTime = order.PreferredPickupDateTime?.ToString(),
                    Notes = order.Notes,
                    PickupAddressString  = pickupAddress,
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
                        IsStairs = x.IsStairs
                    }).ToList();
                orderVM.Services = order.Services.OrderBy(x => x.ServiceCode).Select(
                    x => new PianoServiceVm()
                    {
                        Id = x.Id.ToString(),
                        ServiceCode = x.ServiceCode.ToString(),
                        ServiceType = ((ServiceTypeEnum) x.ServiceType).ToString(),
                        ServiceDetails = x.ServiceDetails,
                        ServiceCharges = x.ServiceCharges.ToString()
                    }).ToList();
                orderVMs.Add(orderVM);

                _forceRefreshOrders = true;
            }
            return View(orderVMs);
        }
        
    }
}