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

namespace WFP.ICT.Web.Controllers
{
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
            IEnumerable<Address> Address = db.Addresses;

            var totalCount = Address.Count();

            #region Filtering
            // Apply filters for searching
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                Address = Address.AsEnumerable().
                                          Where(p => p.Name.Contains(value) ||
                                         p.City.Contains(value) ||
                                         p.State.Contains(value) ||
                                         p.PostCode.Contains(value) ||
                                         p.PhoneNumber.Contains(value) ||
                                         p.Address1.Contains(value) ||
                                         ((AddressTypeEnum)(p.AddressType)).ToString().ToLower().Contains(value.ToLower())
                                         );
            }

            var filteredCount = Address.Count();

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

                        Address = column.SortDirection.ToString() == "Ascendant" ?
                                    Address.OrderBy(x => x.AddressType) :
                                    Address.OrderByDescending(x => x.AddressType);
                    }

                    if (column.Data == "Name")
                    {

                        Address = column.SortDirection.ToString() == "Ascendant" ?
                                    Address.OrderBy(x => x.Name) :
                                    Address.OrderByDescending(x => x.Name);
                    }

                    if (column.Data == "Address")
                    {

                        Address = column.SortDirection.ToString() == "Ascendant" ?
                                    Address.OrderBy(x => x.Address1) :
                                     Address.OrderByDescending(x => x.Address1);
                    }

                    if (column.Data == "City")
                    {

                        Address = column.SortDirection.ToString() == "Ascendant" ?
                                    Address.OrderBy(x => x.City) :
                                     Address.OrderByDescending(x => x.City);
                    }

                    if (column.Data == "State")
                    {

                        Address = column.SortDirection.ToString() == "Ascendant" ?
                                    Address.OrderBy(x => x.State) :
                                     Address.OrderByDescending(x => x.State);
                    }

                    if (column.Data == "ZipCode")
                    {

                        Address = column.SortDirection.ToString() == "Ascendant" ?
                                    Address.OrderBy(x => x.PostCode) :
                                     Address.OrderByDescending(x => x.PostCode);
                    }
                    if (column.Data == "Phone")
                    {

                        Address = column.SortDirection.ToString() == "Ascendant" ?
                                    Address.OrderBy(x => x.PhoneNumber) :
                                     Address.OrderByDescending(x => x.PhoneNumber);
                    }
                    if (column.Data == "Linked")
                    {

                        Address = column.SortDirection.ToString() == "Ascendant" ?
                                    Address.OrderBy(x => x.WarehouseId).ThenBy(x => x.CustomerId) :
                                     Address.OrderByDescending(x => x.WarehouseId).ThenByDescending(x => x.CustomerId);
                    }
                }
                orderByString = "Ordered";
            }

            if (orderByString == string.Empty)
            {
                Address = Address.OrderBy(x =>
                                         x.Name);
            }
            #endregion Sorting

            // Paging
            if (requestModel.Length != -1)
            {
                Address = Address.Skip(requestModel.Start).Take(requestModel.Length);
            }

            var result = Address.
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
                            Location = GetLocationLink(add.Lat, add.Lng),
                            Linked = GetLinkedTo(add.WarehouseId, add.CustomerId),
                            Actions = GetActions(add.Id)
                        });

            return Json(new DataTablesResponse
            (requestModel.Draw, result, filteredCount, totalCount),
                        JsonRequestBehavior.AllowGet);
        }

        public string GetLocationLink(string lng, string lat)
        {
            string action = @"<a href='#' data-tooltip='tooltip' title='Show Map' class='btnMap' data-lat='" + lat + "' data-lng='" + lng + "'>Show Map</a>";

            return action;
        }

        public string GetLinkedTo(Guid? warehouse, Guid? client)
        {
            string action = "No Link";

            if (warehouse != (Guid?)null)
            {
                action = db.Warehouses.FirstOrDefault(x => x.Id == warehouse).Name;
            }
            if (client != (Guid?)null)
            {
                action = db.Clients.FirstOrDefault(x => x.Id == client).Name;
            }

            return action;
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
                if (db.PianoOrders.Where(x => x.DeliveryAddressId == id || x.PickupAddressId == id).Count() > 0)
                {
                    return Json(new JsonResponse() { IsSucess = false, ErrorMessage = "Unable to process as there are orders against this address" }, JsonRequestBehavior.AllowGet);
                }

                var address = db.Addresses.FirstOrDefault(x => x.Id == id);
                db.Addresses.Remove(address);
                db.SaveChanges();
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
                var address = db.Addresses.FirstOrDefault(x => x.Id == id);
                TempData["AddressStates"] = new SelectList(States, "Value", "Text");
                var model = new NewAddressVm()
                {
                    State = address.State,
                    City = address.City,
                    Address = address.Address1,
                    Id = address.Id,
                    Name = address.Name,
                    PhoneNumber = address.PhoneNumber,
                    PostCode = address.PostCode
                };
                return PartialView("~/Views/Address/Edit.cshtml", model);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Save(NewAddressVm address)
        {
            try {

                return Json(new JsonResponse() { IsSucess = true}, JsonRequestBehavior.AllowGet);

            }

            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}