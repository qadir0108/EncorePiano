using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using Hangfire;
using Nelibur.ObjectMapper;
using WebGrease.Css.Extensions;
using WFP.ICT.Data.Entities;
using WFP.ICT.Enum;
using WFP.ICT.Web.Async;
using WFP.ICT.Web.Models;
using WFP.ICT.Web.ViewModels;

namespace WFP.ICT.Web.Controllers
{
    public class WarehouseController : BaseController
    {
        public ActionResult Index()
        {
            var vms = new List<WarehouseVm>();
            var entities = db.Warehouses
                .Include(x => x.Address)
                .Include(x => x.Inventory)
                .ToList();

            foreach (var entity in entities)
            {
                vms.Add(TinyMapper.Map<WarehouseVm>(entity));
            }

            return View(vms);
        }

        public ActionResult New()
        {
            OrderVm model = new OrderVm()
            {
                Services = ServicesList,
                 //  P1 = new PianoVm() { Id = Guid.NewGuid() },
                //P2 = new PianoVm(),
                //P3 = new PianoVm(),
                //P4 = new PianoVm(),
                //P5 = new PianoVm()
            };
            ViewBag.Customers = new SelectList(CustomersList, "Value", "Text");
            ViewBag.OrderMedium = new SelectList(OrderMediumList, "Value", "Text");
            ViewBag.PaymentOption = new SelectList(PaymentOptionsList, "Value", "Text");
            ViewData["P1.PianoType"] = new SelectList(PianoTypesList, "Value", "Text");
            ViewData["P2.PianoType"] = new SelectList(PianoTypesList, "Value", "Text");
            ViewData["P3.PianoType"] = new SelectList(PianoTypesList, "Value", "Text");
            ViewData["P4.PianoType"] = new SelectList(PianoTypesList, "Value", "Text");
            ViewData["P5.PianoType"] = new SelectList(PianoTypesList, "Value", "Text");
            ViewData["PickupAddress.State"] = new SelectList(States, "Value", "Text");
            ViewData["DeliveryAddress.State"] = new SelectList(States, "Value", "Text");
            return View(model);
        }

        public ActionResult Edit(Guid? id)
        {
            //var order = db.PianoOrders
            //                .Include(x => x.Customer)
            //                .Include(x => x.Pianos)
            //                .Include(x => x.PickupAddress)
            //                .Include(x => x.DeliveryAddress)
            //                .Include(x => x.Services)
            //                .FirstOrDefault(x => x.Id == id);

            //var pickupAddress = new AddressVm(order.PickupAddress);
            //var deliveryAddress = new AddressVm(order.DeliveryAddress);

            //var orderVm = new OrderVm()
            //{
            //    Id = order.Id.ToString(),
            //    OrderDate = order.CreatedAt.ToString(),
            //    OrderNumber = order.OrderNumber,
            //    OrderMedium = ((OrderMediumEnum)order.OrderMedium).ToString(),
            //    CallerFirstName = order.CallerFirstName,
            //    CallerLastName = order.CallerLastName,
            //    CallerPhoneNumber = order.CallerPhoneNumber,
            //    CallerEmail = order.CallerEmail,
            //    PreferredPickupDateTime = order.PreferredPickupDateTime?.ToString(),
            //    Notes = order.Notes,
            //    PickupAddress = pickupAddress,
            //    DeliveryAddress = deliveryAddress,
            //    PickupDate = order.PickupDate?.ToString(),
            //    DeliveryDate = order.DeliveryDate?.ToString(),
            //    Customer = order.Customer != null ? order.Customer.AccountCode + " " + order.Customer.Name : ""
            //};

            //var pianos = order.Pianos.ToList();
            //orderVm.P1 = GetPiano(pianos, 0);
            //orderVm.P2 = GetPiano(pianos, 1);
            //orderVm.P3 = GetPiano(pianos, 2);
            //orderVm.P4 = GetPiano(pianos, 3);
            //orderVm.P5 = GetPiano(pianos, 4);

            //orderVm.Services = ServicesList;
            //foreach (var service in order.Services)
            //{
            //    var serviceVm = orderVm.Services.FirstOrDefault(x => x.ServiceCode == service.ServiceCode.ToString());
            //    serviceVm.IsSelected = true;
            //    serviceVm.ServiceCharges = service.ServiceCharges.ToString();
            //}

            //ViewBag.Customers = new SelectList(CustomersList, "Value", "Text", order.Customer?.Id.ToString());
            //ViewBag.OrderMedium = new SelectList(OrderMediumList, "Value", "Text", order.OrderMedium);
            //ViewBag.PaymentOption = new SelectList(PaymentOptionsList, "Value", "Text", order.PaymentOption);
            //ViewData["P1.PianoType"] = new SelectList(PianoTypesList, "Value", "Text", orderVm.P1?.PianoTypeId);
            //ViewData["P2.PianoType"] = new SelectList(PianoTypesList, "Value", "Text", orderVm.P2?.PianoTypeId);
            //ViewData["P3.PianoType"] = new SelectList(PianoTypesList, "Value", "Text", orderVm.P3?.PianoTypeId);
            //ViewData["P4.PianoType"] = new SelectList(PianoTypesList, "Value", "Text", orderVm.P4?.PianoTypeId);
            //ViewData["P5.PianoType"] = new SelectList(PianoTypesList, "Value", "Text", orderVm.P5?.PianoTypeId);
            //ViewData["PickupAddress.State"] = new SelectList(States, "Value", "Text", orderVm.PickupAddress.State);
            //ViewData["DeliveryAddress.State"] = new SelectList(States, "Value", "Text", orderVm.DeliveryAddress.State);
            return View("New", null);
        }
        
