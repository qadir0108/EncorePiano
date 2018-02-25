using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using WFP.ICT.Data.Entities;
using WFP.ICT.Web.Models;
using WFP.ICT.Enum;
using WFP.ICT.Enums;
using WFP.ICT.Common;
using System.Web;
using System.IO;
using WFP.ICT.Web.Async;
using System.Text;
using WFP.ICT.Web.Helpers;

namespace WFP.ICT.Web.Controllers
{
    [Authorize]
    [AjaxAuthorize]
    public class OrdersController : BaseController
    {
        // GET: Orders
        public ActionResult Index()
        {
            //SMSHelper.Send("+923216334272", "Hello, Kamran");
            var orderVMs = new List<OrderVm>();
            var orders = Db.Orders
                .Include(x => x.Client)
                .Include(x => x.Pianos)
                .Include(x => x.PickupAddress)
                .Include(x => x.DeliveryAddress)
                .Include(x => x.OrderCharges)
                .ToList();
            foreach (var order in orders)
            {

                orderVMs.Add(FromOrder(order));

                _forceRefreshOrders = true;
            }
            return View(orderVMs);
        }

        public ActionResult Private()
        {
            OrderVm model = new OrderVm()
            {
                OrderPlacementType = (int)OrderTypeEnum.Private,
            };
            model.Pianos.Add(new PianoVm());
            model.Legs.Add(new LegVm() { LegNumber = 1, LegDate = DateTime.Now, LegTypeId = LegTypesList.FirstOrDefault(x => x.Text == LegTypeEnum.Pickup.ToString()).Value });
            model.Charges.Add(new PianoChargesVm());

            PopulateViewData();

            return View(model);
        }
        public ActionResult Dealer()
        {
            OrderVm model = new OrderVm();

            model.Pianos.Add(new PianoVm());
            model.Legs.Add(new LegVm() { LegNumber = 1, LegDate = DateTime.Now, LegTypeId = LegTypesList.FirstOrDefault(x => x.Text == LegTypeEnum.Pickup.ToString()).Value });
            model.Charges.Add(new PianoChargesVm());

            PopulateViewData();

            model.OrderPlacementType = (int)OrderTypeEnum.Dealer;

            return View("Private", model);

        }
        public ActionResult Manufacturer()
        {
            OrderVm model = new OrderVm();

            model.Pianos.Add(new PianoVm());
            model.Legs.Add(new LegVm() { LegNumber = 1, LegDate = DateTime.Now, LegTypeId = LegTypesList.FirstOrDefault(x => x.Text == LegTypeEnum.Pickup.ToString()).Value });
            model.Charges.Add(new PianoChargesVm());

            PopulateViewData();

            model.OrderPlacementType = (int)OrderTypeEnum.Manufacturer;

            return View("Private", model);

        }
        public ActionResult Edit(Guid? id)
        {
            var order = Db.Orders
                            .Include(x => x.Client)
                            .Include(x => x.Pianos)
                            .Include(x => x.Legs)
                            .Include(x => x.PickupAddress)
                            .Include(x => x.DeliveryAddress)
                            .Include(x => x.OrderCharges)
                            .FirstOrDefault(x => x.Id == id);

            var pickupAddress = OrderVm.PopulateAddress(order.PickupAddress);
            var deliveryAddress = OrderVm.PopulateAddress(order.DeliveryAddress);

            pickupAddress.Notes = order.PickUpNotes;
            deliveryAddress.Notes = order.DeliveryNotes;

            pickupAddress.PickUpDate = order.PickupDate;
            deliveryAddress.PickUpDate = order.DeliveryDate;

            var orderVm = new OrderVm()
            {
                Id = order.Id.ToString(),
                OrderPlacementType = order.OrderType,
                OrderNumber = order.OrderNumber,
                PaymentOption = order.PaymentOption.ToString(),

                CallerFirstName = order.CallerFirstName,
                CallerLastName = order.CallerLastName,
                CallerPhoneNumber = order.CallerPhoneNumber,
                CallerEmail = order.CallerEmail,
                CallerAlternatePhone = order.CallerAlternatePhone,

                CollectableAmount = order.CodAmount,
                CarriedBy = order.CarriedBy,
                SalesOrderNumber = order.SalesOrderNumber,
                IsBilledThirdParty = order.BillToDifferent,

                OfficeStaffDetails = order.OfficeStaff,
                OnlinePaymentDetails = order.OnlinePayment,
                ThirdParty = order.InvoiceBillingPartyId.ToString(),
                Dealer = order.InvoiceClientId.ToString(),

                DeliverForm = order.DeliveryForm,

                PickupAddress = pickupAddress,
                DeliveryAddress = deliveryAddress,

                Shuttle = order.Client != null ? order.ClientId.ToString() : ""
            };

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
                pianoVm.PianoCategoryType = piano.PianoCategoryType.ToString();
                pianoVm.Notes = piano.Notes;
                pianoVm.PianoFinish = piano.PianoFinishId.ToString();
                pianoVm.PianoSize = piano.PianoSizeId.ToString();
                // pianoVm.PianoCategoryType = piano.PianoType.ToString();
                orderVm.Pianos.Add(pianoVm);
            }

