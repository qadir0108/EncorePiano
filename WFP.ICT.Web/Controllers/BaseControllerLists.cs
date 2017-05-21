using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WFP.ICT.Common;
using WFP.ICT.Data.Entities;
using WFP.ICT.Data.EntityManager;
using WFP.ICT.Enum;
using WFP.ICT.Web.Models;

namespace WFP.ICT.Web.Controllers
{
    public class BaseControllerLists : Controller
    {
        private WFPICTContext _db;
        public WFPICTContext db
        {
            get
            {
                if (_db == null)
                {
                    _db = HttpContext.GetOwinContext().Get<WFPICTContext>();
                }
                return _db;
            }
            private set
            {
                _db = value;
            }
        }

        private static List<SelectListItem> _claimTypes;
        public IEnumerable<SelectListItem> ClaimTypes
        {
            get
            {
                if (_claimTypes == null)
                {
                    var mgr = new AspNetClaimsManager();
                    _claimTypes = mgr.GetAll().Select(
                    x => new SelectListItem()
                    {
                        Text = x.ClaimType,
                        Value = x.ClaimType
                    }).Distinct().ToList();
                    _claimTypes.Insert(0, new SelectListItem()
                    {
                        Text = "Select Category",
                        Value = string.Empty
                    });
                }
                return _claimTypes;
            }
        }

        public IEnumerable<SelectListItem> MonthsList
        {
            get
            {
                var months = new List<SelectListItem>();
                for (int i = 0; i < 12; i++)
                {
                    months.Add(new SelectListItem()
                    {
                        Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i + 1),
                        Value = i.ToString()
                    });
                }
                months.Insert(0, new SelectListItem()
                {
                    Text = "Select Month",
                    Value = string.Empty
                });
                return months;
            }
        }

        private static List<SelectListItem> _users;
        public IEnumerable<SelectListItem> UsersList
        {
            get
            {
                if (_users == null)
                {
                    var user = Session["user"] as WFPUser;
                    _users = db.Users
                        .OrderBy(x => x.CreatedAt).Select(
                             x => new SelectListItem()
                             {
                                 Text = x.UserName,
                                 Value = x.Id
                             }).ToList();
                    _users.Insert(0, new SelectListItem()
                    {
                        Text = "Select User",
                        Value = string.Empty
                    });
                }
                return _users;
            }
        }

        private static List<SelectListItem> _customers;
        public IEnumerable<SelectListItem> CustomersList
        {
            get
            {
                if (_customers == null)
                {
                    _customers = db.Customers
                        .OrderBy(x => x.CreatedAt).Select(
                             x => new SelectListItem()
                             {
                                 Text = x.AccountCode + " " + x.Name,
                                 Value = x.Id.ToString()
                             }).ToList();
                    _customers.Insert(0, new SelectListItem()
                    {
                        Text = "Select Customer",
                        Value = string.Empty
                    });
                }
                return _customers;
            }
        }

        private static List<SelectListItem> _pianoType;
        public IEnumerable<SelectListItem> PianoTypesList
        {
            get
            {
                if (_pianoType == null)
                {
                    _pianoType = db.PianoTypes
                        .OrderBy(x => x.CreatedAt).Select(
                             x => new SelectListItem()
                             {
                                 Text = x.Type,
                                 Value = x.Id.ToString()
                             }).ToList();
                    _pianoType.Insert(0, new SelectListItem()
                    {
                        Text = "Select Type",
                        Value = string.Empty
                    });
                }
                return _pianoType;
            }
        }

        private static List<SelectListItem> _orderMedium;
        public IEnumerable<SelectListItem> OrderMediumList
        {
            get
            {
                if (_orderMedium == null)
                {
                    _orderMedium = EnumHelper.GetEnumTextValues(typeof(OrderMediumEnum)).Select(
                             x => new SelectListItem()
                             {
                                 Text = x.Text,
                                 Value = x.Value
                             }).ToList();
                    _orderMedium.Insert(0, new SelectListItem()
                    {
                        Text = "Select Order Via",
                        Value = string.Empty
                    });
                }
                return _orderMedium;
            }
        }

