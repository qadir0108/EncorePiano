using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using WFP.ICT.Data.Entities;
using WFP.ICT.Enum;
using System.Text;
using DataTables.Mvc;
using WFP.ICT.Web.Models;
using WFP.ICT.Web.Helpers;

namespace WFP.ICT.Web.Controllers
{
    [Authorize]
    [AjaxAuthorize]
    public class AddressController : BaseController
    {
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

        // GET: Address
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult InitializeAddress([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            IEnumerable<Address> addresses = Db.Addresses;

            var totalCount = addresses.Count();

            #region Filtering
            // Apply filters for searching
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                addresses = addresses.AsEnumerable().
                                          Where(p => ( (!string.IsNullOrEmpty(p.Name) && p.Name.Contains(value)) ||
                                          (!string.IsNullOrEmpty(p.City) && p.City.Contains(value)) ||
                                          (!string.IsNullOrEmpty(p.State) && p.State.Contains(value)) ||
                                          (!string.IsNullOrEmpty(p.PostCode) && p.PostCode.Contains(value)) ||
                                          (!string.IsNullOrEmpty(p.PhoneNumber) && p.PhoneNumber.Contains(value)) ||
                                          (!string.IsNullOrEmpty(p.Address1) && p.Address1.Contains(value))
                                          //((AddressTypeEnum)(p.AddressType)).ToString().ToLower().Contains(value.ToLower()))
                                         ));
            }

            var filteredCount = addresses.Count();

            #endregion Filtering

            #region Sorting
            // Sorting
            var sortedColumns = requestModel.Columns.GetSortedColumns();
            var orderByString = String.Empty;

            if (sortedColumns.Count() > 0)
            {
                foreach (var column in sortedColumns)
                {
                    if (column.Data == "Type")
                    {

                        addresses = column.SortDirection.ToString() == "Ascendant" ?
                                    addresses.OrderBy(x => x.AddressType) :
                                    addresses.OrderByDescending(x => x.AddressType);
                    }

                    if (column.Data == "Name")
                    {

                        addresses = column.SortDirection.ToString() == "Ascendant" ?
                                    addresses.OrderBy(x => x.Name) :
                                    addresses.OrderByDescending(x => x.Name);
                    }

                    if (column.Data == "Address")
                    {

                        addresses = column.SortDirection.ToString() == "Ascendant" ?
                                    addresses.OrderBy(x => x.Address1) :
                                     addresses.OrderByDescending(x => x.Address1);
                    }

                    if (column.Data == "City")
                    {

                        addresses = column.SortDirection.ToString() == "Ascendant" ?
                                    addresses.OrderBy(x => x.City) :
                                     addresses.OrderByDescending(x => x.City);
                    }

                    if (column.Data == "State")
                    {

                        addresses = column.SortDirection.ToString() == "Ascendant" ?
                                    addresses.OrderBy(x => x.State) :
                                     addresses.OrderByDescending(x => x.State);
                    }

                    if (column.Data == "ZipCode")
                    {

                        addresses = column.SortDirection.ToString() == "Ascendant" ?
                                    addresses.OrderBy(x => x.PostCode) :
                                     addresses.OrderByDescending(x => x.PostCode);
                    }
                    if (column.Data == "Phone")
                    {

                        addresses = column.SortDirection.ToString() == "Ascendant" ?
                                    addresses.OrderBy(x => x.PhoneNumber) :
                                     addresses.OrderByDescending(x => x.PhoneNumber);
                    }
                    if (column.Data == "Linked")
                    {

                        addresses = column.SortDirection.ToString() == "Ascendant" ?
                                    addresses.OrderBy(x => x.WarehouseId).ThenBy(x => x.ClientId) :
                                     addresses.OrderByDescending(x => x.WarehouseId).ThenByDescending(x => x.ClientId);
                    }
                }
                orderByString = "Ordered";
            }

            if (orderByString == string.Empty)
            {
                addresses = addresses.OrderBy(x =>
                                         x.Name);
            }
            #endregion Sorting

            // Paging
            if (requestModel.Length != -1)
            {
                addresses = addresses.Skip(requestModel.Start).Take(requestModel.Length);
            }

            var result = addresses.
                         ToList()
                        .Select(add => new
                        {
                            Type = ((AddressTypeEnum)(add.AddressType)).ToString(),
                            Name = add.Name,
                            Address = add.Address1,
                            City = add.City,
                            State = add.State,
                            ZipCode = add.PostCode,
                            Phone = add.PhoneNumber,
                            Location = GetLocationLink(add.Lng, add.Lat, add.Address1, add.City, add.State),
                            Linked = GetLinkedTo(add.Id, add.WarehouseId, add.ClientId ),
                            Actions = GetActions(add.Id)
                        });

            return Json(new DataTablesResponse
            (requestModel.Draw, result, filteredCount, totalCount),
                        JsonRequestBehavior.AllowGet);
        }