            foreach (var leg in order.Legs)
            {
                LegVm legVm = new LegVm();
                legVm.Id = leg.Id;
                legVm.OrderId = leg.OrderId.Value;
                legVm.LegNumber = leg.LegNumber;
                legVm.LegTypeId = leg.LegType.ToString();
                legVm.FromLocationId = leg.FromLocationId?.ToString();
                legVm.ToLocationId = leg.ToLocationId?.ToString();
                legVm.DriverId = leg.DriverId?.ToString();
                legVm.LegDate = leg.LegDate;
                orderVm.Legs.Add(legVm);
            }

            foreach (var service in order.OrderCharges)
            {
                orderVm.Charges.Add(new PianoChargesVm
                {
                    Amount = service.Amount.ToString(),
                    ChargesCode = service.PianoChargesId.ToString(),
                });
            }

            PopulateViewData();

            return View("Private", orderVm);
        }

        [HttpPost]
        public ActionResult Save(OrderVm orderVm)
        {

            try
            {
                if (orderVm.Id != null)
                {
                    return EditOrder(orderVm);
                }

                int newOrderNumber = Db.Orders.Any()
                    ? Db.Orders.ToList().Max(x => int.Parse(x.OrderNumber)) + 1
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
                    NumberTurns = orderVm.PickupAddress.Turns == null ? 0 : orderVm.PickupAddress.Turns.Value,
                    NumberStairs = orderVm.PickupAddress.Stairs == null ? 0 : orderVm.PickupAddress.Stairs.Value,
                    PostCode = orderVm.PickupAddress.PostCode,
                    PhoneNumber = orderVm.PickupAddress.PhoneNumber,
                    AlternateContact = orderVm.PickupAddress.AlternateContact,
                    AlternatePhone = orderVm.PickupAddress.AlternatePhone,
                    WarehouseId = string.IsNullOrEmpty(orderVm.PickupAddress.Warehouse) ? (Guid?)null : Guid.Parse(orderVm.PickupAddress.Warehouse),


                };
                Db.Addresses.Add(pickupAddress);
                Db.SaveChanges();

                Address deliveryAddress = new Address()
                {
                    Id = deliveryAddressId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = LoggedInUser?.UserName,
                    Name = orderVm.DeliveryAddress.Name,
                    Address1 = orderVm.DeliveryAddress.Address1,
                    City = orderVm.DeliveryAddress.City,
                    State = orderVm.DeliveryAddress.State,
                    NumberTurns = orderVm.DeliveryAddress.Turns == null ? 0 : orderVm.DeliveryAddress.Turns.Value,
                    NumberStairs = orderVm.DeliveryAddress.Stairs == null ? 0 : orderVm.DeliveryAddress.Stairs.Value,
                    PostCode = orderVm.DeliveryAddress.PostCode,
                    PhoneNumber = orderVm.DeliveryAddress.PhoneNumber,
                    AlternateContact = orderVm.DeliveryAddress.AlternateContact,
                    AlternatePhone = orderVm.DeliveryAddress.AlternatePhone,
                    WarehouseId = string.IsNullOrEmpty(orderVm.DeliveryAddress.Warehouse) ? (Guid?)null : Guid.Parse(orderVm.DeliveryAddress.Warehouse),
                };
                Db.Addresses.Add(deliveryAddress);
                Db.SaveChanges();

                var order = new Order();
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
                    order.CallerAlternatePhone = orderVm.CallerAlternatePhone;

                    order.OrderType = orderVm.OrderPlacementType;

                    if (orderVm.PaymentOption != null)
                    {
                        order.PaymentOption = int.Parse(orderVm.PaymentOption);
                    }

                    order.PickupDate = orderVm.PickupAddress.PickUpDate;
                    order.DeliveryDate = orderVm.DeliveryAddress.PickUpDate;
                    order.ClientId =
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
                Db.Orders.Add(order);
                Db.SaveChanges();
                foreach (var piano in orderVm.Pianos)
                {
                    InsertPiano(piano, orderId);
                }
                Db.SaveChanges();

                foreach (var leg in orderVm.Legs)
                {
                    InsertLeg(leg, orderId);
                }
                Db.SaveChanges();

                foreach (var item in orderVm.Charges)
                {
                    if (string.IsNullOrEmpty(item.ChargesCode)) continue;

                    Db.OrderCharges.Add(new OrderCharges()
                    {
                        Id = Guid.NewGuid(),
                        PianoChargesId = Guid.Parse(item.ChargesCode),
                        OrderId = orderId,
                        Amount = int.Parse(item.Amount),
                        CreatedAt = DateTime.Now,
                        CreatedBy = LoggedInUser?.UserName,
                        ServiceStatus = (int)ServiceStatusEnum.Requested

                    });
                    Db.SaveChanges();
                }

                foreach (string selectedFile in Request.Files)
                {
                    HttpPostedFileBase fileContent = Request.Files[selectedFile];
                    UploadFile(fileContent, order.OrderNumber);
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

        public ActionResult EditOrder(OrderVm orderVm)
        {
            try
            {
                Order order = Db.Orders.Include(x => x.Client)
                            .Include(x => x.Pianos)
                            .Include(x => x.Legs)
                            .Include(x => x.PickupAddress)
                            .Include(x => x.DeliveryAddress)
                            .Include(x => x.OrderCharges)
                            .Where(x => x.Id.ToString() == orderVm.Id).FirstOrDefault();

                {

                    order.OrderType = (int)(OrderTypeEnum.Private);
                    order.CallerFirstName = orderVm.CallerFirstName;
                    order.CallerLastName = orderVm.CallerLastName;
                    order.CallerPhoneNumber = orderVm.CallerPhoneNumber;
                    order.CallerEmail = orderVm.CallerEmail;
                    order.CallerAlternatePhone = orderVm.CallerAlternatePhone;
                    order.OrderType = orderVm.OrderPlacementType;
                    if (orderVm.PaymentOption != null)
                    {
                        order.PaymentOption = int.Parse(orderVm.PaymentOption);
                    }

                    order.PickupDate = orderVm.PickupAddress.PickUpDate;
                    order.DeliveryDate = orderVm.DeliveryAddress.PickUpDate;
                    order.ClientId =
                            string.IsNullOrEmpty(orderVm.Shuttle) ? (Guid?)null : Guid.Parse(orderVm.Shuttle);

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
                    //Delivery Address
                    order.DeliveryAddress.Name = orderVm.DeliveryAddress.Name;
                    order.DeliveryAddress.Address1 = orderVm.DeliveryAddress.Address1;
                    order.DeliveryAddress.City = orderVm.DeliveryAddress.City;
                    order.DeliveryAddress.State = orderVm.DeliveryAddress.State;
                    order.DeliveryAddress.NumberTurns = orderVm.DeliveryAddress.Turns == null ? 0 : orderVm.DeliveryAddress.Turns.Value;
                    order.DeliveryAddress.NumberStairs = orderVm.DeliveryAddress.Turns == null ? 0 : orderVm.DeliveryAddress.Turns.Value;
                    order.DeliveryAddress.PostCode = orderVm.DeliveryAddress.PostCode;
                    order.DeliveryAddress.PhoneNumber = orderVm.DeliveryAddress.PhoneNumber;
                    order.DeliveryAddress.AlternateContact = orderVm.DeliveryAddress.AlternateContact;
                    order.DeliveryAddress.AlternatePhone = orderVm.DeliveryAddress.AlternatePhone;
                    order.DeliveryAddress.WarehouseId = string.IsNullOrEmpty(orderVm.DeliveryAddress.Warehouse) ? (Guid?)null : Guid.Parse(orderVm.DeliveryAddress.Warehouse);

                    // Pickup Address
                    order.PickupAddress.Name = orderVm.PickupAddress.Name;
                    order.PickupAddress.Address1 = orderVm.PickupAddress.Address1;
                    order.PickupAddress.City = orderVm.PickupAddress.City;
                    order.PickupAddress.State = orderVm.PickupAddress.State;
                    order.PickupAddress.NumberTurns = orderVm.PickupAddress.Turns == null ? 0 : orderVm.PickupAddress.Turns.Value;
                    order.PickupAddress.NumberStairs = orderVm.PickupAddress.Turns == null ? 0 : orderVm.PickupAddress.Turns.Value;
                    order.PickupAddress.PostCode = orderVm.PickupAddress.PostCode;
                    order.PickupAddress.PhoneNumber = orderVm.PickupAddress.PhoneNumber;
                    order.PickupAddress.AlternateContact = orderVm.PickupAddress.AlternateContact;
                    order.PickupAddress.AlternatePhone = orderVm.PickupAddress.AlternatePhone;
                    order.PickupAddress.WarehouseId = string.IsNullOrEmpty(orderVm.PickupAddress.Warehouse) ? (Guid?)null : Guid.Parse(orderVm.PickupAddress.Warehouse);
                };

                Db.SaveChanges();

                foreach (var piano in order.Pianos.ToList())
                    Db.Pianos.Remove(piano);
                Db.SaveChanges();
                foreach (var piano in orderVm.Pianos)
                {
                    InsertPiano(piano, order.Id);
                }
                Db.SaveChanges();

                foreach (var leg in order.Legs.ToList())
                    Db.Legs.Remove(leg);
                Db.SaveChanges();
                foreach (var leg in orderVm.Legs)
                {
                    InsertLeg(leg, order.Id);
                }
                Db.SaveChanges();

                foreach (var charge in order.OrderCharges.ToList())
                    Db.OrderCharges.Remove(charge);
                Db.SaveChanges();

                foreach (var item in orderVm.Charges)
                {

                    Db.OrderCharges.Add(new OrderCharges()
                    {
                        Id = Guid.NewGuid(),
                        PianoChargesId = Guid.Parse(item.ChargesCode),
                        OrderId = order.Id,
                        Amount = int.Parse(item.Amount),
                        CreatedAt = DateTime.Now,
                        CreatedBy = LoggedInUser?.UserName,
                        ServiceStatus = (int)ServiceStatusEnum.Requested

                    });
                    Db.SaveChanges();

                    foreach (string selectedFile in Request.Files)
                    {
                        HttpPostedFileBase fileContent = Request.Files[selectedFile];
                        UploadFile(fileContent, order.OrderNumber);
                    }
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

            TempData["PianoFinish"] = new SelectList(PianoFinishList, "Value", "Text");

            TempData["AdditionalItemStatus"] = new SelectList(AdditionalItemStatusList, "Value", "Text");

            TempData["LegTypes"] = new SelectList(LegTypesList, "Value", "Text");
            TempData["Locations"] = new SelectList(LocationsList, "Value", "Text");
            TempData["Drivers"] = new SelectList(DriversList, "Value", "Text");

        }
        private void InsertPiano(PianoVm vm, Guid orderId)
        {
            if (string.IsNullOrEmpty(vm.PianoCategoryType)) return;

            Piano obj = new Piano();

            obj.Id = Guid.NewGuid();
            obj.OrderId = orderId;
            obj.CreatedAt = DateTime.Now;
            obj.CreatedBy = LoggedInUser?.UserName;
            //TypeID from table
            obj.PianoTypeId = string.IsNullOrEmpty(vm.PianoTypeId) ? (Guid?)null : Guid.Parse(vm.PianoTypeId);

            obj.PianoFinishId = vm.PianoFinish == null ? (Guid?)null : Guid.Parse(vm.PianoFinish);

            obj.Model = vm.PianoModel;

            obj.PianoCategoryType = int.Parse(vm.PianoCategoryType);

            //make entity guid
            obj.PianoMakeId = vm.PianoMake == null ? (Guid?)null : Guid.Parse(vm.PianoMake);
            //size guid

            obj.PianoSizeId = vm.PianoSize == null ? (Guid?)null : Guid.Parse(vm.PianoSize);
            obj.Notes = vm.Notes;
            //Need add piano category
            obj.SerialNumber = vm.SerialNumber;
            obj.IsBench = vm.IsBench;
            obj.IsBoxed = vm.IsBoxed;
            obj.IsPlayer = vm.IsPlayer;

            Db.Pianos.Add(obj);
        }
        private void InsertLeg(LegVm vm, Guid orderId)
        {
            if (vm.LegNumber <= 0) return;

            Leg obj = new Leg();

            obj.Id = Guid.NewGuid();
            obj.OrderId = orderId;
            obj.CreatedAt = DateTime.Now;
            obj.CreatedBy = LoggedInUser?.UserName;
            
            //TypeID from table
            obj.LegNumber = vm.LegNumber;
            obj.LegType = int.Parse(vm.LegTypeId);

            obj.FromLocationId = vm.FromLocationId == null ? Guid.Empty : Guid.Parse(vm.FromLocationId);
            obj.ToLocationId = vm.ToLocationId == null ? Guid.Empty : Guid.Parse(vm.ToLocationId);
            obj.DriverId = vm.DriverId == null ? Guid.Empty : Guid.Parse(vm.DriverId);

            obj.LegDate = vm.LegDate;

            Db.Legs.Add(obj);
        }
        public ActionResult NewPiano()
        {
            TempData["PianoMake"] = new SelectList(PianoMakeList, "Value", "Text");
            TempData["PianoFinish"] = new SelectList(PianoFinishList, "Value", "Text");
            TempData["PianoType"] = new SelectList(PianoTypesList, "Value", "Text");
            TempData["PianoCategoryType"] = new SelectList(PianoCategoryTypesList, "Value", "Text");
            TempData["AdditionalItemStatus"] = new SelectList(AdditionalItemStatusList, "Value", "Text");
            return PartialView("~/Views/Shared/Editors/_Piano.cshtml", new PianoVm());
        }
        public ActionResult NewLeg()
        {
            TempData["LegTypes"] = new SelectList(LegTypesList, "Value", "Text");
            TempData["Locations"] = new SelectList(LocationsList, "Value", "Text");
            TempData["Drivers"] = new SelectList(DriversList, "Value", "Text");
            return PartialView("~/Views/Shared/Editors/_Leg.cshtml", new LegVm());
        }
        public ActionResult NewService()
        {
            TempData["Charges"] = new SelectList(ServicesSelectList, "Value", "Text");
            return PartialView("~/Views/Shared/Editors/_Services.cshtml", new PianoChargesVm());
        }
        public ActionResult PopulateWarehouseDetails(string warehouseId)
        {
            try
            {
                Guid id = Guid.Parse(warehouseId);
                if (id != null)
                {
                    Warehouse warehouse = Db.Warehouses.Where(x => x.Id == id).FirstOrDefault();
                    if (warehouse != null)
                    {
                        var populate = new
                        {
                            address = warehouse.Address1,
                            phone = warehouse.PhoneNumber,
                            state = warehouse.State,
                            city = warehouse.City,
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
                Piano piano = Db.Pianos.Where(x => x.SerialNumber == pianoSerialNumber).
                                   FirstOrDefault();
                if (piano != null)
                {
                    var populate = new
                    {

                        type = piano.PianoTypeId,
                        make = piano.PianoMakeId,
                        model = piano.Model,
                        finish = piano.PianoFinishId,
                        isBoxed = piano.IsBoxed ? "True" : "False",
                        isBench = piano.IsBench ? "True" : "False",
                        isPlayer = piano.IsPlayer ? "True" : "False",
                        category = piano.PianoCategoryType,
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
                PianoCharges charges = Db.PianoCharges.Where(x => x.Id == guidId).
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
                Client client = Db.Clients.Where(x => x.Id == id).
                                   FirstOrDefault();
                if (client != null)
                {
                    var populate = new
                    {
                        name = client.Name,
                        contact = client.PhoneNumber,
                        //needtfrom db
                        address = client.Address
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

        public ActionResult PopulateSelectList(string pianoMake)
        {
            try
            {
                Guid id = Guid.Parse(pianoMake);
                var list = Db.PianoSize.Where(x => x.PianoTypeId == id).ToList().
                            Select(x => new
                            {
                                id = x.Id,
                                width = PianoSizeConversion.GetFeetInches(x.Width),
                            });



                return Json(new { key = true, list = list }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                return Json(new { key = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UploadFile(HttpPostedFileBase fileContent, string OrderNumber)
        {
            try
            {
                string FileName = "";
                string Path = Server.MapPath("~/Uploads/Forms");
                if (!System.IO.Directory.Exists(Path)) System.IO.Directory.CreateDirectory(Path);

                if (fileContent != null && fileContent.ContentLength > 0)
                {
                    FileName = string.Format("{0:yyyyMMdd_HHmmss}_{1}_{2}", DateTime.Now, fileContent.FileName, OrderNumber);
                    var stream = fileContent.InputStream;
                    string filePath = System.IO.Path.Combine(Path, FileName);
                    using (var fileStream = System.IO.File.Create(filePath))
                    {
                        stream.CopyTo(fileStream);
                    }
                }

                var order =  Db.Orders.Where(x => x.OrderNumber == OrderNumber).FirstOrDefault();
                order.DeliveryForm = FileName;
                Db.SaveChanges();

                return Json(new JsonResponse() { IsSucess = true, Result = FileName });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Download(string fileName)
        {
            string PathDl = Server.MapPath("~/Uploads/Forms");
            string filePath = Path.Combine(PathDl, Server.UrlEncode(fileName));
            if (!System.IO.File.Exists(filePath))
                return null;

            return File(filePath, "application", fileName);
        
       }

        public OrderVm FromOrder(Order order)
        {
            AddressVm pickupAddress = OrderVm.PopulateAddress(order.PickupAddress);
            AddressVm deliveryAdress = OrderVm.PopulateAddress(order.DeliveryAddress);

            //pickupAddress.Notes = order.PickUpNotes;
            //deliveryAdress.Notes = order.DeliveryNotes;

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
                PickupAddressString = pickupAddress.ToString,
                DeliveryAddressString = deliveryAdress.ToString,
                PickupDate = order.PickupDate?.ToString(),
                DeliveryDate = order.DeliveryDate?.ToString(),
                PickupInstructions = order.PickUpNotes,
                DeliveryInstructions = order.DeliveryNotes,
                Customer = order.Client != null ? order.Client.AccountCode + " " + order.Client.Name : "",

                IsBilledThirdParty = order.BillToDifferent,
                CollectableAmount = order.CodAmount,
                OnlinePaymentDetails = order.OnlinePayment,

                OfficeStaffDetails = order.OfficeStaff,
                Dealer = order.InvoiceClientId.ToString(),
                ThirdParty = order.InvoiceBillingPartyId.ToString(),

            };

            orderVM.Pianos = order.Pianos.OrderByDescending(x => x.CreatedAt).Select(
                x => new PianoVm()
                {
                    Id = x.Id,
                    OrderId = order.Id,
                    PianoCategoryType = System.Enum.GetName(typeof(PianoCategoryTypeEnum),x.PianoCategoryType),
                    PianoType = x.PianoTypeId.ToString(),
                    PianoSize = x.PianoSizeId.ToString(),
                    PianoMake = x.PianoMakeId.ToString(),
                    PianoModel = x.Model,
                    PianoFinish = PianoFinishList.FirstOrDefault(y => y.Value == x.PianoFinishId.ToString()).Text,
                    SerialNumber = x.SerialNumber,
                    IsBench = x.IsBench,
                    IsBoxed = x.IsBoxed,
                    IsPlayer = x.IsPlayer,
                    Notes = x.Notes
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

            });

            orderVM.Charges = order.OrderCharges.OrderBy(x => x.Id).Select(
               x => new PianoChargesVm()
               {
                   Id = x.Id.ToString(),
                   ChargesTypeId = x.PianoChargesId.ToString(),
                   Amount = x.Amount.ToString()
               }).ToList();

            orderVM.Charges.ForEach(x =>
            {
                var obj = Db.PianoCharges.Where(y => y.Id.ToString() == x.ChargesTypeId).FirstOrDefault();
                x.ChargesCode = obj.Code.ToString();
                x.ChargesDetails = obj.Details.ToString();
                //x.ChargesType = ((ChargesTypeEnum)obj.ChargesType).ToString();

            });


            return orderVM;
        }

        [HttpPost]
        public ActionResult SendQoute(OrderVm orderVm)
        {

            try
            {
                String subject = string.Format(@"Order Qoute");

                StringBuilder body = new StringBuilder();
                body.AppendFormat(@"Please find the detailed qoute for order # {0} <br/><br/>", orderVm.OrderNumber);
                body.AppendFormat(@"Pick Up Address : {0} <br/><br/>", orderVm.PickupAddress.ToStringWithStairTurns );

                body.AppendFormat(@"Delivery Address : {0} <br/><br/>", orderVm.DeliveryAddress.ToStringWithStairTurns);

                body.AppendFormat(@"Units : <br/>");
                foreach(var item in orderVm.Pianos)
                {
                    body.AppendFormat(@"Bench? : {0} " , item.IsBench ? "Yes" : "No");
                    body.AppendFormat(@"Player? : {0} <br/>", item.IsPlayer ? "Yes" : "No");
                    body.AppendFormat(@"Boxed? : {0} ", item.IsBoxed ? "Yes" : "No");

                    body.AppendFormat(@"Serial Number : {0} ", item.SerialNumber);
                    body.AppendFormat(@"Category : {0} ", ((PianoCategoryTypeEnum)(int.Parse(item.PianoCategoryType))).ToString());
                    body.AppendFormat(@"Make : {0} ", string.IsNullOrEmpty(item.PianoMake) ? "N/A" : Db.PianoMake.Where(x => x.Id.ToString() == item.PianoMake).FirstOrDefault().Name);
                    body.AppendFormat(@"Model : {0} <br/>", item.PianoModel);
                    body.AppendFormat(@"Size : {0} ", string.IsNullOrEmpty(item.PianoSize) ? "N/A" : PianoSizeConversion.GetFeetInches(Db.PianoSize.Where(x => x.Id.ToString() == item.PianoSize).FirstOrDefault().Width));
                    body.AppendFormat(@"Type : {0} ", string.IsNullOrEmpty(item.PianoTypeId) ? "N/A" : Db.PianoTypes.Where(x => x.Id.ToString() == item.PianoTypeId).FirstOrDefault().Type);
                    body.AppendFormat(@"Finish : {0} <br/>", string.IsNullOrEmpty(item.PianoFinish) ? "N/A" : Db.PianoFinish.Where(x => x.Id.ToString() == item.PianoFinish).FirstOrDefault().Name);

                    body.AppendFormat(@"Miscellaneous : {0} <br/><br/>", item.Notes);

                }
                body.AppendFormat(@"Charges : <br/>");

                foreach (var item in orderVm.Charges)
                {
                    body.AppendFormat(@"Type : {0} ", Db.PianoCharges.Where(x=> x.Id.ToString() == item.ChargesCode).FirstOrDefault().Details);
                    body.AppendFormat(@"Amount : {0} <br/>", item.Amount);
                }

                EmailHelper.SendEmail(orderVm.CallerEmail, subject, body.ToString());

                return Json(new { key = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { key = false }, JsonRequestBehavior.AllowGet);

            }

        }

    }
}