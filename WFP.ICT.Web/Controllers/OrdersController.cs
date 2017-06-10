using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using Hangfire;
using Nelibur.ObjectMapper;
using WFP.ICT.Data.Entities;
using WFP.ICT.Web.Models;
using WFP.ICT.Enum;
using WFP.ICT.Enums;
using WFP.ICT.Web.Async;

namespace WFP.ICT.Web.Controllers
{
    //[Authorize]
    public class OrdersController : BaseController
    {
        public ActionResult Index()
        {
            var orderVMs = new List<OrderVm>();
            var orders = db.PianoOrders
                .Include(x => x.Customer)
                .Include(x => x.Pianos)
                .Include(x => x.PickupAddress)
                .Include(x => x.DeliveryAddress)
                .Include(x => x.Services)
                .ToList();
            foreach (var order in orders)
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

        public ActionResult New()
        {
            OrderVm model = new OrderVm()
            {
                Services = ServicesList,
                P1 = new PianoVm() {Id =  Guid.NewGuid()},
                P2 = new PianoVm(),
                P3 = new PianoVm(),
                P4 = new PianoVm(),
                P5 = new PianoVm()
            };
            ViewBag.Customers = new SelectList(CustomersList, "Value", "Text");
            ViewBag.OrderMedium = new SelectList(OrderMediumList, "Value","Text");
            ViewBag.OrderType = new SelectList(OrderTypeList, "Value", "Text");
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
            var order = db.PianoOrders
                            .Include(x => x.Customer)
                            .Include(x => x.Pianos)
                            .Include(x => x.PickupAddress)
                            .Include(x => x.DeliveryAddress)
                            .Include(x => x.Services)
                            .FirstOrDefault(x => x.Id == id);

            var pickupAddress = TinyMapper.Map<AddressVm>(order.PickupAddress);
            var deliveryAddress = TinyMapper.Map<AddressVm>(order.DeliveryAddress);

            var orderVm = new OrderVm()
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
                PreferredPickupDateTime = order.PreferredPickupDateTime?.ToString(),
                Notes = order.Notes,
                PickupAddress = pickupAddress,
                DeliveryAddress = deliveryAddress,
                PickupDate = order.PickupDate?.ToString(),
                DeliveryDate = order.DeliveryDate?.ToString(),
                Customer = order.Customer != null ? order.Customer.AccountCode + " " + order.Customer.Name : ""
            };

            var pianos = order.Pianos.ToList();
            orderVm.P1 = GetPiano(pianos, 0);
            orderVm.P2 = GetPiano(pianos, 1);
            orderVm.P3 = GetPiano(pianos, 2);
            orderVm.P4 = GetPiano(pianos, 3);
            orderVm.P5 = GetPiano(pianos, 4);

            orderVm.Services = ServicesList;
            foreach (var service in order.Services)
            {
                var serviceVm = orderVm.Services.FirstOrDefault(x => x.ServiceCode == service.ServiceCode.ToString());
                serviceVm.IsSelected = true;
                serviceVm.ServiceCharges = service.ServiceCharges.ToString();
            }

            ViewBag.Customers = new SelectList(CustomersList, "Value", "Text", order.Customer?.Id.ToString());
            ViewBag.OrderType = new SelectList(OrderTypeList, "Value", "Text", order.OrderType);
            ViewBag.OrderMedium = new SelectList(OrderMediumList, "Value", "Text", order.OrderMedium);
            ViewBag.PaymentOption = new SelectList(PaymentOptionsList, "Value", "Text", order.PaymentOption);
            ViewData["P1.PianoType"] = new SelectList(PianoTypesList, "Value", "Text", orderVm.P1?.PianoTypeId);
            ViewData["P2.PianoType"] = new SelectList(PianoTypesList, "Value", "Text", orderVm.P2?.PianoTypeId);
            ViewData["P3.PianoType"] = new SelectList(PianoTypesList, "Value", "Text", orderVm.P3?.PianoTypeId);
            ViewData["P4.PianoType"] = new SelectList(PianoTypesList, "Value", "Text", orderVm.P4?.PianoTypeId);
            ViewData["P5.PianoType"] = new SelectList(PianoTypesList, "Value", "Text", orderVm.P5?.PianoTypeId);
            ViewData["PickupAddress.State"] = new SelectList(States, "Value", "Text", orderVm.PickupAddress.State);
            ViewData["DeliveryAddress.State"] = new SelectList(States, "Value", "Text", orderVm.DeliveryAddress.State);
            return View("New", orderVm);
        }

