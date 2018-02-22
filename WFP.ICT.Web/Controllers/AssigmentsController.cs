using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using Hangfire;
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
using System.Text;
using Microsoft.Practices.ObjectBuilder2;
using WebGrease.Css.Extensions;

namespace WFP.ICT.Web.Controllers
{
    //[Authorize]
    public class AssignmentsController : BaseController
    {
        int pageSize = 15;

        public ActionResult Index()
        {
            var consigmentVms = new List<ConsigmentVm>();
            var consignments = Db.Assignments
                .Include(x => x.Leg)
                .Include(x => x.Drivers)
                .Include(x => x.Vehicle)
                .Include(x => x.Statuses)
                .ToList();//.Where(x => x.CreatedBy == LoggedInUser.Id)
            foreach (var consignment in consignments)
            {
                var order = Db.Orders.Include(x => x.Pianos).FirstOrDefault(x => x.Id == consignment.OrderId);

                var consigmentVm = new ConsigmentVm()
                {
                    Id = consignment.Id,
                    LegDate = consignment.Leg.LegDate?.ToString(StringConstants.DateFormatSlashes),
                    OrderId = consignment.OrderId?.ToString(),
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
            var order = Db.Orders.Include(x => x.Pianos).FirstOrDefault(x => x.OrderNumber == orderNumber);

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
                   PianoType = x.PianoTypeId.ToString(),
                   PianoSize = x.PianoSizeId.ToString(),
                   PianoMake = x.PianoMakeId.ToString(),
                   PianoFinish = x.PianoFinishId.ToString(),
                   PianoModel = x.Model,
                   SerialNumber = x.SerialNumber,
                   IsBench = x.IsBench,
                   IsBoxed = x.IsBoxed,
                   IsPlayer = x.IsPlayer,
                   Notes = x.Notes,
               }).ToList();

            orderVM.Pianos.ForEach(x =>
            {
                if (!string.IsNullOrEmpty(x.PianoType))
                {
                    x.PianoType = Db.PianoTypes.Where(y => y.Id.ToString() == x.PianoType).FirstOrDefault().Type;
                }

                if (!string.IsNullOrEmpty(x.PianoMake))
                {
                    x.PianoMake = Db.PianoMake.Where(y => y.Id.ToString() == x.PianoMake).FirstOrDefault().Name;
                }

                if (!string.IsNullOrEmpty(x.PianoSize))
                {
                    x.PianoSize = Db.PianoSize.Where(y => y.Id.ToString() == x.PianoSize).FirstOrDefault().Width.ToString();
                }
                if (!string.IsNullOrEmpty(x.PianoFinish))
                {
                    x.PianoFinish = Db.PianoFinish.Where(y => y.Id.ToString() == x.PianoFinish).FirstOrDefault().Name;
                }

                var pickupProof = Db.Proofs.Include(y => y.Pictures).FirstOrDefault(y => y.PianoId == x.Id && y.ProofType == (int)ProofTypeEnum.Pickup);
                if (pickupProof == null) return;

                x.Pictures = GetPianoImage(pickupProof.Pictures);
                x.IsMainUnitLoaded = pickupProof.IsMainUnitLoaded ? "Yes" : "No";
                x.PickerName = pickupProof.PickerName;
                x.PickerSignature = GetSignatureImage(pickupProof.PickerSignature);
                x.LoadTimeStamp = pickupProof.LoadTime?.ToString(StringConstants.TimeStampFormatSlashes);
                x.AdditionalBench1Status = System.Enum.GetName(typeof(AdditionalItemStatus), pickupProof.AdditionalBench1Status);
                x.AdditionalBench2Status = System.Enum.GetName(typeof(AdditionalItemStatus), pickupProof.AdditionalBench2Status);
                x.AdditionalCasterCupsStatus = System.Enum.GetName(typeof(AdditionalItemStatus), pickupProof.AdditionalCasterCupsStatus);
                x.AdditionalCoverStatus = System.Enum.GetName(typeof(AdditionalItemStatus), pickupProof.AdditionalCoverStatus);
                x.AdditionalLampStatus = System.Enum.GetName(typeof(AdditionalItemStatus), pickupProof.AdditionalLampStatus);
                x.AdditionalOwnersManualStatus = System.Enum.GetName(typeof(AdditionalItemStatus), pickupProof.AdditionalOwnersManualStatus);
                x.AdditionalMisc1Status = System.Enum.GetName(typeof(AdditionalItemStatus), pickupProof.AdditionalMisc1Status);
                x.AdditionalMisc2Status = System.Enum.GetName(typeof(AdditionalItemStatus), pickupProof.AdditionalMisc2Status);
                x.AdditionalMisc3Status = System.Enum.GetName(typeof(AdditionalItemStatus), pickupProof.AdditionalMisc3Status);

            });

            return View(orderVM);
        }

