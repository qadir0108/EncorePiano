using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using WFP.ICT.Data.Entities;
using WFP.ICT.Web.Helpers;
using WFP.ICT.Web.Models;
using PagedList;
using WFP.ICT.Enum;
using WFP.ICT.Enums;

namespace WFP.ICT.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
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
                .Include(x => x.Items)
                .Include(x => x.PickupAddress)
                .Include(x => x.DeliveryAddress)
                .ToList();//.Where(x => x.CreatedBy == LoggedInUser.Id)
            foreach (var order in orders)
            {
                var piano = db.Pianos.Include(x => x.PianoType).FirstOrDefault(x => x.OrderId == order.Id);
                var pickupAddress = new AddressVm(order.PickupAddress).AddressToString;
                var deliveryAddress = new AddressVm(order.DeliveryAddress).AddressToString;

                var orderVM = new OrderVm()
                {
                    Id = order.Id,
                    OrderDate = order.CreatedAt.ToString(),
                    OrderNumber = order.OrderNumber,
                    Notes = order.Notes,
                    PianoType = piano.PianoType.Type,
                    PianoName = piano.Name,
                    PianoMake = piano.Make,
                    PianoColor = piano.Color,
                    PianoModel = piano.Model,
                    IsBench = piano.IsBench,
                    IsBoxed = piano.IsBoxed,
                    
                    PickupAddressString  = pickupAddress,
                    DeliveryAddressString = deliveryAddress,
                    PickupDate = order.PickupDate?.ToString(),
                    DeliveryDate = order.DeliveryDate?.ToString()
                };
                orderVMs.Add(orderVM);
            }
            return View(orderVMs);
        }

        // GET: NewOrder
        public ActionResult NewOrder()
        {
            OrderVm model = new OrderVm()
            {
                Id = Guid.NewGuid(),
                OrderNumber = "",
            };
            //ViewBag.PianoType = new SelectList(EnumHelper.GetEnumTextValues(typeof(PianoTypeEnum)), "Value","Text");
            ViewBag.PianoType = new SelectList(PianoTypesList, "Value", "Text");
            return View(model);
        }

        // POST: NewOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewOrder(OrderVm orderVm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var camps = db.PianoOrders.ToList();
                    int newOrderNumber = camps.Count > 0
                        ? camps.Max(x => int.Parse(x.OrderNumber.TrimEnd("RDP".ToCharArray()))) + 1
                        : 2500;

                    var orderId = Guid.NewGuid();
                    var pianoId = Guid.NewGuid();
                    var pickupAddressId = Guid.NewGuid();
                    var deliveryAddressId = Guid.NewGuid();

                    var piano = new Piano()
                    {
                        Id = pianoId,
                        CreatedAt = DateTime.Now,
                        SerialNumber = orderVm.SerialNumber,
                        Name = orderVm.PianoName,
                        Color = orderVm.PianoColor,
                        Model = orderVm.PianoModel,
                        Make = orderVm.PianoMake,
                        IsBench = orderVm.IsBench,
                        IsBoxed = orderVm.IsBoxed,
                        PianoTypeId = Guid.Parse(orderVm.PianoType)
                    };
                    db.Pianos.Add(piano);
                    db.SaveChanges();

                    Piano[] pianos = new Piano[] { piano };

                    Address pickupAddress = new Address()
                    {
                        Id = pickupAddressId,
                        CreatedAt = DateTime.Now,
                        Name = orderVm.PickupAddress.Name,
                        Address1 = orderVm.PickupAddress.Address1,
                        Suburb = orderVm.PickupAddress.Suburb,
                        State = orderVm.PickupAddress.State,
                        PostCode = orderVm.PickupAddress.PostCode,
                        PhoneNumber = orderVm.PickupAddress.PhoneNumber
                    };
                    db.Addresses.Add(pickupAddress);
                    db.SaveChanges();

                    Address deliveryAddress = new Address()
                    {
                        Id = deliveryAddressId,
                        CreatedAt = DateTime.Now,
                        Name = orderVm.DeliveryAddress.Name,
                        Address1 = orderVm.DeliveryAddress.Address1,
                        Suburb = orderVm.DeliveryAddress.Suburb,
                        State = orderVm.DeliveryAddress.State,
                        PostCode = orderVm.DeliveryAddress.PostCode,
                        PhoneNumber = orderVm.DeliveryAddress.PhoneNumber
                    };
                    db.Addresses.Add(deliveryAddress);
                    db.SaveChanges();

                    piano.OrderId = orderId;
                    SetupLoggedInUser("test.user");
                    var order = new PianoOrder()
                    {
                        Id = orderId,
                        CreatedAt = DateTime.Now,
                        CreatedBy = LoggedInUser.UserName,
                        OrderNumber = newOrderNumber.ToString(),
                        Notes = orderVm.Notes,
                        Items = pianos,
                        PickupAddressId = pickupAddressId,
                        DeliveryAddressId = deliveryAddressId,
                    };

                    db.PianoOrders.Add(order);
                    db.SaveChanges();

                    //var threadParams = new EmailThreadParams() { idFirst = campaign.Id, user = LoggedInUser };
                    //BackgroundJob.Enqueue(() => CampaignProcessor.ProcessNewOrder(threadParams));

                    TempData["Success"] = "Order #: " + order.OrderNumber + " has been submitted sucessfully.";
                    return RedirectToAction("Index");

                }
                catch (Exception ex)
                {
                    TempData["Error"] = "This is error while creating order." + ex.Message;
                    ViewBag.PianoType = new SelectList(PianoTypesList, "Value", "Text");
                    return View(orderVm);
                }
            }
            return View(orderVm);
        }

    }
}