        private PianoVm GetPiano(List<Piano> pianos, int number)
        {
            if (number >= pianos.Count) return new PianoVm();
            var x = pianos.ElementAt(number);
            return new PianoVm()
            {
                Id = x.Id,
                //OrderId = order.Id,
                PianoTypeId = x.PianoTypeId.ToString(),
                PianoType = PianoTypesList.FirstOrDefault(y => y.Value == x.PianoTypeId.ToString()).Text,
                PianoName = x.Name,
                PianoColor = x.Color,
                PianoModel = x.Model,
                PianoMake = x.Make,
                SerialNumber = x.SerialNumber,
                IsBench = x.IsBench,
                IsBoxed = x.IsBoxed,
                IsStairs = x.IsStairs
            };
        }

        [HttpPost]
        public ActionResult Save(OrderVm orderVm)
        {
            //return null;
            if (ModelState.IsValid)
            {
                try
                {
                    List<int> serviceCodes = string.IsNullOrEmpty(Request.Params["ServiceCodes"]) ? new List<int>() :
                            Request.Params["ServiceCodes"].Split(",".ToArray()).Select(x => int.Parse(x)).ToList();
                    int[] serviceCharges =
                        Request.Params["service.ServiceCharges"].Split(",".ToArray()).Select(x => int.Parse(x)).ToArray();

                    if (string.IsNullOrEmpty(orderVm.Id))
                    {
                        int newOrderNumber = db.PianoOrders.Any()
                            ? db.PianoOrders.ToList().Max(x => int.Parse(x.OrderNumber)) + 1
                            : 2500;

                        var orderId = Guid.NewGuid();
                        var pickupAddressId = Guid.NewGuid();
                        var deliveryAddressId = Guid.NewGuid();

                        Address pickupAddress = new Address()
                        {
                            Id = pickupAddressId,
                            CreatedAt = DateTime.Now,
                            CreatedBy = LoggedInUser?.UserName,
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
                            CreatedBy = LoggedInUser?.UserName,
                            Name = orderVm.DeliveryAddress.Name,
                            Address1 = orderVm.DeliveryAddress.Address1,
                            Suburb = orderVm.DeliveryAddress.Suburb,
                            State = orderVm.DeliveryAddress.State,
                            PostCode = orderVm.DeliveryAddress.PostCode,
                            PhoneNumber = orderVm.DeliveryAddress.PhoneNumber
                        };
                        db.Addresses.Add(deliveryAddress);
                        db.SaveChanges();

                        var order = new PianoOrder()
                        {
                            Id = orderId,
                            CreatedAt = DateTime.Now,
                            CreatedBy = LoggedInUser?.UserName,
                            OrderNumber = newOrderNumber.ToString(),
                            OrderType = int.Parse(orderVm.OrderType),
                            OrderMedium = int.Parse(orderVm.OrderMedium),
                            CallerFirstName = orderVm.CallerFirstName,
                            CallerLastName = orderVm.CallerLastName,
                            CallerPhoneNumber = orderVm.CallerPhoneNumber,
                            CallerEmail = orderVm.CallerEmail,
                            PaymentOption = int.Parse(orderVm.PaymentOption),
                            PreferredPickupDateTime = string.IsNullOrEmpty(orderVm.PreferredPickupDateTime) ? (DateTime?)null : DateTime.Parse(orderVm.PreferredPickupDateTime),
                            CustomerId =
                                string.IsNullOrEmpty(orderVm.Customers) ? (Guid?) null : Guid.Parse(orderVm.Customers),
                            Notes = orderVm.Notes,
                            PickupAddressId = pickupAddressId,
                            DeliveryAddressId = deliveryAddressId,
                        };
                        db.PianoOrders.Add(order);
                        db.SaveChanges();

                        InsertPiano(orderVm.P1, orderId);
                        InsertPiano(orderVm.P2, orderId);
                        InsertPiano(orderVm.P3, orderId);
                        InsertPiano(orderVm.P4, orderId);
                        InsertPiano(orderVm.P5, orderId);
                        db.SaveChanges();

                        foreach (var code in serviceCodes)
                        {
                            var service =
                                db.PianoServices.FirstOrDefault(x => !x.PianoOrderId.HasValue && x.ServiceCode == code);
                            long serviceCharge = serviceCharges.Length >= (code / 100) ? serviceCharges[(code / 100) - 1] : service.ServiceCharges;
                            db.PianoServices.Add(new PianoService()
                            {
                                Id = Guid.NewGuid(),
                                CreatedAt = DateTime.Now,
                                CreatedBy = LoggedInUser?.UserName,
                                PianoOrderId = orderId,
                                ServiceCode = service.ServiceCode,
                                ServiceType = service.ServiceType,
                                ServiceDetails = service.ServiceDetails,
                                ServiceCharges = serviceCharge,
                                //ServiceStatus = service.ServiceStatus
                            });
                            db.SaveChanges();
                        }

                        BackgroundJob.Enqueue(() => EmailHelper.SendOrderEmailToClient(orderVm));
                        TempData["Success"] = "Order #: " + order.OrderNumber + " has been saved sucessfully.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var order = db.PianoOrders
                            .Include(x => x.Customer)
                            .Include(x => x.Pianos)
                            .Include(x => x.PickupAddress)
                            .Include(x => x.DeliveryAddress)
                            .Include(x => x.Services)
                            .FirstOrDefault(x => x.Id.ToString() == orderVm.Id);

                        order.OrderType = int.Parse(orderVm.OrderType);
                        order.OrderMedium = int.Parse(orderVm.OrderMedium);
                        order.CallerFirstName = orderVm.CallerFirstName;
                        order.CallerLastName = orderVm.CallerLastName;
                        order.CallerPhoneNumber = orderVm.CallerPhoneNumber;
                        order.CallerEmail = orderVm.CallerEmail;
                        order.PaymentOption = int.Parse(orderVm.PaymentOption);
                        order.PreferredPickupDateTime = string.IsNullOrEmpty(orderVm.PreferredPickupDateTime)
                            ? (DateTime?)null
                            : DateTime.Parse(orderVm.PreferredPickupDateTime);
                        order.CustomerId =
                            string.IsNullOrEmpty(orderVm.Customers) ? (Guid?) null : Guid.Parse(orderVm.Customers);
                        order.Notes = orderVm.Notes;

                        order.PickupAddress.Name = orderVm.PickupAddress.Name;
                        order.PickupAddress.Address1 = orderVm.PickupAddress.Address1;
                        order.PickupAddress.Suburb = orderVm.PickupAddress.Suburb;
                        order.PickupAddress.State = orderVm.PickupAddress.State;
                        order.PickupAddress.PostCode = orderVm.PickupAddress.PostCode;
                        order.PickupAddress.PhoneNumber = orderVm.PickupAddress.PhoneNumber;
                        order.DeliveryAddress.Name = orderVm.DeliveryAddress.Name;
                        order.DeliveryAddress.Address1 = orderVm.DeliveryAddress.Address1;
                        order.DeliveryAddress.Suburb = orderVm.DeliveryAddress.Suburb;
                        order.DeliveryAddress.State = orderVm.DeliveryAddress.State;
                        order.DeliveryAddress.PostCode = orderVm.DeliveryAddress.PostCode;
                        order.DeliveryAddress.PhoneNumber = orderVm.DeliveryAddress.PhoneNumber;
                        db.SaveChanges();
                        
                        foreach (var piano in order.Pianos.ToList())
                            db.Pianos.Remove(piano);
                        InsertPiano(orderVm.P1, order.Id);
                        InsertPiano(orderVm.P2, order.Id);
                        InsertPiano(orderVm.P3, order.Id);
                        InsertPiano(orderVm.P4, order.Id);
                        InsertPiano(orderVm.P5, order.Id);
                        db.SaveChanges();
                        
                        foreach (var service in order.Services.ToList())
                            db.PianoServices.Remove(service);
                        foreach (var code in serviceCodes)
                        {
                            var service =
                                db.PianoServices.FirstOrDefault(x => !x.PianoOrderId.HasValue && x.ServiceCode == code);
                            long serviceCharge = serviceCharges.Length >= (code / 100) ? serviceCharges[(code/100) - 1] : service.ServiceCharges;
                            db.PianoServices.Add(new PianoService()
                            {
                                Id = Guid.NewGuid(),
                                CreatedAt = DateTime.Now,
                                CreatedBy = LoggedInUser?.UserName,
                                PianoOrderId = order.Id,
                                ServiceCode = service.ServiceCode,
                                ServiceType = service.ServiceType,
                                ServiceDetails = service.ServiceDetails,
                                ServiceCharges = serviceCharge,
                                //ServiceStatus = service.ServiceStatus
                            });
                            db.SaveChanges();
                        }

                        TempData["Success"] = "Order #: " + order.OrderNumber + " has been updated sucessfully.";
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "This is error while submitting request." + ex.Message;
                }
            }

            orderVm.Services = ServicesList;
            ViewBag.Customers = new SelectList(CustomersList, "Value", "Text");
            ViewBag.OrderMedium = new SelectList(OrderMediumList, "Value", "Text");
            ViewBag.PaymentOption = new SelectList(PaymentOptionsList, "Value", "Text");
            ViewData["P1.PianoType"] = new SelectList(PianoTypesList, "Value", "Text", orderVm.P1?.PianoTypeId);
            ViewData["P2.PianoType"] = new SelectList(PianoTypesList, "Value", "Text", orderVm.P2?.PianoTypeId);
            ViewData["P3.PianoType"] = new SelectList(PianoTypesList, "Value", "Text", orderVm.P3?.PianoTypeId);
            ViewData["P4.PianoType"] = new SelectList(PianoTypesList, "Value", "Text", orderVm.P4?.PianoTypeId);
            ViewData["P5.PianoType"] = new SelectList(PianoTypesList, "Value", "Text", orderVm.P5?.PianoTypeId);
            ViewData["PickupAddress.State"] = new SelectList(States, "Value", "Text", orderVm.PickupAddress.State);
            ViewData["DeliveryAddress.State"] = new SelectList(States, "Value", "Text", orderVm.DeliveryAddress.State);
            return View("New", orderVm);
        }

        private void InsertPiano(PianoVm vm, Guid orderId)
        {
            if (string.IsNullOrEmpty(vm.PianoName) || string.IsNullOrEmpty(vm.PianoName.Trim())) return;

            db.Pianos.Add(new Piano()
            {
                Id = Guid.NewGuid(),
                OrderId= orderId,
                CreatedAt = DateTime.Now,
                CreatedBy = LoggedInUser?.UserName,
                Name = vm.PianoName,
                PianoTypeId = Guid.Parse(vm.PianoType),
                Color = vm.PianoColor,
                Model = vm.PianoModel,
                Make = vm.PianoMake,
                SerialNumber = vm.SerialNumber,
                IsBench = vm.IsBench,
                IsBoxed = vm.IsBoxed,
                IsStairs = vm.IsStairs
            });
        }

        [HttpPost]
        public ActionResult Delete(Guid? id)
        {
            try
            {
                var order = db.PianoOrders
                    .Include(x => x.Pianos)
                    .Include(x => x.PickupAddress)
                    .Include(x => x.DeliveryAddress)
                    .Include(x => x.Services)
                    .FirstOrDefault(x => x.Id == id);

                foreach (var piano in order.Pianos.ToList())
                    db.Pianos.Remove(piano);
                foreach (var service in order.Services.ToList())
                    db.PianoServices.Remove(service);

                db.PianoOrders.Remove(order);
                db.SaveChanges();
                return Json(new JsonResponse() { IsSucess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}