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
    public class OrdersController : BaseController
    {
        // GET: Orders
        public ActionResult Index()
        {
            //SMSHelper.Send("+923216334272", "Hello, Kamran");
            var orderVMs = new List<OrderVm>();
            var orders = db.PianoOrders
                .Include(x => x.Customer)
                .Include(x => x.Pianos)
                .Include(x => x.PickupAddress)
                .Include(x => x.DeliveryAddress)
                .Include(x => x.OrderCharges)
                .ToList();
            foreach (var order in orders)
            {

                orderVMs.Add(FromOrder(order, PianoTypesList));

                _forceRefreshOrders = true;
            }
            return View(orderVMs);
        }

        public ActionResult Private()
        {
            OrderVm model = new OrderVm();

            model.Pianos.Add(new PianoVm());
            model.Charges.Add(new PianoServiceVm());

            PopulateViewData();

            model.OrderPlacementType = (int)OrderTypeEnum.Private;


            return View(model);
        }
        public ActionResult Dealer()
        {
            OrderVm model = new OrderVm();

            model.Pianos.Add(new PianoVm());
            model.Charges.Add(new PianoServiceVm());

            PopulateViewData();

            model.OrderPlacementType = (int)OrderTypeEnum.Dealer;

            return View("Private", model);

        }
        public ActionResult Manufacturer()
        {
            OrderVm model = new OrderVm();

            model.Pianos.Add(new PianoVm());
            model.Charges.Add(new PianoServiceVm());

            PopulateViewData();

            model.OrderPlacementType = (int)OrderTypeEnum.Manufacturer;

            return View("Private", model);

        }
        public ActionResult Edit(Guid? id)
        {
            var order = db.PianoOrders
                            .Include(x => x.Customer)
                            .Include(x => x.Pianos)
                            .Include(x => x.PickupAddress)
                            .Include(x => x.DeliveryAddress)
                            .Include(x => x.OrderCharges)
                            .FirstOrDefault(x => x.Id == id);

            var pickupAddress = PopulateAddress(order.PickupAddress);
            var deliveryAddress = PopulateAddress(order.DeliveryAddress);

            pickupAddress.Notes = order.PickUpNotes;
            deliveryAddress.Notes = order.DeliveryNotes;

            var orderVm = new OrderVm()
            {
                Id = order.Id.ToString(),

                OrderNumber = order.OrderNumber,
                PaymentOption = order.PaymentOption.ToString(),

                CallerFirstName = order.CallerFirstName,
                CallerLastName = order.CallerLastName,
                CallerPhoneNumber = order.CallerPhoneNumber,
                CallerEmail = order.CallerEmail,

                PickupAddress = pickupAddress,
                DeliveryAddress = deliveryAddress,

                PickupDate = order.PickupDate?.ToString(),
                DeliveryDate = order.DeliveryDate?.ToString(),

                Shuttle = order.Customer != null ? order.CustomerId.ToString() : ""
            };

            var pianos = order.Pianos.ToList();

            foreach (var piano in order.Pianos)
            {
                PianoVm pianoVm = new PianoVm();
                pianoVm.SerialNumber = string.IsNullOrEmpty(piano.SerialNumber) ? string.Empty : piano.SerialNumber;
                pianoVm.IsBench = piano.IsBench;
                pianoVm.IsBoxed = piano.IsBoxed;
                pianoVm.IsPlayer = piano.IsPlayer;
                pianoVm.PianoMake = piano.PianoMakeId.ToString();
                pianoVm.PianoModel = piano.Model;
                pianoVm.PianoTypeId = piano.PianoTypeId.ToString();

                // pianoVm.PianoCategoryType = piano.PianoType.ToString();

                orderVm.Pianos.Add(pianoVm);
            }

            foreach (var service in order.OrderCharges)
            {
                orderVm.Charges.Add(new PianoServiceVm
                {
                    ServiceCharges = service.Amount.ToString(),
                    ServiceCode = service.PianoChargesId.ToString(),
                });
            }

            PopulateViewData();

            return View("Private", orderVm);
        }


        [HttpPost]
        public ActionResult Save(OrderVm orderVm)
        {
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            var errors = ModelState.Select(x => x.Value.Errors)
                                     .Where(y => y.Count > 0)
                                     .ToList();
            try
            {
                if (orderVm.Id != null)
                {
                    return EditPiano(orderVm);
                }

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
                    City = orderVm.PickupAddress.City,
                    State = orderVm.PickupAddress.State,
                    NumberTurns = orderVm.PickupAddress.Turns,
                    NumberStairs = orderVm.PickupAddress.Stairs,
                    PostCode = orderVm.PickupAddress.PostCode,
                    PhoneNumber = orderVm.PickupAddress.PhoneNumber,

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
                    City = orderVm.DeliveryAddress.City,
                    State = orderVm.DeliveryAddress.State,
                    NumberTurns = orderVm.DeliveryAddress.Turns,
                    NumberStairs = orderVm.DeliveryAddress.Stairs,
                    PostCode = orderVm.DeliveryAddress.PostCode,
                    PhoneNumber = orderVm.DeliveryAddress.PhoneNumber,


                };
                db.Addresses.Add(deliveryAddress);
                db.SaveChanges();

                var order = new PianoOrder();
                {
                    order.Id = orderId;
                    order.CreatedAt = DateTime.Now;
                    order.CreatedBy = LoggedInUser?.UserName;
                    order.OrderNumber = newOrderNumber.ToString();
                    order.OrderType = (int)(OrderTypeEnum.Private);
                    order.CallerFirstName = orderVm.CallerFirstName;
                    order.CallerLastName = orderVm.CallerLastName;
                    order.CallerPhoneNumber = orderVm.CallerPhoneNumber;
                    order.CallerEmail = orderVm.CallerEmail;

                    order.OrderType = orderVm.OrderPlacementType;

                    if (orderVm.PaymentOption != null)
                    {
                        order.PaymentOption = int.Parse(orderVm.PaymentOption);
                    }

                    order.PickupDate = orderVm.PickupAddress.PickUpDate;
                    order.DeliveryDate = orderVm.DeliveryAddress.PickUpDate;
                    order.CustomerId =
                            string.IsNullOrEmpty(orderVm.Shuttle) ? (Guid?)null : Guid.Parse(orderVm.Shuttle);

                    order.PickupAddressId = pickupAddressId;
                    order.DeliveryAddressId = deliveryAddressId;

                    order.CodAmount = orderVm.CollectableAmount;
                    order.OnlinePayment = orderVm.OnlinePaymentDetails;
                    order.OfficeStaff = orderVm.OfficeStaffDetails;

                    if (orderVm.Dealer != null)
                    { order.InvoiceClientId = Guid.Parse(orderVm.Dealer); }


                    if (orderVm.ThirdParty != null)
                    { order.InvoiceBillingPartyId = Guid.Parse(orderVm.ThirdParty); }

                    order.BillToDifferent = orderVm.IsBilledThirdParty;
                    order.CarriedBy = orderVm.CarriedBy;
                    order.SalesOrderNumber = orderVm.SalesOrderNumber;

                    order.PickUpNotes = orderVm.PickupAddress.Notes;
                    order.DeliveryNotes = orderVm.DeliveryAddress.Notes;

                    //From address notes order.DeliveryNotes


                };
                db.PianoOrders.Add(order);
                db.SaveChanges();
                foreach (var piano in orderVm.Pianos)
                {
                    InsertPiano(piano, orderId);
                }
                db.SaveChanges();

                foreach (var item in orderVm.Charges)
                {

                    db.PianoOrderCharges.Add(new PianoOrderCharges()
                    {
                        Id = new Guid(),
                        PianoChargesId = Guid.Parse(item.ServiceCode),
                        PianoOrderId = orderId,
                        Amount = int.Parse(item.ServiceCharges),
                        CreatedAt = DateTime.Now,
                        CreatedBy = LoggedInUser?.UserName,
                        ServiceStatus = (int)ServiceStatusEnum.Requested

                    });
                    db.SaveChanges();
                }

                // var orderVmSaved = OrderVm.FromOrder(order, PianoTypesList);
                // BackgroundJob.Enqueue(() => EmailHelper.SendOrderEmailToClient(orderVmSaved));

                // BackgroundJob.Enqueue(() => SMSHelper.Send(orderVmSaved));

                return Json(new { key = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { key = false }, JsonRequestBehavior.AllowGet);

            }

        }
        public ActionResult EditPiano(OrderVm orderVm)
        {
            try
            {
                PianoOrder order = db.PianoOrders.Include(x => x.Customer)
                            .Include(x => x.Pianos)
                            .Include(x => x.PickupAddress)
                            .Include(x => x.DeliveryAddress)
                            .Include(x => x.OrderCharges)
                            .Where(x => x.Id.ToString() == orderVm.Id).FirstOrDefault();

                {
                    //Order Details 
                    order.CallerFirstName = orderVm.CallerFirstName;
                    order.CallerLastName = orderVm.CallerLastName;
                    order.CallerPhoneNumber = orderVm.CallerPhoneNumber;
                    order.CallerEmail = orderVm.CallerEmail;
                    order.PaymentOption = int.Parse(orderVm.PaymentOption);
                    // order.PickupDate = orderVm.PickupAddress.PickUpDate;
                    // order.DeliveryDate = orderVm.DeliveryAddress.PickUpDate;
                    order.CustomerId =
                            string.IsNullOrEmpty(orderVm.Shuttle) ? (Guid?)null : Guid.Parse(orderVm.Shuttle);

                    order.CodAmount = orderVm.CollectableAmount;
                    order.OnlinePayment = orderVm.OnlinePaymentDetails;
                    order.OfficeStaff = orderVm.OfficeStaffDetails;

                    if (orderVm.Dealer != null)
                    {
                        order.InvoiceClientId = Guid.Parse(orderVm.Dealer);
                    }
                    if (orderVm.ThirdParty != null)
                    {
                        order.InvoiceBillingPartyId = Guid.Parse(orderVm.ThirdParty);
                    }


                    order.BillToDifferent = orderVm.IsBilledThirdParty;
                    order.CarriedBy = orderVm.CarriedBy;
                    order.SalesOrderNumber = orderVm.SalesOrderNumber;


                    //Delivery Address
                    order.DeliveryAddress.Name = orderVm.DeliveryAddress.Name;
                    order.DeliveryAddress.Address1 = orderVm.DeliveryAddress.Address1;
                    order.DeliveryAddress.City = orderVm.DeliveryAddress.City;
                    order.DeliveryAddress.State = orderVm.DeliveryAddress.State;
                    order.DeliveryAddress.NumberTurns = orderVm.DeliveryAddress.Turns;
                    order.DeliveryAddress.NumberStairs = orderVm.DeliveryAddress.Stairs;
                    order.DeliveryAddress.PostCode = orderVm.DeliveryAddress.PostCode;
                    order.DeliveryAddress.PhoneNumber = orderVm.DeliveryAddress.PhoneNumber;
                    order.DeliveryAddress.Notes = orderVm.DeliveryAddress.Notes;

                    // Pickup Address
                    order.PickupAddress.Name = orderVm.PickupAddress.Name;
                    order.PickupAddress.Address1 = orderVm.PickupAddress.Address1;
                    order.PickupAddress.City = orderVm.PickupAddress.City;
                    order.PickupAddress.State = orderVm.PickupAddress.State;
                    order.PickupAddress.NumberTurns = orderVm.PickupAddress.Turns;
                    order.PickupAddress.NumberStairs = orderVm.PickupAddress.Stairs;
                    order.PickupAddress.PostCode = orderVm.PickupAddress.PostCode;
                    order.PickupAddress.PhoneNumber = orderVm.PickupAddress.PhoneNumber;
                    order.PickupAddress.Notes = orderVm.PickupAddress.Notes;
                };

                db.SaveChanges();

                foreach (var piano in order.Pianos.ToList())
                    db.Pianos.Remove(piano);
                db.SaveChanges();

                foreach (var piano in orderVm.Pianos)
                {
                    InsertPiano(piano, order.Id);
                }
                db.SaveChanges();

                foreach (var charge in order.OrderCharges.ToList())
                    db.PianoOrderCharges.Remove(charge);
                db.SaveChanges();

                foreach (var item in orderVm.Charges)
                {

                    db.PianoOrderCharges.Add(new PianoOrderCharges()
                    {
                        Id = new Guid(),
                        PianoChargesId = Guid.Parse(item.ServiceCode),
                        PianoOrderId = order.Id,
                        Amount = int.Parse(item.ServiceCharges),
                        CreatedAt = DateTime.Now,
                        CreatedBy = LoggedInUser?.UserName,
                        ServiceStatus = (int)ServiceStatusEnum.Requested

                    });
                    db.SaveChanges();
                }
                return Json(new { key = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { key = false }, JsonRequestBehavior.AllowGet);
            }

        }

        public void PopulateViewData()
        {
            TempData["Customers"] = new SelectList(CustomersList, "Value", "Text");

            TempData["PaymentOption"] = new SelectList(PaymentOptionsList, "Value", "Text");

            TempData["PianoMake"] = new SelectList(PianoMakeList, "Value", "Text");

            TempData["PianoCategoryType"] = new SelectList(PianoCategoryTypesList, "Value", "Text");

            TempData["Charges"] = new SelectList(ServicesSelectList, "Value", "Text");

            TempData["Warehouses"] = new SelectList(WarehousesList, "Value", "Text");

            TempData["AddressStates"] = new SelectList(States, "Value", "Text");

            TempData["PianoType"] = new SelectList(PianoTypesList, "Value", "Text");

        }
        private void InsertPiano(PianoVm vm, Guid orderId)
        {
            // if (string.IsNullOrEmpty(vm.PianoName) || string.IsNullOrEmpty(vm.PianoName.Trim())) return;
            Piano obj = new Piano();

            obj.Id = Guid.NewGuid();
            obj.OrderId = orderId;
            obj.CreatedAt = DateTime.Now;
            obj.CreatedBy = LoggedInUser?.UserName;
            //TypeID from table
            obj.PianoTypeId = string.IsNullOrEmpty(vm.PianoTypeId) ? (Guid?)null : Guid.Parse(vm.PianoTypeId);

            obj.Model = vm.PianoModel;
            obj.PianoCategoryType = int.Parse(vm.PianoCategoryType);
            //make entity guid
            obj.PianoMakeId = Guid.Parse(vm.PianoMake);
            //size guid
            // obj.PianoSizeId = Guid.Parse(vm.PianoSize);
            //Need add piano category
            obj.SerialNumber = vm.SerialNumber;
            obj.IsBench = vm.IsBench;
            obj.IsBoxed = vm.IsBoxed;
            obj.IsPlayer = vm.IsStairs;

            db.Pianos.Add(obj);
        }

        public ActionResult NewPiano()
        {
            TempData["PianoMake"] = new SelectList(PianoMakeList, "Value", "Text");
            TempData["PianoType"] = new SelectList(PianoTypesList, "Value", "Text");
            TempData["PianoCategoryType"] = new SelectList(PianoCategoryTypesList, "Value", "Text");
            return PartialView("~/Views/Shared/Editors/_Piano.cshtml", new PianoVm());
        }
        public ActionResult NewService()
        {
            TempData["Charges"] = new SelectList(ServicesSelectList, "Value", "Text");
            return PartialView("~/Views/Shared/Editors/_Services.cshtml", new PianoServiceVm());
        }
        public ActionResult PopulateWarehouseDetails(string warehouseId)
        {
            try
            {
                Guid id = Guid.Parse(warehouseId);
                if (id != null)
                {
                    Address warehouse = db.Warehouses.Where(x => x.Id == id).Select(x => x.Address).FirstOrDefault();
                    if (warehouse != null)
                    {
                        var populate = new
                        {
                            address = warehouse.Address1,
                            phone = warehouse.PhoneNumber,
                            state = warehouse.State,
                            city = warehouse.City,
                            stairs = warehouse.NumberStairs,
                            turns = warehouse.NumberTurns,
                            name = warehouse.Name,
                            altName = warehouse.AlternateContact,
                            altPhone = warehouse.AlternatePhone,
                            postCode = warehouse.PostCode,


                        };
                        return Json(new { key = true, warehouse = populate }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { key = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { key = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PopulatePiano(string pianoSerialNumber)
        {
            try
            {
                Piano piano = db.Pianos.Where(x => x.SerialNumber == pianoSerialNumber).
                                   FirstOrDefault();
                if (piano != null)
                {
                    var populate = new
                    {

                        type = piano.PianoTypeId,
                        make = piano.PianoMakeId,
                        model = piano.Model,
                        isBoxed = piano.IsBoxed,
                        isBench = piano.IsBench,
                        isPlayer = piano.IsPlayer,
                        size = piano.PianoSizeId,
                    };
                    return Json(new { key = true, piano = populate }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { key = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { key = false }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult PopulateServiceCharges(string pianoServiceCode)
        {
            try
            {
                Guid guidId = Guid.Parse(pianoServiceCode);
                PianoCharges charges = db.PianoCharges.Where(x => x.Id == guidId).
                                   FirstOrDefault();
                if (charges != null)
                {
                    return Json(new { key = true, charges = charges.Amount }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { key = false }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { key = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PopulateCustomer(string customerId)
        {
            try
            {
                Guid id = Guid.Parse(customerId);
                Client client = db.Clients.Where(x => x.Id == id).
                                   FirstOrDefault();
                if (client != null)
                {
                    var populate = new
                    {
                        name = client.Name,
                        contact = client.PhoneNumber,
                        //needtfrom db
                        address = client.Addresses
                    };
                    return Json(new { key = true, client = populate }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { key = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { key = false }, JsonRequestBehavior.AllowGet);
            }

        }

        public OrderVm FromOrder(PianoOrder order, IEnumerable<SelectListItem> PianoTypesList)
        {

            AddressVm pickupAddress = PopulateAddress(order.PickupAddress);

            AddressVm deliveryAdress = PopulateAddress(order.DeliveryAddress);


            var orderVM = new OrderVm()
            {
                Id = order.Id.ToString(),
                OrderDate = order.CreatedAt.ToString(),
                OrderNumber = order.OrderNumber,
                OrderType = ((OrderTypeEnum)order.OrderType).ToString(),
                CallerFirstName = order.CallerFirstName,
                CallerLastName = order.CallerLastName,
                CallerPhoneNumber = order.CallerPhoneNumber,
                CallerEmail = order.CallerEmail,
                PickupAddressString = pickupAddress.AddressToString,
                DeliveryAddressString = deliveryAdress.AddressToString,
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
                    PianoModel = x.Model,
                    PianoMake = x.PianoMakeId.ToString(),
                    SerialNumber = x.SerialNumber,
                    IsBench = x.IsBench,
                    IsBoxed = x.IsBoxed,
                    IsStairs = x.IsPlayer
                }).ToList();

            orderVM.Pianos.ForEach(x =>
            {
                x.PianoMake = db.PianoMake.Where(y => y.Id.ToString() == x.PianoMake).FirstOrDefault().Name;

            });

            orderVM.Charges = order.OrderCharges.OrderBy(x => x.Id).Select(
               x => new PianoServiceVm()
               {
                   Id = x.Id.ToString(),
                   ServiceTypeId  = x.PianoChargesId.ToString(),
                   ServiceCharges = x.Amount.ToString()
               }).ToList();

            orderVM.Charges.ForEach(x =>
            {
                var obj = db.PianoCharges.Where(y => y.Id.ToString() == x.ServiceTypeId).FirstOrDefault();
                x.ServiceCode = obj.ChargesCode.ToString();
                x.ServiceDetails = obj.ChargesDetails.ToString();
                x.ServiceType = ((ChargesTypeEnum)obj.ChargesType).ToString();

            });
           
           
            return orderVM;
        }

        public AddressVm PopulateAddress(Address address)
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
            };
        }
    
    }
}