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
using WFP.ICT.Enums;
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

        private static List<SelectListItem> _states;
        public IEnumerable<SelectListItem> States
        {
            get
            {
                if (_states == null)
                {
                    _states = new SelectListItem[]
                        {
                            new SelectListItem() { Value = "AL", Text = "Alabama"},
                            new SelectListItem() { Value = "AK", Text = "Alaska"},
                            new SelectListItem() { Value = "AZ", Text = "Arizona"},
                            new SelectListItem() { Value = "AR", Text = "Arkansas"},
                            new SelectListItem() { Value = "CA", Text = "California"},
                            new SelectListItem() { Value = "CO", Text = "Colorado"},
                            new SelectListItem() { Value = "CT", Text = "Connecticut"},
                            new SelectListItem() { Value = "DE", Text = "Delaware"},
                            new SelectListItem() { Value = "DC", Text = "District Of Columbia"},
                            new SelectListItem() { Value = "FL", Text = "Florida"},
                            new SelectListItem() { Value = "GA", Text = "Georgia"},
                            new SelectListItem() { Value = "HI", Text = "Hawaii"},
                            new SelectListItem() { Value = "ID", Text = "Idaho"},
                            new SelectListItem() { Value = "IL", Text = "Illinois"},
                            new SelectListItem() { Value = "IN", Text = "Indiana"},
                            new SelectListItem() { Value = "IA", Text = "Iowa"},
                            new SelectListItem() { Value = "KS", Text = "Kansas"},
                            new SelectListItem() { Value = "KY", Text = "Kentucky"},
                            new SelectListItem() { Value = "LA", Text = "Louisiana"},
                            new SelectListItem() { Value = "ME", Text = "Maine"},
                            new SelectListItem() { Value = "MD", Text = "Maryland"},
                            new SelectListItem() { Value = "MA", Text = "Massachusetts"},
                            new SelectListItem() { Value = "MI", Text = "Michigan"},
                            new SelectListItem() { Value = "MN", Text = "Minnesota"},
                            new SelectListItem() { Value = "MS", Text = "Mississippi"},
                            new SelectListItem() { Value = "MO", Text = "Missouri"},
                            new SelectListItem() { Value = "MT", Text = "Montana"},
                            new SelectListItem() { Value = "NE", Text = "Nebraska"},
                            new SelectListItem() { Value = "NV", Text = "Nevada"},
                            new SelectListItem() { Value = "NH", Text = "New Hampshire"},
                            new SelectListItem() { Value = "NJ", Text = "New Jersey"},
                            new SelectListItem() { Value = "NM", Text = "New Mexico"},
                            new SelectListItem() { Value = "NY", Text = "New York"},
                            new SelectListItem() { Value = "NC", Text = "North Carolina"},
                            new SelectListItem() { Value = "ND", Text = "North Dakota"},
                            new SelectListItem() { Value = "OH", Text = "Ohio"},
                            new SelectListItem() { Value = "OK", Text = "Oklahoma"},
                            new SelectListItem() { Value = "OR", Text = "Oregon"},
                            new SelectListItem() { Value = "PA", Text = "Pennsylvania"},
                            new SelectListItem() { Value = "RI", Text = "Rhode Island"},
                            new SelectListItem() { Value = "SC", Text = "South Carolina"},
                            new SelectListItem() { Value = "SD", Text = "South Dakota"},
                            new SelectListItem() { Value = "TN", Text = "Tennessee"},
                            new SelectListItem() { Value = "TX", Text = "Texas"},
                            new SelectListItem() { Value = "UT", Text = "Utah"},
                            new SelectListItem() { Value = "VT", Text = "Vermont"},
                            new SelectListItem() { Value = "VA", Text = "Virginia"},
                            new SelectListItem() { Value = "WA", Text = "Washington"},
                            new SelectListItem() { Value = "WV", Text = "West Virginia"},
                            new SelectListItem() { Value = "WI", Text = "Wisconsin"},
                            new SelectListItem() { Value = "WY", Text = "Wyoming"}
                        }.ToList();
                    _states.Insert(0, new SelectListItem()
                    {
                        Text = "Select a State",
                        Value = string.Empty
                    });
                }
                return _states;
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
                    _customers = db.Clients
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

        private static List<SelectListItem> _pianoCategoryType;
        public IEnumerable<SelectListItem> PianoCategoryTypesList
        {
            get
            {
                if (_pianoCategoryType == null)
                {
                    _pianoCategoryType = EnumHelper.GetEnumTextValues(typeof(PianoCategoryTypeEnum)).Select(
                             x => new SelectListItem()
                             {
                                 Text = x.Text,
                                 Value = x.Value
                             }).ToList();

                    _pianoCategoryType.Insert(0, new SelectListItem()
                    {
                        Text = "Select Type",
                        Value = string.Empty
                    });
                }
                return _pianoCategoryType;
            }
        }

        private static List<SelectListItem> _orderType;
        public IEnumerable<SelectListItem> OrderTypeList
        {
            get
            {
                if (_orderType == null)
                {
                    _orderType = EnumHelper.GetEnumTextValues(typeof(OrderTypeEnum)).Select(
                             x => new SelectListItem()
                             {
                                 Text = x.Text,
                                 Value = x.Value
                             }).ToList();
                    _orderType.Insert(0, new SelectListItem()
                    {
                        Text = "Select Order Type",
                        Value = string.Empty
                    });
                }
                return _orderType;
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
                    _servicesList = db.PianoCharges.Where(x => x.PianoOrderId == null)
                        .OrderBy(x => x.ServiceCode).Select(
                             x => new PianoServiceVm()
                             {
                                 Id= x.Id.ToString(),
                                 ServiceCode = x.ServiceCode.ToString(),
                                 ServiceType = ((ChargesTypeEnum)x.ServiceType).ToString(),
                                 ServiceDetails = x.ServiceDetails,
                                 ServiceCharges = x.ServiceCharges.ToString()
                             }).ToList();
                }
                return _servicesList;
            }
        }
        private static List<SelectListItem> _servicesSelectList;
        public IEnumerable<SelectListItem> ServicesSelectList
        {
            get
            {
                if (_servicesSelectList == null)
                {
                    _servicesSelectList = db.PianoCharges.Where(x => x.PianoOrderId == null)
                        .OrderBy(x => x.ServiceCode).Select(
                             x => new SelectListItem()
                             {
                                 Text = x.ServiceDetails,
                                 Value = x.ServiceCode.ToString()
                             }).ToList();
                    _servicesSelectList.Insert(0, new SelectListItem()
                    {
                        Text = "Select Service Charges",
                        Value = string.Empty
                    });
                }
                return _servicesSelectList;
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
        private static List<SelectListItem> _Warehouses;
        public IEnumerable<SelectListItem> Warehouses
        {
            get
            {
                if (_Warehouses == null)
                {
                    _Warehouses = db.Warehouses
                        .OrderBy(x => x.CreatedAt).Select(
                             x => new SelectListItem()
                             {
                                 Text = x.Code + " " + x.Name,
                                 Value = x.Id.ToString()
                             }).ToList();
                    _Warehouses.Insert(0, new SelectListItem()
                    {
                        Text = "Select Warehouse Option",
                        Value = string.Empty
                    });
                }
                return _Warehouses;
            }
        }
    }
}