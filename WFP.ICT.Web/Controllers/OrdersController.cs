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

                orderVMs.Add(OrderVm.FromOrder(order, PianoTypesList));
                _forceRefreshOrders = true;
            }
            return View(orderVMs);
        }

        public ActionResult Private()
        {
            OrderVm model = new OrderVm()
            {
                Services = new List<PianoServiceVm>(),
                Pianos = new List<PianoVm>()
            };

            model.Pianos.Add(new PianoVm());
            model.Services.Add(new PianoServiceVm());

            ViewBag.Customers = new SelectList(CustomersList, "Value", "Text");

            ViewBag.PaymentOption = new SelectList(PaymentOptionsList, "Value", "Text");

            ViewBag.PianoCategoryType = new SelectList(PianoCategoryTypesList, "Value", "Text");

            ViewBag.Services = new SelectList(ServicesSelectList, "Value", "Text");

            ViewBag.Warehouses = new SelectList(WarehousesList, "Value", "Text");

            ViewBag.AddressStates = new SelectList(States, "Value", "Text");

            ViewBag.PianoType = new SelectList(PianoTypesList, "Value", "Text");

            return View(model);
        }

        public ActionResult Dealer()
        {
            OrderVm model = new OrderVm()
            {
                Services = new List<PianoServiceVm>(),
                Pianos = new List<PianoVm>()
            };

            model.Pianos.Add(new PianoVm());
            model.Services.Add(new PianoServiceVm());

            ViewBag.Customers = new SelectList(CustomersList, "Value", "Text");

            ViewBag.PaymentOption = new SelectList(PaymentOptionsList, "Value", "Text");

            ViewBag.PianoCategoryType = new SelectList(PianoCategoryTypesList, "Value", "Text");

            ViewBag.PianoType = new SelectList(PianoTypesList, "Value", "Text");

            ViewBag.Services = new SelectList(ServicesSelectList, "Value", "Text");

            ViewBag.Warehouses = new SelectList(WarehousesList, "Value", "Text");

            ViewBag.AddressStates = new SelectList(States, "Value", "Text");

            return View(model);
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
                            Notes = orderVm.PickupAddress.Notes
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
                            Notes = orderVm.DeliveryAddress.Notes

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
                        order.PaymentOption = int.Parse(orderVm.PaymentOption);
                        order.PickupDate = orderVm.PickupAddress.PickUpDate;
                        order.DeliveryDate = orderVm.DeliveryAddress.PickUpDate;
                        order.CustomerId =
                                string.IsNullOrEmpty(orderVm.Shuttle) ? (Guid?)null : Guid.Parse(orderVm.Shuttle);

                        order.PickupAddressId = pickupAddressId;
                        order.DeliveryAddressId = deliveryAddressId;
                        };
                        db.PianoOrders.Add(order);
                        db.SaveChanges();
                        foreach(var piano in orderVm.Pianos)
                        {
                            InsertPiano(piano, orderId);
                        }
                        db.SaveChanges();

                        foreach (var item in orderVm.Services)
                        {

                    db.PianoOrderCharges.Add(new PianoOrderCharges()
                    {
                              Id = new Guid(),
                               PianoChargesId = Guid.Parse(item.ServiceCode),
                               PianoOrderId = orderId,
                               Amount = int.Parse(item.ServiceCharges),
                               CreatedAt = DateTime.Now,
                                CreatedBy = LoggedInUser?.UserName

                            });
                            db.SaveChanges();
                        }

                       // var orderVmSaved = OrderVm.FromOrder(order, PianoTypesList);
                       // BackgroundJob.Enqueue(() => EmailHelper.SendOrderEmailToClient(orderVmSaved));

                       // BackgroundJob.Enqueue(() => SMSHelper.Send(orderVmSaved));

                return Json(new { key = true}, JsonRequestBehavior.AllowGet);
            }
                catch (Exception ex)
                {

                return Json(new { key = false }, JsonRequestBehavior.AllowGet);

                }

        }
        private void InsertPiano(PianoVm vm, Guid orderId)
        {
           // if (string.IsNullOrEmpty(vm.PianoName) || string.IsNullOrEmpty(vm.PianoName.Trim())) return;
            Piano obj = new Piano();

                obj.Id = Guid.NewGuid();
                obj.OrderId = orderId;
                obj.CreatedAt = DateTime.Now;
                obj.CreatedBy = LoggedInUser?.UserName;
                obj.Name = vm.PianoName;
            //TypeID from table
               obj.PianoTypeId = string.IsNullOrEmpty(vm.PianoTypeId) ? (Guid?)null : Guid.Parse(vm.PianoTypeId);
                obj.Color = vm.PianoColor;
                obj.Model = vm.PianoModel;
            //make entity guid
                obj.Make = vm.PianoMake;
            //size guid

                obj.SerialNumber = vm.SerialNumber;
                obj.IsBench = vm.IsBench;
                obj.IsBoxed = vm.IsBoxed;
                 obj.IsPlayer = vm.IsStairs;

            db.Pianos.Add(obj);
        }

        public ActionResult NewPiano()
        {
            ViewBag.PianoType = new SelectList(PianoTypesList, "Value", "Text");
            ViewBag.PianoCategoryType = new SelectList(PianoCategoryTypesList, "Value", "Text");
            return PartialView("~/Views/Shared/Editors/_Piano.cshtml", new PianoVm());
        }

        public ActionResult NewService()
        {
            ViewBag.Services = new SelectList(ServicesSelectList, "Value", "Text");
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
                Piano piano =  db.Pianos.Where(x => x.SerialNumber == pianoSerialNumber).
                                   FirstOrDefault();
                if (piano != null)
                {
                    var populate = new
                    {

                        type =  piano.PianoTypeId,
                        make = piano.Make,
                        model = piano.Model,
                        isBoxed = piano.IsBoxed,
                        isBench = piano.IsBench,
                        isPlayer = piano.IsPlayer,
                        size = piano.PianoSize,
                    };
                    return Json(new { key = true , piano = populate }, JsonRequestBehavior.AllowGet);
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
                    if(charges != null)
                {
                    return Json(new { key = true, charges = charges.Amount }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { key = false}, JsonRequestBehavior.AllowGet);

            }
            catch(Exception ex)
            {
                return Json(new { key = false }, JsonRequestBehavior.AllowGet);
            }         
        }

    }
}