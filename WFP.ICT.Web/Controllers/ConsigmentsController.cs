using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using WFP.ICT.Data.Entities;
using WFP.ICT.Web.Helpers;
using WFP.ICT.Web.Models;
using PagedList;
using WFP.ICT.Enum;
using WFP.ICT.Enums;

namespace WFP.ICT.Web.Controllers
{
    //[Authorize]
    public class ConsignmentsController : BaseController
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
            var consigmentVms = new List<ConsigmentVm>();
            var consignments = db.PianoConsignments
                .Include(x => x.Driver)
                .Include(x => x.Vehicle)
                .Include(x => x.WarehouseStart)
                .ToList();//.Where(x => x.CreatedBy == LoggedInUser.Id)
            foreach (var consignment in consignments)
            {
                var order = db.PianoOrders.FirstOrDefault(x => x.Id == consignment.PianoOrderId);

                var consigmentVm = new ConsigmentVm()
                {
                    Id = consignment.Id,
                    CreatedAt = consignment.CreatedAt.ToString(),
                    OrderId = consignment.PianoOrderId?.ToString(),
                    OrderNumber = order.OrderNumber,
                    ConsignmentNumber = consignment.ConsignmentNumber,
                    DriverName = consignment.Driver?.Name,
                    VehicleName = consignment.Vehicle?.Name,
                    StartWarehouseName = consignment.WarehouseStart?.Name
                };
                consigmentVms.Add(consigmentVm);
            }
            return View(consigmentVms);
        }

        // GET: NewOrder
        public ActionResult New()
        {
            ConsigmentVm model = new ConsigmentVm()
            {
                Id = Guid.NewGuid(),
            };
            ViewBag.Warehouses = new SelectList(WarehousesList, "Value", "Text");
            ViewBag.Vehicles = new SelectList(VehiclesList, "Value", "Text");
            ViewBag.Drivers = new SelectList(DriversList, "Value", "Text");
            ViewBag.Orders = new SelectList(OrdersList, "Value","Text");
            
            return View(model);
        }

        // GET: NewOrder
        public ActionResult GetOrderDetails(Guid? Id)
        {
            try
            {
                var order = db.PianoOrders
                    .Include(x => x.Customer)
                    .Include(x => x.Items)
                    .Include(x => x.PickupAddress)
                    .Include(x => x.DeliveryAddress)
                    .FirstOrDefault(x => x.Id == Id);
                
                var piano = db.Pianos.Include(x => x.PianoType).FirstOrDefault(x => x.OrderId == order.Id);
                var pickupAddress = new AddressVm(order.PickupAddress).AddressToStringWithOutPhone;
                var deliveryAddress = new AddressVm(order.DeliveryAddress).AddressToStringWithOutPhone;

                var orderVM = new OrderVm()
                {
                    Id = order.Id,
                    OrderDate = order.CreatedAt.ToString(),
                    OrderNumber = order.OrderNumber,
                    IsStairs = order.IsStairs,
                    PreferredPickupDateTime = order.PreferredPickupDateTime?.ToString(),
                    Notes = order.Notes,
                    PianoType = piano.PianoType.Type,
                    PianoName = piano.Name,
                    PianoMake = piano.Make,
                    PianoColor = piano.Color,
                    PianoModel = piano.Model,
                    IsBench = piano.IsBench,
                    IsBoxed = piano.IsBoxed,
                    PickupAddressString = pickupAddress,
                    DeliveryAddressString = deliveryAddress,
                    PickupDate = order.PickupDate?.ToString(),
                    DeliveryDate = order.DeliveryDate?.ToString(),
                    Customer = order.Customer != null ? order.Customer.AccountCode + " " + order.Customer.Name : ""
                };
                
                return Json(new JsonResponse() { IsSucess = true, Result = orderVM.ToJson() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: NewOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New(ConsigmentVm conVm)
        {
            SetupLoggedInUser("test.user");
            if (ModelState.IsValid)
            {
                try
                {
                    var order = db.PianoOrders.FirstOrDefault(x => x.Id == conVm.Orders);
                    var consignment = new PianoConsignment()
                    {
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTime.Now,
                        CreatedBy = LoggedInUser?.UserName,
                        PianoOrderId = conVm.Orders,
                        DriverId = conVm.Drivers,
                        WarehouseStartId = conVm.Warehouses,
                        VehicleId = conVm.Vehicles,
                        ConsignmentNumber = "CON-" + order.OrderNumber
                    };
                    db.PianoConsignments.Add(consignment);
                    db.SaveChanges();

                    int odr = 1;
                    var paths = JsonConvert.DeserializeObject<ConsignmentRouteVm[]>(conVm.Paths);
                    foreach (var path in paths)
                    {
                        db.PianoConsignmentRoutes.Add(new PianoConsignmentRoute()
                        {
                            Id = Guid.NewGuid(),
                            CreatedAt = DateTime.Now,
                            PianoConsignmentId = consignment.Id,
                            Order = order.ToString(),
                            Lat = path.Lat,
                            Lng = path.Lng
                        });
                        odr++;
                    }
                    db.SaveChanges();
                    //BackgroundJob.Enqueue(() => CampaignProcessor.ProcessNewOrder(threadParams));

                    TempData["Success"] = "Conisgnment #: " + consignment.ConsignmentNumber +
                                          " has been saved sucessfully.";
                    return RedirectToAction("Index");

                }
                catch (Exception ex)
                {
                    TempData["Error"] = "This is error while creating order." + ex.Message;
                }
            }
            ViewBag.Warehouses = new SelectList(WarehousesList, "Value", "Text");
            ViewBag.Vehicles = new SelectList(VehiclesList, "Value", "Text");
            ViewBag.Drivers = new SelectList(DriversList, "Value", "Text");
            ViewBag.Orders = new SelectList(OrdersList, "Value", "Text");

            return View(conVm);
        }
    }
}