        [HttpPost]
        public ActionResult Save(OrderVm orderVm)
        {
            return null;
            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        List<int> serviceCodes = string.IsNullOrEmpty(Request.Params["ServiceCodes"]) ? new List<int>() :
            //                Request.Params["ServiceCodes"].Split(",".ToArray()).Select(x => int.Parse(x)).ToList();
            //        int[] serviceCharges =
            //            Request.Params["service.ServiceCharges"].Split(",".ToArray()).Select(x => int.Parse(x)).ToArray();

            //        if (string.IsNullOrEmpty(orderVm.Id))
            //        {
            //            int newOrderNumber = db.PianoOrders.Any()
            //                ? db.PianoOrders.ToList().Max(x => int.Parse(x.OrderNumber)) + 1
            //                : 2500;

            //            var orderId = Guid.NewGuid();
            //            var pickupAddressId = Guid.NewGuid();
            //            var deliveryAddressId = Guid.NewGuid();

            //            Address pickupAddress = new Address()
            //            {
            //                Id = pickupAddressId,
            //                CreatedAt = DateTime.Now,
            //                CreatedBy = LoggedInUser?.UserName,
            //                Name = orderVm.PickupAddress.Name,
            //                Address1 = orderVm.PickupAddress.Address1,
            //                Suburb = orderVm.PickupAddress.Suburb,
            //                State = orderVm.PickupAddress.State,
            //                PostCode = orderVm.PickupAddress.PostCode,
            //                PhoneNumber = orderVm.PickupAddress.PhoneNumber
            //            };
            //            db.Addresses.Add(pickupAddress);
            //            db.SaveChanges();

            //            Address deliveryAddress = new Address()
            //            {
            //                Id = deliveryAddressId,
            //                CreatedAt = DateTime.Now,
            //                CreatedBy = LoggedInUser?.UserName,
            //                Name = orderVm.DeliveryAddress.Name,
            //                Address1 = orderVm.DeliveryAddress.Address1,
            //                Suburb = orderVm.DeliveryAddress.Suburb,
            //                State = orderVm.DeliveryAddress.State,
            //                PostCode = orderVm.DeliveryAddress.PostCode,
            //                PhoneNumber = orderVm.DeliveryAddress.PhoneNumber
            //            };
            //            db.Addresses.Add(deliveryAddress);
            //            db.SaveChanges();

            //            var order = new PianoOrder()
            //            {
            //                Id = orderId,
            //                CreatedAt = DateTime.Now,
            //                CreatedBy = LoggedInUser?.UserName,
            //                OrderNumber = newOrderNumber.ToString(),
            //                OrderMedium = int.Parse(orderVm.OrderMedium),
            //                CallerFirstName = orderVm.CallerFirstName,
            //                CallerLastName = orderVm.CallerLastName,
            //                CallerPhoneNumber = orderVm.CallerPhoneNumber,
            //                CallerEmail = orderVm.CallerEmail,
            //                PaymentOption = int.Parse(orderVm.PaymentOption),
            //                PreferredPickupDateTime = string.IsNullOrEmpty(orderVm.PreferredPickupDateTime) ? (DateTime?)null : DateTime.Parse(orderVm.PreferredPickupDateTime),
            //                CustomerId =
            //                    string.IsNullOrEmpty(orderVm.Customers) ? (Guid?)null : Guid.Parse(orderVm.Customers),
            //                Notes = orderVm.Notes,
            //                PickupAddressId = pickupAddressId,
            //                DeliveryAddressId = deliveryAddressId,
            //            };
            //            db.PianoOrders.Add(order);
            //            db.SaveChanges();

            //            InsertPiano(orderVm.P1, orderId);
            //            InsertPiano(orderVm.P2, orderId);
            //            InsertPiano(orderVm.P3, orderId);
            //            InsertPiano(orderVm.P4, orderId);
            //            InsertPiano(orderVm.P5, orderId);
            //            db.SaveChanges();

            //            foreach (var code in serviceCodes)
            //            {
            //                var service =
            //                    db.PianoServices.FirstOrDefault(x => !x.PianoOrderId.HasValue && x.ServiceCode == code);
            //                long serviceCharge = serviceCharges.Length >= (code / 100) ? serviceCharges[(code / 100) - 1] : service.ServiceCharges;
            //                db.PianoServices.Add(new PianoService()
            //                {
            //                    Id = Guid.NewGuid(),
            //                    CreatedAt = DateTime.Now,
            //                    CreatedBy = LoggedInUser?.UserName,
            //                    PianoOrderId = orderId,
            //                    ServiceCode = service.ServiceCode,
            //                    ServiceType = service.ServiceType,
            //                    ServiceDetails = service.ServiceDetails,
            //                    ServiceCharges = serviceCharge,
            //                    //ServiceStatus = service.ServiceStatus
            //                });
            //                db.SaveChanges();
            //            }

            //            BackgroundJob.Enqueue(() => EmailHelper.SendOrderEmailToClient(orderVm));
            //            TempData["Success"] = "Order #: " + order.OrderNumber + " has been saved sucessfully.";
            //            return RedirectToAction("Index");
            //        }
            //        else
            //        {
            //            var order = db.PianoOrders
            //                .Include(x => x.Customer)
            //                .Include(x => x.Pianos)
            //                .Include(x => x.PickupAddress)
            //                .Include(x => x.DeliveryAddress)
            //                .Include(x => x.Services)
            //                .FirstOrDefault(x => x.Id.ToString() == orderVm.Id);

            //            order.OrderMedium = int.Parse(orderVm.OrderMedium);
            //            order.CallerFirstName = orderVm.CallerFirstName;
            //            order.CallerLastName = orderVm.CallerLastName;
            //            order.CallerPhoneNumber = orderVm.CallerPhoneNumber;
            //            order.CallerEmail = orderVm.CallerEmail;
            //            order.PaymentOption = int.Parse(orderVm.PaymentOption);
            //            order.PreferredPickupDateTime = string.IsNullOrEmpty(orderVm.PreferredPickupDateTime)
            //                ? (DateTime?)null
            //                : DateTime.Parse(orderVm.PreferredPickupDateTime);
            //            order.CustomerId =
            //                string.IsNullOrEmpty(orderVm.Customers) ? (Guid?)null : Guid.Parse(orderVm.Customers);
            //            order.Notes = orderVm.Notes;

            //            order.PickupAddress.Name = orderVm.PickupAddress.Name;
            //            order.PickupAddress.Address1 = orderVm.PickupAddress.Address1;
            //            order.PickupAddress.Suburb = orderVm.PickupAddress.Suburb;
            //            order.PickupAddress.State = orderVm.PickupAddress.State;
            //            order.PickupAddress.PostCode = orderVm.PickupAddress.PostCode;
            //            order.PickupAddress.PhoneNumber = orderVm.PickupAddress.PhoneNumber;
            //            order.DeliveryAddress.Name = orderVm.DeliveryAddress.Name;
            //            order.DeliveryAddress.Address1 = orderVm.DeliveryAddress.Address1;
            //            order.DeliveryAddress.Suburb = orderVm.DeliveryAddress.Suburb;
            //            order.DeliveryAddress.State = orderVm.DeliveryAddress.State;
            //            order.DeliveryAddress.PostCode = orderVm.DeliveryAddress.PostCode;
            //            order.DeliveryAddress.PhoneNumber = orderVm.DeliveryAddress.PhoneNumber;
            //            db.SaveChanges();

            //            foreach (var piano in order.Pianos.ToList())
            //                db.Pianos.Remove(piano);
            //            InsertPiano(orderVm.P1, order.Id);
            //            InsertPiano(orderVm.P2, order.Id);
            //            InsertPiano(orderVm.P3, order.Id);
            //            InsertPiano(orderVm.P4, order.Id);
            //            InsertPiano(orderVm.P5, order.Id);
            //            db.SaveChanges();

            //            foreach (var service in order.Services.ToList())
            //                db.PianoServices.Remove(service);
            //            foreach (var code in serviceCodes)
            //            {
            //                var service =
            //                    db.PianoServices.FirstOrDefault(x => !x.PianoOrderId.HasValue && x.ServiceCode == code);
            //                long serviceCharge = serviceCharges.Length >= (code / 100) ? serviceCharges[(code / 100) - 1] : service.ServiceCharges;
            //                db.PianoServices.Add(new PianoService()
            //                {
            //                    Id = Guid.NewGuid(),
            //                    CreatedAt = DateTime.Now,
            //                    CreatedBy = LoggedInUser?.UserName,
            //                    PianoOrderId = order.Id,
            //                    ServiceCode = service.ServiceCode,
            //                    ServiceType = service.ServiceType,
            //                    ServiceDetails = service.ServiceDetails,
            //                    ServiceCharges = serviceCharge,
            //                    //ServiceStatus = service.ServiceStatus
            //                });
            //                db.SaveChanges();
            //            }

            //            TempData["Success"] = "Order #: " + order.OrderNumber + " has been updated sucessfully.";
            //            return RedirectToAction("Index");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        TempData["Error"] = "This is error while submitting request." + ex.Message;
            //    }
            //}

            //orderVm.Services = ServicesList;
            //ViewBag.Customers = new SelectList(CustomersList, "Value", "Text");
            //ViewBag.OrderMedium = new SelectList(OrderMediumList, "Value", "Text");
            //ViewBag.PaymentOption = new SelectList(PaymentOptionsList, "Value", "Text");
            //ViewData["P1.PianoType"] = new SelectList(PianoTypesList, "Value", "Text", orderVm.P1?.PianoTypeId);
            //ViewData["P2.PianoType"] = new SelectList(PianoTypesList, "Value", "Text", orderVm.P2?.PianoTypeId);
            //ViewData["P3.PianoType"] = new SelectList(PianoTypesList, "Value", "Text", orderVm.P3?.PianoTypeId);
            //ViewData["P4.PianoType"] = new SelectList(PianoTypesList, "Value", "Text", orderVm.P4?.PianoTypeId);
            //ViewData["P5.PianoType"] = new SelectList(PianoTypesList, "Value", "Text", orderVm.P5?.PianoTypeId);
            //ViewData["PickupAddress.State"] = new SelectList(States, "Value", "Text", orderVm.PickupAddress.State);
            //ViewData["DeliveryAddress.State"] = new SelectList(States, "Value", "Text", orderVm.DeliveryAddress.State);
            //return View("New", orderVm);
        }

        [HttpPost]
        public ActionResult Delete(Guid? id)
        {
            try
            {
                //var order = db.PianoOrders
                //    .Include(x => x.Pianos)
                //    .Include(x => x.PickupAddress)
                //    .Include(x => x.DeliveryAddress)
                //    .Include(x => x.Services)
                //    .FirstOrDefault(x => x.Id.ToString() == Id);

                //foreach (var piano in order.Pianos.ToList())
                //    db.Pianos.Remove(piano);
                //foreach (var service in order.Services.ToList())
                //    db.PianoServices.Remove(service);

                //db.PianoOrders.Remove(order);
                //db.SaveChanges();
                return Json(new JsonResponse() { IsSucess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAddress(Guid? id)
        {
            try
            {
                var warehouse = db.Warehouses.Include(x => x.Address).FirstOrDefault(x => x.Id == id);
                var warehouseAddressVm = TinyMapper.Map<AddressVm>(warehouse.Address);
                return Json(new JsonResponse() { IsSucess = true, Result = warehouseAddressVm.ToJson() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}