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
using WFP.ICT.Enum;
using WFP.ICT.Enums;

namespace WFP.ICT.Web.Controllers
{
    //[Authorize]
    public class ConsignmentsController : BaseController
    {
        int pageSize = 15;

        public ActionResult Index()
        {
            var consigmentVms = new List<ConsigmentVm>();
            var consignments = Db.PianoAssignments
                .Include(x => x.Drivers)
                .Include(x => x.Vehicle)
                .Include(x => x.Statuses)
                .ToList();//.Where(x => x.CreatedBy == LoggedInUser.Id)
            foreach (var consignment in consignments)
            {
                var order = Db.PianoOrders.Include(x => x.Pianos).FirstOrDefault(x => x.Id == consignment.PianoOrderId);

                var consigmentVm = new ConsigmentVm()
                {
                    Id = consignment.Id,
                    CreatedAt = consignment.CreatedAt.ToString(),
                    OrderId = consignment.PianoOrderId?.ToString(),
                    OrderNumber = order.OrderNumber,
                    ConsignmentNumber = consignment.AssignmentNumber,
                    DriverName = String.Join(",", consignment.Drivers.Select(x=>x.Name).ToList()),
                    VehicleName = consignment.Vehicle?.Name,
                    TripStatus = consignment.Statuses.Count == 0 ? TripStatusEnum.NotStarted.ToString()
                    : ((TripStatusEnum)(consignment.Statuses.OrderByDescending(x => x.CreatedAt).FirstOrDefault().Status)).ToString(),
                    StartTime = consignment.StartTime?.ToString(),
                    EstimatedTime = consignment.EstimatedTime?.ToString()
                };

                foreach (var status in consignment.Statuses.OrderByDescending(x => x.CreatedAt))
                {
                    consigmentVm.Statuses.Add(new StatusVm()
                    {
                        Status = ((TripStatusEnum)status.Status).ToString(),
                        StatusBy = status.StatusBy,
                        StatusTime = status.StatusTime.ToString(),
                        Comments = status.Comments
                    });
                }
                consigmentVms.Add(consigmentVm);
            }
            return View(consigmentVms);
        }

        public ActionResult UnitsPickup(string orderNumber)
        {
            var order = Db.PianoOrders.Include(x => x.Pianos)
                .FirstOrDefault(x => x.OrderNumber == orderNumber);

            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("Index", "Consignments");
            }

            var orderVM = new OrderVm()
            {
                Id = order.Id.ToString(),
                OrderDate = order.CreatedAt.ToString(),
                OrderNumber = order.OrderNumber
             };

            orderVM.Pianos = order.Pianos.OrderByDescending(x => x.CreatedAt).Select(
               x => new PianoVm()
               {
                   Id = x.Id,
                   OrderId = order.Id,
                   PianoCategoryType = System.Enum.GetName(typeof(PianoCategoryTypeEnum), x.PianoCategoryType),
                   PianoType = PianoTypesList.FirstOrDefault(y => y.Value == x.PianoTypeId.ToString()).Text,
                   PianoSize = x.PianoSizeId.ToString(),
                   PianoMake = x.PianoMakeId.ToString(),
                   PianoModel = x.Model,
                   PianoFinish = PianoFinishList.FirstOrDefault(y => y.Value == x.PianoFinishId.ToString()).Text,
                   SerialNumber = x.SerialNumber,
                   IsBench = x.IsBench,
                   IsBoxed = x.IsBoxed,
                   IsPlayer = x.IsPlayer,
                   Notes = x.Notes,

                   IsMainUnitLoaded = x.IsMainUnitLoaded ? "Yes" : "No",
                   AdditionalBenchesStatus = System.Enum.GetName(typeof(AdditionalItemStatus), x.AdditionalBenchesStatus),
                   AdditionalCasterCupsStatus = System.Enum.GetName(typeof(AdditionalItemStatus), x.AdditionalCasterCupsStatus),
                   AdditionalCoverStatus = System.Enum.GetName(typeof(AdditionalItemStatus), x.AdditionalCoverStatus),
                   AdditionalLampStatus = System.Enum.GetName(typeof(AdditionalItemStatus), x.AdditionalLampStatus),
                   AdditionalOwnersManualStatus = System.Enum.GetName(typeof(AdditionalItemStatus), x.AdditionalOwnersManualStatus),
                   LoadTimeStamp = x.LoadTimeStamp?.ToString(StringConstants.TimeStampFormatSlashes),
               }).ToList();