        public string GetPianoImage(ICollection<PianoPicture> Pictures)
        {
            StringBuilder builder = new StringBuilder();
            if (Pictures.Count > 0)
            {
                Pictures.ToList().ForEach(x =>
                {
                    builder.AppendFormat("<a href='../Images/Piano/{0}' data-lightbox='piano-images'>", x.Picture);
                    builder.AppendFormat("<image  class='grid-image' src='../Images/Piano/{0}'/>", x.Picture);
                    builder.Append("</a>");
                });
            }
            else
            {
                builder.Append("No image available");
            }
            return builder.ToString();
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
                var order = Db.Orders
                    .Include(x => x.Client)
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
                    Customer = order.Client != null ? order.Client.AccountCode + " " + order.Client.Name : ""
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
                    var order = Db.Orders
                        .Include(x => x.Pianos)
                        .Include(x => x.Legs)
                        .Include(x => x.Assignments)
                        .FirstOrDefault(x => x.Id == conVm.Orders);

                    if (order.Assignments.Count > 0) throw new Exception(" Assignment already done.");

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
                    Db.SaveChanges();

                    // Legs
                    var legs = order.Legs.OrderBy(x => x.LegNumber).ToList();
                    string assignmentNumber = $"CON-{order.OrderNumber}";

                    foreach (var l in order.Legs)
                    {
                        var leg = GetLeg(Db, l.Id);
                        if(legs.Count > 1)
                            assignmentNumber = $"CON-{order.OrderNumber}/{leg.LegNumber}";

                        Assignment assignment = new Assignment()
                        {
                            //Id = conVm.Orders.Value, //  Needs to be same as orderId to maintain 1 vs 1 realtion integrity
                            Id = leg.Id,
                            CreatedAt = DateTime.Now,
                            CreatedBy = LoggedInUser?.UserName,
                            OrderId = conVm.Orders,
                            VehicleId = conVm.Vehicles,
                            AssignmentNumber = assignmentNumber,
                            LegId = leg.Id
                        };

                        conVm.Drivers.ForEach(x =>
                        {
                            assignment.Drivers.Add(Db.Drivers.FirstOrDefault(y => y.Id == x.Value));
                        });
                        order.Assignments.Add(assignment);
                        Db.SaveChanges();

                        int odr = 1;
                        var paths = JsonConvert.DeserializeObject<ConsignmentRouteVm[]>(conVm.Paths);
                        foreach (var path in paths)
                        {
                            Db.AssignmentRoutes.Add(new AssignmentRoute()
                            {
                                Id = Guid.NewGuid(),
                                CreatedAt = DateTime.Now,
                                AssignmentId = assignment.Id,
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
                            AssignmentId = assignment.Id,
                            CreatedAt = DateTime.Now,
                            Status = (int)TripStatusEnum.NotStarted,
                            StatusTime = DateTime.Now,
                            StatusBy = LoggedInUser?.UserName
                        });
                        Db.SaveChanges();
                    }

                    //else
                    //{
                    //    assignment = Db.Assignments
                    //        .Include(x => x.Route)
                    //        .Include(x => x.Drivers)
                    //        .FirstOrDefault(x => x.Id == order.AssignmentId);

                    //    assignment.OrderId = conVm.Orders;
                    //    assignment.VehicleId = conVm.Vehicles;
                    //    assignment.AssignmentNumber = "CN-" + order.OrderNumber;
                    //    Db.SaveChanges();

                    //    assignment.Drivers.ToList().ForEach(x => {
                    //        assignment.Drivers.Remove(Db.Drivers.FirstOrDefault(y => y.Id == x.Id));
                    //    });
                    //    Db.SaveChanges();
                    //    conVm.Drivers.ForEach(x => {
                    //        assignment.Drivers.Add(Db.Drivers.FirstOrDefault(y => y.Id == x.Value));
                    //    });

                    //    assignment.Route.ToList().ForEach(x => {
                    //        assignment.Route.Remove(Db.AssignmentRoutes.FirstOrDefault(y => y.Id == x.Id));
                    //    });
                    //    Db.SaveChanges();

                    //    int odr = 1;
                    //    var paths = JsonConvert.DeserializeObject<ConsignmentRouteVm[]>(conVm.Paths);
                    //    foreach (var path in paths)
                    //    {
                    //        Db.AssignmentRoutes.Add(new AssignmentRoute()
                    //        {
                    //            Id = Guid.NewGuid(),
                    //            CreatedAt = DateTime.Now,
                    //            AssignmentId = assignment.Id,
                    //            Order = odr,
                    //            Lat = path.Lat,
                    //            Lng = path.Lng
                    //        });
                    //        odr++;
                    //    }
                    //    Db.SaveChanges();

                    //}

                    //var consignmentDrivers = Db.Assignments
                    //                          .Include(x => x.Drivers)
                    //                          .Where(x => x.Id == assignment.Id);

                    //consignmentDrivers.Drivers.ToList().ForEach(x =>
                    //{
                    //    BackgroundJob.Enqueue(() => FCMUitlity.SendConsignment(x.FCMToken, assignment.Id.ToString(), assignment.AssignmentNumber));
                    //});

                    TempData["Success"] = $"Assignment #: {order.OrderNumber} has been saved sucessfully.";
                    return RedirectToAction("Index");

                }
                catch (Exception ex)
                {
                    TempData["Error"] = "This is error while creating assignment." + ex.Message;
                    return RedirectToAction("Index");
                }
            }
           
