using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using Hangfire;
using Microsoft.AspNet.Identity.Owin;
using Nelibur.ObjectMapper;
using Newtonsoft.Json;
using WFP.ICT.Data.Entities;
using WFP.ICT.Web.Models;
using TransfocusTabletApp.Helpers;
using WFP.ICT.Web.Async;
using WFP.ICT.Web.FCM;
using WFP.ICT.Common;

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
            var consignments = db.PianoAssignments
                .Include(x => x.Drivers)
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
                    ConsignmentNumber = consignment.AssignmentNumber,
                    DriverName = String.Join(",", consignment.Drivers.Select(x=>x.Name).ToList()),
                    VehicleName = consignment.Vehicle?.Name,
                    StartWarehouseName = consignment.WarehouseStart?.Name
                };
                consigmentVms.Add(consigmentVm);
            }
            return View(consigmentVms);
        }

        // GET: New
        public ActionResult New()
        {
            ConsigmentVm model = new ConsigmentVm()
            {
                Id = Guid.NewGuid(),
            };
            ViewBag.Warehouses = new SelectList(WarehousesList, "Value", "Text");
            ViewBag.Vehicles = new SelectList(VehiclesList, "Value", "Text");
            ViewBag.Drivers = new SelectList(DriversList, "Value", "Text");
            ViewBag.Orders = new SelectList(OrdersList, "Value", "Text");

            return View(model);
        }

        // GET: NewOrder
        public ActionResult GetOrderDetails(Guid? Id)
        {
            try
            {
                var order = db.PianoOrders
                    .Include(x => x.Customer)
                    .Include(x => x.Pianos)
                    .Include(x => x.PickupAddress)
                    .Include(x => x.DeliveryAddress)
                    .FirstOrDefault(x => x.Id == Id);

                var piano = db.Pianos.Include(x => x.PianoType).FirstOrDefault(x => x.OrderId == order.Id);
                var pickupAddress = TinyMapper.Map<AddressVm>(order.PickupAddress).AddressToStringWithOutPhone;
                var deliveryAddress = TinyMapper.Map<AddressVm>(order.DeliveryAddress).AddressToStringWithOutPhone;

                var orderVM = new OrderVm()
                {
                    Id = order.Id.ToString(),
                    OrderDate = order.CreatedAt.ToString(),
                    OrderNumber = order.OrderNumber,
                    PreferredPickupDateTime = order.PreferredPickupDateTime?.ToString(),
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

        // POST: New
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(ConsigmentVm conVm)
        {
            //SetupLoggedInUser("test.user");
            if (ModelState.IsValid)
            {
                try
                {
                    var order = db.PianoOrders
                        .FirstOrDefault(x => x.Id == conVm.Orders);

                    PianoAssignment consignment;
                    if (!order.PianoAssignmentId.HasValue)
                    {
                        consignment = new PianoAssignment()
                        {
                            Id = conVm.Orders.Value, // Needs to be same as orderId to maintain 1 vs 1 realtion integrity
                            CreatedAt = DateTime.Now,
                            CreatedBy = LoggedInUser?.UserName,
                            PianoOrderId = conVm.Orders,
                           // WarehouseStartId = conVm.Warehouses,
                            VehicleId = conVm.Vehicles,
                            AssignmentNumber = "CON-" + order.OrderNumber
                        };
                        db.PianoAssignments.Add(consignment);
                        db.SaveChanges();

                   
                        conVm.Drivers.ForEach(x => {
                            consignment.Drivers.Add(db.Drivers.FirstOrDefault(y => y.Id == x.Value));
                        });

                        int odr = 1;
                        var paths = JsonConvert.DeserializeObject<ConsignmentRouteVm[]>(conVm.Paths);
                        foreach (var path in paths)
                        {
                            db.PianoAssignmentRoutes.Add(new PianoAssignmentRoute()
                            {
                                Id = Guid.NewGuid(),
                                CreatedAt = DateTime.Now,
                                PianoAssignmentId = consignment.Id,
                                Order = odr,
                                Lat = path.Lat,
                                Lng = path.Lng
                            });
                            odr++;
                        }
                        db.SaveChanges();
                    }
                    else
                    {
                        consignment = db.PianoAssignments
                            .Include(x => x.Route)
                            .FirstOrDefault(x => x.Id == order.PianoAssignmentId);

                        consignment.PianoOrderId = conVm.Orders;
                        consignment.WarehouseStartId = conVm.Warehouses;
                        consignment.VehicleId = conVm.Vehicles;
                        consignment.AssignmentNumber = "CON-" + order.OrderNumber;
                        db.SaveChanges();

                        consignment.Drivers.ToList().ForEach(x => {
                            consignment.Drivers.Remove(db.Drivers.FirstOrDefault(y => y.Id == x.Id));
                        });
                        db.SaveChanges();

                        conVm.Drivers.ForEach(x => {
                            consignment.Drivers.Add(db.Drivers.FirstOrDefault(y => y.Id == x.Value));
                        });


                        foreach (var route in consignment.Route)
                        {
                            db.PianoAssignmentRoutes.Remove(route);
                        }
                        db.SaveChanges();

                        int odr = 1;
                        var paths = JsonConvert.DeserializeObject<ConsignmentRouteVm[]>(conVm.Paths);
                        foreach (var path in paths)
                        {
                            db.PianoAssignmentRoutes.Add(new PianoAssignmentRoute()
                            {
                                Id = Guid.NewGuid(),
                                CreatedAt = DateTime.Now,
                                PianoAssignmentId = consignment.Id,
                                Order = odr,
                                Lat = path.Lat,
                                Lng = path.Lng
                            });
                            odr++;
                        }
                        db.SaveChanges();

                    }

                    var consignmentDrivers = db.PianoAssignments
                                              .Include(x => x.Drivers).FirstOrDefault(x => x.Id == consignment.Id);

                    consignmentDrivers.Drivers.ToList().ForEach(x =>
                    {
                        BackgroundJob.Enqueue(() => FCMUitlity.SendConsignment(x.FCMToken, consignment.Id.ToString(), consignment.AssignmentNumber));

                    });

                    TempData["Success"] = "Conisgnment #: " + consignment.AssignmentNumber +
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

        public ActionResult PickTickets()
        {
            var consigmentVms = new List<ConsigmentVm>();
            var consignments = db.PianoAssignments
                .Include(x => x.Drivers)
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
                    ConsignmentNumber = consignment.AssignmentNumber,
                    DriverName = String.Join(",", consignment.Drivers.Select(x => x.Name).ToList()),
                    VehicleName = consignment.Vehicle?.Name,
                    StartWarehouseName = consignment.WarehouseStart?.Name
                };
                consigmentVms.Add(consigmentVm);
            }
            return View(consigmentVms);
        }

        public ActionResult Generate()
        {
            var consignments = db.PianoAssignments
                .Include(x => x.Drivers)
                .Include(x => x.Vehicle)
                .Include(x => x.WarehouseStart)
                .ToList();//.Where(x => x.CreatedBy == LoggedInUser.Id)

            List<OrderVm> orderVMs = new List<OrderVm>();
            foreach (var consignment in consignments)
            {
                var order = db.PianoOrders
                    .Include(x => x.PickupAddress)
                    .Include(x => x.DeliveryAddress)
                    .FirstOrDefault(x => x.Id == consignment.PianoOrderId);

                string prefix = consignment.WarehouseStart.Code;
                var barcode = GeneratorHelper.GenerateBarcode(prefix, int.Parse(order.OrderNumber));
                consignment.PickupTicket = barcode;
                consignment.PickupTicketGenerationTime = DateTime.Now;
                db.SaveChanges();

                var pickupAddress = TinyMapper.Map<AddressVm>(order.PickupAddress).AddressToString;
                var deliveryAddress = TinyMapper.Map<AddressVm>(order.DeliveryAddress).AddressToString;

                var orderVM = new OrderVm()
                {
                    Id = order.Id.ToString(),
                    OrderDate = order.CreatedAt.ToString(),
                    OrderNumber = order.OrderNumber,
                    PreferredPickupDateTime = order.PreferredPickupDateTime?.ToString(),
                    PickupAddressString = pickupAddress,
                    DeliveryAddressString = deliveryAddress,
                    PickupDate = order.PickupDate?.ToString(),
                    DeliveryDate = order.DeliveryDate?.ToString(),
                    Customer = order.Customer != null ? order.Customer.AccountCode + " " + order.Customer.Name : "",
                    PickupTicket = consignment.PickupTicket
                };

                orderVMs.Add(orderVM);
            }
            // Generate 
            var filePath = QRCodeGenerator.Generate(ImagesPath, UploadPath, orderVMs);
            return File(filePath, "application/octet-stream", "PickupTickets.pdf");

        }

        [HttpPost]
        public ActionResult Send(Guid? id)
        {
            try
            {
                var consignment = db.PianoAssignments
                            .Include(x=> x.Drivers).FirstOrDefault(x => x.Id == id);

                var driver = consignment.Drivers.ToList();
                driver.ForEach(x =>
                {
                    BackgroundJob.Enqueue(() => FCMUitlity.SendConsignment(x.FCMToken, consignment.Id.ToString(), consignment.AssignmentNumber));

                });
              
                return Json(new JsonResponse() { IsSucess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Invoice(Guid? id)
        {
            try
            {
                var order = db.PianoOrders
                            .Include(x => x.Customer)
                            .Include(x => x.Pianos.Select(y => y.PianoMake))
                            .Include(x => x.Pianos.Select(y => y.PianoType))
                            .Include(x => x.Pianos.Select(y => y.PianoSize))
                            .Include(x => x.PianoAssignment.Drivers)
                            .Include(x => x.PickupAddress)
                            .Include(x => x.DeliveryAddress)
                            .Include(x => x.OrderCharges.Select(y=>y.PianoCharges))
                            .FirstOrDefault(x => x.Id == id);

                string _directoryPath = UploadPath + "\\InvoiceCodes";

                string html = InvoiceHtmlHelper.GenerateInvoiceHtml(order, _directoryPath);

                JsonResponse Path = HtmlToPdf(html,order.PianoAssignment.AssignmentNumber);
                string pathValue = Path.Result.ToString();

                if (!System.IO.File.Exists(pathValue))
                    return null;
                return File(pathValue, "application/pdf", "Invoice.pdf");

            }
            catch (Exception ex)
            {
                return File(new byte[0] , "application/pdf", "Error.pdf");
            }

         
        }
        public JsonResponse HtmlToPdf(string html , string conNum)
        {
            try
            {
                var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
                var pdfBytes = htmlToPdf.GeneratePdf(html);

                string Path = Server.MapPath("~/Uploads/ConsignmentInvoices");
                if (!System.IO.Directory.Exists(Path)) System.IO.Directory.CreateDirectory(Path);
                string filePath = System.IO.Path.Combine(Path, conNum +"_Invoice.pdf");

                using (var fileStream = System.IO.File.Create(filePath))
                {
                    fileStream.Write(pdfBytes, 0, pdfBytes.Length);
                }
                return new JsonResponse() { IsSucess = true, Result = filePath };
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}