            orderVM.Pianos.ForEach(x =>
            {
                if (x.PianoMake != string.Empty)
                {
                    x.PianoMake = Db.PianoMake.Where(y => y.Id.ToString() == x.PianoMake).FirstOrDefault().Name;
                }

                if (x.PianoSize != string.Empty)
                {
                    x.PianoSize = Db.PianoSize.Where(y => y.Id.ToString() == x.PianoSize).FirstOrDefault().Width.ToString();
                }
            });

            return View(orderVM);
        }

        public ActionResult UnitsDelivery(string orderNumber)
        {
            var order = Db.PianoOrders.Include(x => x.Pianos)
                .FirstOrDefault(x => x.OrderNumber == orderNumber);

            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("Index", "Consignments");
            }

            var orderVM = new OrderVm()
            {
                Id = order.Id.ToString(),
                OrderDate = order.CreatedAt.ToString(),
                OrderNumber = order.OrderNumber
            };

            orderVM.Pianos = order.Pianos.OrderByDescending(x => x.CreatedAt).Select(
               x => new PianoVm()
               {
                   Id = x.Id,
                   OrderId = order.Id,
                   PianoCategoryType = System.Enum.GetName(typeof(PianoCategoryTypeEnum), x.PianoCategoryType),
                   PianoType = PianoTypesList.FirstOrDefault(y => y.Value == x.PianoTypeId.ToString()).Text,
                   PianoSize = x.PianoSizeId.ToString(),
                   PianoMake = x.PianoMakeId.ToString(),
                   PianoModel = x.Model,
                   PianoFinish = PianoFinishList.FirstOrDefault(y => y.Value == x.PianoFinishId.ToString()).Text,
                   SerialNumber = x.SerialNumber,
                   IsBench = x.IsBench,
                   IsBoxed = x.IsBoxed,
                   IsPlayer = x.IsPlayer,
                   Notes = x.Notes,

                   IsMainUnitLoaded = x.IsMainUnitLoaded ? "Yes" : "No",
                   AdditionalBenchesStatus = System.Enum.GetName(typeof(AdditionalItemStatus), x.AdditionalBenchesStatus),
                   AdditionalCasterCupsStatus = System.Enum.GetName(typeof(AdditionalItemStatus), x.AdditionalCasterCupsStatus),
                   AdditionalCoverStatus = System.Enum.GetName(typeof(AdditionalItemStatus), x.AdditionalCoverStatus),
                   AdditionalLampStatus = System.Enum.GetName(typeof(AdditionalItemStatus), x.AdditionalLampStatus),
                   AdditionalOwnersManualStatus = System.Enum.GetName(typeof(AdditionalItemStatus), x.AdditionalOwnersManualStatus),
                   LoadTimeStamp = x.LoadTimeStamp?.ToString(StringConstants.TimeStampFormatSlashes),
               }).ToList();

            orderVM.Pianos.ForEach(x =>
            {
                if (x.PianoMake != string.Empty)
                {
                    x.PianoMake = Db.PianoMake.Where(y => y.Id.ToString() == x.PianoMake).FirstOrDefault().Name;
                }

                if (x.PianoSize != string.Empty)
                {
                    x.PianoSize = Db.PianoSize.Where(y => y.Id.ToString() == x.PianoSize).FirstOrDefault().Width.ToString();
                }
            });

            return View(orderVM);
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
                var order = Db.PianoOrders
                    .Include(x => x.Customer)
                    .Include(x => x.Pianos)
                    .Include(x => x.PickupAddress)
                    .Include(x => x.DeliveryAddress)
                    .FirstOrDefault(x => x.Id == Id);

                var piano = Db.Pianos.Include(x => x.PianoType).FirstOrDefault(x => x.OrderId == order.Id);
                var pickupAddress = TinyMapper.Map<AddressVm>(order.PickupAddress).ToStringWithOutPhone;
                var deliveryAddress = TinyMapper.Map<AddressVm>(order.DeliveryAddress).ToStringWithOutPhone;