        private static List<PianoServiceVm> _servicesList;
        public List<PianoServiceVm> ServicesList
        {
            get
            {
                if (_servicesList == null)
                {
                    _servicesList = db.PianoServices.Where(x => x.PianoOrderId == null)
                        .OrderBy(x => x.CreatedAt).Select(
                             x => new PianoServiceVm()
                             {
                                 Id= x.Id.ToString(),
                                 ServiceCode = x.ServiceCode.ToString(),
                                 ServiceType = ((ServiceTypeEnum)x.ServiceType).ToString(),
                                 ServiceDetails = x.ServiceDetails,
                                 ServiceCharges = x.ServiceCharges.ToString()
                             }).ToList();
                }
                return _servicesList;
            }
        }

        private static List<SelectListItem> _paymentOptions;
        public IEnumerable<SelectListItem> PaymentOptionsList
        {
            get
            {
                if (_paymentOptions == null)
                {
                    _paymentOptions = EnumHelper.GetEnumTextValues(typeof(PaymentOptionEnum)).Select(
                             x => new SelectListItem()
                             {
                                 Text = x.Text,
                                 Value = x.Value
                             }).ToList();
                    _paymentOptions.Insert(0, new SelectListItem()
                    {
                        Text = "Select Payment Option",
                        Value = string.Empty
                    });
                }
                return _paymentOptions;
            }
        }

        public static bool _forceRefreshOrders;
        private static List<SelectListItem> _orders;
        public IEnumerable<SelectListItem> OrdersList
        {
            get
            {
                if (_orders == null || _forceRefreshOrders)
                {
                    _orders = db.PianoOrders
                        .OrderBy(x => x.CreatedAt).Select(
                             x => new SelectListItem()
                             {
                                 Text = x.OrderNumber,
                                 Value = x.Id.ToString()
                             }).ToList();
                    _orders.Insert(0, new SelectListItem()
                    {
                        Text = "Select Order",
                        Value = string.Empty
                    });
                    _forceRefreshOrders = false;
                }
                return _orders;
            }
        }

        private static List<SelectListItem> _warehouses;
        public IEnumerable<SelectListItem> WarehousesList
        {
            get
            {
                if (_warehouses == null)
                {
                    _warehouses = db.Warehouses
                        .OrderBy(x => x.CreatedAt).Select(
                             x => new SelectListItem()
                             {
                                 Text = x.Code + " " + x.Name,
                                 Value = x.Id.ToString()
                             }).ToList();
                    _warehouses.Insert(0, new SelectListItem()
                    {
                        Text = "Select Warehouse",
                        Value = string.Empty
                    });
                }
                return _warehouses;
            }
        }

        private static List<SelectListItem> _drivers;
        public IEnumerable<SelectListItem> DriversList
        {
            get
            {
                if (_drivers == null)
                {
                    _drivers = db.Drivers
                        .OrderBy(x => x.CreatedAt).Select(
                             x => new SelectListItem()
                             {
                                 Text = x.Code + " " + x.Name,
                                 Value = x.Id.ToString()
                             }).ToList();
                    _drivers.Insert(0, new SelectListItem()
                    {
                        Text = "Select Driver",
                        Value = string.Empty
                    });
                }
                return _drivers;
            }
        }

        private static List<SelectListItem> _vehicles;
        public IEnumerable<SelectListItem> VehiclesList
        {
            get
            {
                if (_vehicles == null)
                {
                    _vehicles = db.Vehicles
                        .OrderBy(x => x.CreatedAt).Select(
                             x => new SelectListItem()
                             {
                                 Text = x.Code + " " + x.Name,
                                 Value = x.Id.ToString()
                             }).ToList();
                    _vehicles.Insert(0, new SelectListItem()
                    {
                        Text = "Select Vehicle",
                        Value = string.Empty
                    });
                }
                return _vehicles;
            }
        }
    }
}