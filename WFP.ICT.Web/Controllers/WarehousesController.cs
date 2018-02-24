using System.Linq;
using System.Linq.Dynamic;
using System.Data.Entity;
using System.Web.Mvc;
using WFP.ICT.Web.Models;
using WFP.ICT.Enum;
using WFP.ICT.Data.Entities;
using DataTables.Mvc;
using System.Text;
using System.Collections.Generic;
using System;
using WFP.ICT.Web.Helpers;

namespace WFP.ICT.Web.Controllers
{
    [Authorize]
    [AjaxAuthorize]
    //[AuthorizeRole(Roles = SecurityConstants.RoleAdmin)]
    public class WarehousesController : BaseController
    {
        public ActionResult Index()
        {
            TempData["States"] = new SelectList(States, "Value", "Text");
            return View();
        }

        public ActionResult Initialize([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            IEnumerable<Warehouse> warehouses = Db.Warehouses;

            var totalCount = warehouses.Count();

            #region Filtering
            // Apply filters for searching
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                warehouses = warehouses.AsEnumerable().
                                          Where(p => p.Code.Contains(value) || 
                                         p.Name.Contains(value) ||
                                         p.AlternateContact.Contains(value) ||
                                         p.City.Contains(value) ||
                                         p.State.Contains(value) ||
                                         p.Address1.Contains(value)
                                         );
            }

            var filteredCount = warehouses.Count();

            #endregion Filtering

            #region Sorting
            // Sorting
            var sortedColumns = requestModel.Columns.GetSortedColumns();
            var orderByString = String.Empty;

            if (sortedColumns.Count() > 0)
            {
                foreach (var column in sortedColumns)
                {
                    if (column.Data == "Code")
                    {
                        warehouses = column.SortDirection.ToString() == "Ascendant" ?
                                    warehouses.OrderBy(x => x.Code) :
                                    warehouses.OrderByDescending(x => x.Code);
                    }

                    if (column.Data == "Name")
                    {
                        warehouses = column.SortDirection.ToString() == "Ascendant" ?
                                    warehouses.OrderBy(x => x.Name) :
                                    warehouses.OrderByDescending(x => x.Name);
                    }

                    if (column.Data == "City")
                    {
                        warehouses = column.SortDirection.ToString() == "Ascendant" ?
                                    warehouses.OrderBy(x => x.City) :
                                     warehouses.OrderByDescending(x => x.City);
                    }

                    if (column.Data == "State")
                    {
                        warehouses = column.SortDirection.ToString() == "Ascendant" ?
                                    warehouses.OrderBy(x => x.State) :
                                     warehouses.OrderByDescending(x => x.State);
                    }
                }
                orderByString = "Ordered";
            }

            if (orderByString == string.Empty)
            {
                warehouses = warehouses.OrderBy(x =>
                                         x.Name);
            }
            #endregion Sorting

            // Paging
            if (requestModel.Length != -1)
            {
                warehouses = warehouses.Skip(requestModel.Start).Take(requestModel.Length);
            }

            var result = warehouses.
                         ToList()
                        .Select(warehouse => new
                        {
                            Code = warehouse.Code,
                            Name = warehouse.Name,
                            City = warehouse.City,
                            State = warehouse.State,
                            AlternateContact = warehouse.AlternateContact,
                            PhoneNumber = warehouse.PhoneNumber,
                            Actions = GetActions(warehouse.Id),
                        });

            return Json(new DataTablesResponse
            (requestModel.Draw, result, filteredCount, totalCount),
                        JsonRequestBehavior.AllowGet);
        }

        public string GetActions(Guid? id)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"<a href='#' class='btnEdit' data-tooltip='tooltip' data-id='{0}' title='Edit'>
                             <span class='glyphicon glyphicon-pencil'>
                             </span>
                          </a>
                           <a class='btnDelete' data-id={0} href='#' data-tooltip='tooltip' title='Delete'>
                            <span class='glyphicon glyphicon-trash'></span>
                            </a>", id);
            return sb.ToString();
        }

        [HttpPost]
        public ActionResult Save(NewWarehouseVm vm)
        {
            try
            {
                if (vm.Id == null)
                {
                    Warehouse warehouse = new Warehouse()
                    {
                        Id = Guid.NewGuid(),
                        Code = vm.Code,
                        Name = vm.Name,
                        City = vm.City,
                        State = vm.State,
                        AlternateContact = vm.AlternateContact,
                        PhoneNumber = vm.PhoneNumber,
                        AlternatePhone = vm.AlternatePhone,
                        Address1 = vm.Address1,
                        PostCode = vm.PostCode,
                        Notes = vm.Notes,
                        CreatedAt = DateTime.Now,
                        CreatedBy = LoggedInUser?.UserName
                    };
                    Db.Warehouses.Add(warehouse);
                }
                else
                {
                    var warehouse = Db.Warehouses.FirstOrDefault(x => x.Id == vm.Id);
                    warehouse.Code = vm.Code;
                    warehouse.Name = vm.Name;
                    warehouse.City = vm.City;
                    warehouse.State = vm.State;
                    warehouse.AlternateContact = vm.AlternateContact;
                    warehouse.PhoneNumber = vm.PhoneNumber;
                    warehouse.AlternatePhone = vm.AlternatePhone;
                    warehouse.Address1 = vm.Address1;
                    warehouse.PostCode = vm.PostCode;
                    warehouse.Notes = vm.Notes;
                }
                Db.SaveChanges();

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
                var warehouse = Db.Warehouses.FirstOrDefault(x => x.Id == id);
                var addresses = Db.Addresses.Where(x => x.WarehouseId == warehouse.Id);
                var drivers = Db.Drivers.Where(x => x.WarehouseId == warehouse.Id);
                var pianos = Db.Pianos.Where(x => x.WarehouseId == warehouse.Id);

                if (addresses.Count() > 0 || drivers.Count() > 0 || pianos.Count() > 0)
                {
                    return Json(new JsonResponse() { IsSucess = false, ErrorMessage = "Unable to process as there are assignments against this record" }, JsonRequestBehavior.AllowGet);
                }

                Db.Warehouses.Remove(warehouse);
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
                var warehouse = Db.Warehouses.FirstOrDefault(x => x.Id == id);
                var model = new NewWarehouseVm()
                {
                    Id = warehouse.Id,
                    Code = warehouse.Code,
                    Name = warehouse.Name,
                    City = warehouse.City,
                    State = warehouse.State,

                    AlternateContact = warehouse.AlternateContact,
                    PhoneNumber = warehouse.PhoneNumber,
                    AlternatePhone = warehouse.AlternatePhone,
                    Address1 = warehouse.Address1,
                    PostCode = warehouse.PostCode,
                    Notes = warehouse.Notes,
                };
                TempData["States"] = new SelectList(States, "Value", "Text");
                return PartialView("~/Views/Warehouses/Add.cshtml", model);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}