                var orderVM = new OrderVm()
                {
                    Id = order.Id.ToString(),
                    OrderDate = order.CreatedAt.ToString(),
                    OrderNumber = order.OrderNumber,
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
                ViewBag.Warehouses = new SelectList(WarehousesList, "Value", "Text");
                ViewBag.Vehicles = new SelectList(VehiclesList, "Value", "Text");
                ViewBag.Drivers = new SelectList(DriversList, "Value", "Text");
                ViewBag.Orders = new SelectList(OrdersList, "Value", "Text");

                try
                {
                    var order = Db.PianoOrders.Include(x => x.Pianos)
                        .FirstOrDefault(x => x.Id == conVm.Orders);

                    PianoAssignment consignment;
                    if (!order.PianoAssignmentId.HasValue) // New
                    {
                        consignment = new PianoAssignment()
                        {
                            Id = conVm.Orders.Value, // Needs to be same as orderId to maintain 1 vs 1 realtion integrity
                            CreatedAt = DateTime.Now,
                            CreatedBy = LoggedInUser?.UserName,
                            PianoOrderId = conVm.Orders,
                            VehicleId = conVm.Vehicles,
                            AssignmentNumber = "CON-" + order.OrderNumber
                        };
                        Db.PianoAssignments.Add(consignment);
                        Db.SaveChanges();
                        
                        conVm.Drivers.ForEach(x => {
                            consignment.Drivers.Add(Db.Drivers.FirstOrDefault(y => y.Id == x.Value));
                        });
                        order.PianoAssignmentId = consignment.Id;
                        Db.SaveChanges();

                        int odr = 1;
                        var paths = JsonConvert.DeserializeObject<ConsignmentRouteVm[]>(conVm.Paths);
                        foreach (var path in paths)
                        {
                            Db.PianoAssignmentRoutes.Add(new PianoAssignmentRoute()
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
                        Db.SaveChanges();
                        // Save Trip Statuses
                        Db.TripStatuses.Add(new TripStatus()
                        {
                            Id = Guid.NewGuid(),
                            PianoAssignmentId = consignment.Id,
                            CreatedAt = DateTime.Now,
                            Status = (int)TripStatusEnum.NotStarted,
                            StatusTime = DateTime.Now,
                            StatusBy = LoggedInUser?.UserName
                        });
                        Db.SaveChanges();

                        // Save Piano Statuses
                        foreach (var piano in order.Pianos)
                        {
                            Db.PianoStatuses.Add(new PianoStatus()
                            {
                                Id = Guid.NewGuid(),
                                PianoId = piano.Id,
                                CreatedAt = DateTime.Now,
                                Status = (int)PianoStatusEnum.Booked,
                                StatusTime = DateTime.Now,
                                StatusBy = LoggedInUser?.UserName
                            });
                        } 
                        
                    }
                    else
                    {
                        consignment = Db.PianoAssignments
                            .Include(x => x.Route)
                            .Include(x => x.Drivers)
                            .FirstOrDefault(x => x.Id == order.PianoAssignmentId);

                        consignment.PianoOrderId = conVm.Orders;
                        consignment.VehicleId = conVm.Vehicles;
                        consignment.AssignmentNumber = "CON-" + order.OrderNumber;
                        Db.SaveChanges();

                        consignment.Drivers.ToList().ForEach(x => {
                            consignment.Drivers.Remove(Db.Drivers.FirstOrDefault(y => y.Id == x.Id));
                        });
                        Db.SaveChanges();
                        conVm.Drivers.ForEach(x => {
                            consignment.Drivers.Add(Db.Drivers.FirstOrDefault(y => y.Id == x.Value));
                        });

                        consignment.Route.ToList().ForEach(x => {
                            consignment.Route.Remove(Db.PianoAssignmentRoutes.FirstOrDefault(y => y.Id == x.Id));
                        });
                        Db.SaveChanges();

                        int odr = 1;
                        var paths = JsonConvert.DeserializeObject<ConsignmentRouteVm[]>(conVm.Paths);
                        foreach (var path in paths)
                        {
                            Db.PianoAssignmentRoutes.Add(new PianoAssignmentRoute()
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
                        Db.SaveChanges();

                    }

                    var consignmentDrivers = Db.PianoAssignments
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
                    return RedirectToAction("Index");
                }
            }
           
            return View(conVm);
        }

        public ActionResult PickTickets()
        {
            var consigmentVms = new List<ConsigmentVm>();
            var consignments = Db.PianoAssignments
                .Include(x => x.Drivers)
                .Include(x => x.Vehicle)
                .ToList();//.Where(x => x.CreatedBy == LoggedInUser.Id)
            foreach (var consignment in consignments)
            {
                var order = Db.PianoOrders.FirstOrDefault(x => x.Id == consignment.PianoOrderId);

                var consigmentVm = new ConsigmentVm()
                {
                    Id = consignment.Id,
                    CreatedAt = consignment.CreatedAt.ToString(),
                    OrderId = consignment.PianoOrderId?.ToString(),
                    OrderNumber = order.OrderNumber,
                    ConsignmentNumber = consignment.AssignmentNumber,
                    DriverName = String.Join(",", consignment.Drivers.Select(x => x.Name).ToList()),
                    VehicleName = consignment.Vehicle?.Name,
                };
                consigmentVms.Add(consigmentVm);
            }
            return View(consigmentVms);
        }

        public ActionResult Generate()
        {
            var consignments = Db.PianoAssignments
                .Include(x => x.Drivers)
                .Include(x => x.Vehicle)
                .ToList();//.Where(x => x.CreatedBy == LoggedInUser.Id)

            List<OrderVm> orderVMs = new List<OrderVm>();
            foreach (var consignment in consignments)
            {
                var order = Db.PianoOrders
                    .Include(x => x.PickupAddress)
                    .Include(x => x.DeliveryAddress)
                    .FirstOrDefault(x => x.Id == consignment.PianoOrderId);

                string prefix = "ENCORE";
                var barcode = GeneratorHelper.GenerateBarcode(prefix, int.Parse(order.OrderNumber));
                consignment.PickupTicket = barcode;
                consignment.PickupTicketGenerationTime = DateTime.Now;
                Db.SaveChanges();

                var pickupAddress = TinyMapper.Map<AddressVm>(order.PickupAddress).ToString;
                var deliveryAddress = TinyMapper.Map<AddressVm>(order.DeliveryAddress).ToString;

                var orderVM = new OrderVm()
                {
                    Id = order.Id.ToString(),
                    OrderDate = order.CreatedAt.ToString(),
                    OrderNumber = order.OrderNumber,
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
            var filePath = QRCodeGenerator.Generate(ImagesPath, UploadsPath, orderVMs);
            return File(filePath, "application/octet-stream", "PickupTickets.pdf");

        }

        [HttpPost]
        public ActionResult Send(Guid? id)
        {
            try
            {
                var consignment = Db.PianoAssignments
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
                var order = Db.PianoOrders
                            .Include(x => x.Customer)
                            .Include(x => x.Pianos.Select(y => y.PianoMake))
                            .Include(x => x.Pianos.Select(y => y.PianoType))
                            .Include(x => x.Pianos.Select(y => y.PianoSize))
                            .Include(x => x.PianoAssignment.Drivers)
                            .Include(x => x.PickupAddress)
                            .Include(x => x.DeliveryAddress)
                            .Include(x => x.OrderCharges.Select(y=>y.PianoCharges))
                            .FirstOrDefault(x => x.Id == id);

                string _directoryPath = UploadsPath + "\\InvoiceCodes";

                string html = InvoiceHtmlHelper.GenerateInvoiceHtml(order, _directoryPath);

                JsonResponse Path = HtmlToPdf(html,order.PianoAssignment.AssignmentNumber);
                string pathValue = Path.Result.ToString();

                if (!System.IO.File.Exists(pathValue))
                    return null;
                return File(pathValue, "application/pdf", "Invoice.pdf");

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
                return File(new byte[0] , "application/pdf", "Error.pdf");
            }

         
        }
        public JsonResponse HtmlToPdf(string html , string conNum)
        {
            try
            {
                var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter()
                {
                    Quiet = false,
                };
                //htmlToPdf.WkHtmlToPdfExeName = "wkhtmltopdf.exe";
                htmlToPdf.CustomWkHtmlArgs = " --load-media-error-handling ignore ";
                htmlToPdf.LogReceived += HtmlToPdf_LogReceived;
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

        private void HtmlToPdf_LogReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            string Path = Server.MapPath("~/Uploads/ConsignmentInvoices/log.txt");
            System.IO.File.AppendAllText(Path, e.Data);
        }
    }
}