        public string GetLocationLink(string lng, string lat , string add , string city , string state)
        {
            string action = @"<a href='#' data-tooltip='tooltip' title='Show Map' class='btnMap' data-lat='" + lat + "' data-add='" + add + "' data-city='" + city + "' data-state='" + state + "' data-lng='" + lng + "'>Show Map</a>";

            return action;
        }

        public string GetLinkedTo(Guid? addressId, Guid? warehouse, Guid? client)
        {
            string linkedTo = "No Link";

            if (addressId != (Guid?)null)
            {
                var order = Db.Orders.FirstOrDefault(x => x.DeliveryAddressId == addressId || x.PickupAddressId == addressId);
                linkedTo = $"Order # {order?.OrderNumber}";
            }
            if (warehouse != (Guid?)null)
            {
                var warehouseDb = Db.Warehouses.FirstOrDefault(x => x.Id == warehouse);
                linkedTo = warehouseDb?.Name;
            }
            if (client != (Guid?)null)
            {
                var clientDb = Db.Clients.FirstOrDefault(x => x.Id == client);
                linkedTo = clientDb?.Name;
            }
            return linkedTo;
        }

        public string GetActions(Guid? id)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"<a href='#' class='btnEdit' data-tooltip='tooltip' data-id='{0}' title='Edit Address'>
                             <span class='glyphicon glyphicon-pencil'>
                             </span>
                          </a>
                           <a class='btnDelete' data-id={0} href='#' data-tooltip='tooltip' title='Delete Address'>
                            <span class='glyphicon glyphicon-trash'></span>
                            </a>", id);
            return sb.ToString();
        }

        [HttpPost]
        public ActionResult Delete(Guid? id)
        {
            try
            {
                if (Db.Orders.Where(x => x.DeliveryAddressId == id || x.PickupAddressId == id).Count() > 0)
                {
                    return Json(new JsonResponse() { IsSucess = false, ErrorMessage = "Unable to process as there are orders against this address" }, JsonRequestBehavior.AllowGet);
                }

                var client = Db.Clients.FirstOrDefault(x => x.AddressId == id);
                if(client != null)
                {
                    return Json(new JsonResponse() { IsSucess = false, ErrorMessage = "Unable to process as there is client against this address" }, JsonRequestBehavior.AllowGet);
                }

                var address = Db.Addresses.FirstOrDefault(x => x.Id == id);
                Db.Addresses.Remove(address);
                Db.SaveChanges();
                return Json(new JsonResponse() { IsSucess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Edit(Guid? id)
        {
            try
            {
                var address = Db.Addresses.FirstOrDefault(x => x.Id == id);
                TempData["AddressStates"] = new SelectList(States, "Value", "Text");
                var model = new NewAddressVm()
                {
                    State = address.State,
                    City = address.City,
                    Address = address.Address1,
                    Id = address.Id,
                    Name = address.Name,
                    PhoneNumber = address.PhoneNumber,
                    PostCode = address.PostCode,
                    lat = address.Lat,
                    lng = address.Lng
                };
                return PartialView("~/Views/Address/Edit.cshtml", model);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Save(NewAddressVm NewAddressVm)
        {
            try {
                var address = Db.Addresses.FirstOrDefault(x => x.Id == NewAddressVm.Id);

                address.Address1 = NewAddressVm.Address;
                address.City = NewAddressVm.City;
                address.State = NewAddressVm.State;
                address.PostCode = NewAddressVm.PostCode;
                address.Lat = NewAddressVm.lat;
                address.Lng = NewAddressVm.lng;
                address.Name = NewAddressVm.Name;
                address.PhoneNumber = NewAddressVm.PhoneNumber;
                Db.SaveChanges();

                return Json(new JsonResponse() { IsSucess = true}, JsonRequestBehavior.AllowGet);

            }

            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}