            return View(conVm);
        }

        private Leg GetLeg(WFPICTContext db, Guid id)
        {
            return db.Legs
                .Include(x => x.FromLocation)
                .Include(x => x.ToLocation)
                .FirstOrDefault(x => x.Id == id);
        }

        public ActionResult PickTickets()
        {
            var consigmentVms = new List<ConsigmentVm>();
            var consignments = Db.Assignments
                .Include(x => x.Drivers)
                .Include(x => x.Vehicle)
                .ToList();//.Where(x => x.CreatedBy == LoggedInUser.Id)
            foreach (var consignment in consignments)
            {
                var order = Db.Orders.FirstOrDefault(x => x.Id == consignment.OrderId);

                var consigmentVm = new ConsigmentVm()
                {
                    Id = consignment.Id,
                    LegDate = consignment.CreatedAt.ToString(),
                    OrderId = consignment.OrderId?.ToString(),
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
            var consignments = Db.Assignments
                .Include(x => x.Drivers)
                .Include(x => x.Vehicle)
                .ToList();//.Where(x => x.CreatedBy == LoggedInUser.Id)

            List<OrderVm> orderVMs = new List<OrderVm>();
            foreach (var consignment in consignments)
            {
                var order = Db.Orders
                    .Include(x => x.PickupAddress)
                    .Include(x => x.DeliveryAddress)
                    .FirstOrDefault(x => x.Id == consignment.OrderId);

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
                    Customer = order.Client != null ? order.Client.AccountCode + " " + order.Client.Name : "",
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
                var consignment = Db.Assignments
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

        [HttpPost]
        public ActionResult Delete(Guid? id)
        {
            try
            {
                var assignment = Db.Assignments
                            .Include(x => x.Drivers)
                            .Include(x => x.Route)
                            .Include(x => x.Statuses)
                            .Include(x => x.Order)
                            .Include(x => x.Order.Pianos)
                            .Include(x => x.Order.Pianos.Select(y => y.Pictures))
                            .Include(x => x.Proofs)
                            .FirstOrDefault(x => x.Id == id);

                var pianoIds = assignment.Order.Pianos.Select(y => y.Id).ToList();
                var pianoStatuses = Db.PianoStatuses.Where(x => pianoIds.Contains(x.PianoId.Value)).ToList();
                pianoStatuses.ForEach(x =>
                {
                    Db.PianoStatuses.Remove(x);
                });
                Db.SaveChanges();

                assignment.Statuses.ToList().ForEach(x =>
                {
                    assignment.Statuses.Remove(Db.TripStatuses.FirstOrDefault(y => y.Id == x.Id));
                });
                Db.SaveChanges();
                assignment.Drivers.ToList().ForEach(x =>
                {
                    assignment.Drivers.Remove(Db.Drivers.FirstOrDefault(y => y.Id == x.Id));
                });
                Db.SaveChanges();
                assignment.Route.ToList().ForEach(x =>
                {
                    assignment.Route.Remove(Db.AssignmentRoutes.FirstOrDefault(y => y.Id == x.Id));
                });
                Db.SaveChanges();
                
                assignment.Order.Pianos.ToList().ForEach(x =>
                {
                    assignment.Statuses.Remove(Db.TripStatuses.FirstOrDefault(y => y.Id == x.Id));
                });
                Db.SaveChanges();

                assignment.Order.Pianos.ToList().ForEach(x =>
                {
                    x.Pictures.ToList().ForEach(y => Db.PianoPictures.Remove(y));
                    Db.SaveChanges();

                    var proofs = Db.Proofs.Include(zz=> zz.Pictures).Where(zz => zz.PianoId == x.Id).ToList();
                    proofs.ToList().ForEach(p =>
                    {
                        p.Pictures.ToList().ForEach(y => Db.PianoPictures.Remove(y));
                        Db.SaveChanges();
                    });
                    Db.SaveChanges();

                    Db.Proofs.RemoveRange(proofs);
                    Db.SaveChanges();
                });
                Db.SaveChanges();

                assignment.Proofs.ToList().ForEach(x =>
                {
                    assignment.Proofs.Remove(Db.Proofs.FirstOrDefault(y => y.Id == x.Id));
                });
                Db.SaveChanges();

                Db.Assignments.Remove(assignment);
                Db.SaveChanges();
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
                var order = Db.Orders
                            .Include(x => x.Client)
                            .Include(x => x.Pianos.Select(y => y.PianoMake))
                            .Include(x => x.Pianos.Select(y => y.PianoType))
                            .Include(x => x.Pianos.Select(y => y.PianoSize))
                            .Include(x => x.Assignments.Select(y => y.Drivers))
                            .Include(x => x.PickupAddress)
                            .Include(x => x.DeliveryAddress)
                            .Include(x => x.OrderCharges.Select(y=>y.PianoCharges))
                            .FirstOrDefault(x => x.Id == id);

                string _directoryPath = UploadsPath + "\\InvoiceCodes";

                string html = InvoiceHtmlHelper.GenerateInvoiceHtml(order, _directoryPath);

                JsonResponse Path = HtmlToPdf(html,order.OrderNumber);
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