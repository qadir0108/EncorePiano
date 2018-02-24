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
    public class VehiclesController : BaseController
    {
        public ActionResult Index()
        {
            TempData["VehicleTypes"] = new SelectList(VehicleTypesList, "Value", "Text");
            return View();
        }

        public ActionResult Initialize([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            IEnumerable<Vehicle> vehicles = Db.Vehicles.Include(x => x.VehicleType);

            var totalCount = vehicles.Count();

            #region Filtering
            // Apply filters for searching
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                vehicles = vehicles.AsEnumerable().
                                          Where(p => p.Code.Contains(value) || 
                                         p.Name.Contains(value) ||
                                         p.Description.Contains(value)
                                         //p.City.Contains(value) ||
                                         //p.State.Contains(value) ||
                                         //p.Address1.Contains(value)
                                         );
            }

            var filteredCount = vehicles.Count();

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
                        vehicles = column.SortDirection.ToString() == "Ascendant" ?
                                    vehicles.OrderBy(x => x.Code) :
                                    vehicles.OrderByDescending(x => x.Code);
                    }

                    if (column.Data == "Name")
                    {
                        vehicles = column.SortDirection.ToString() == "Ascendant" ?
                                    vehicles.OrderBy(x => x.Name) :
                                    vehicles.OrderByDescending(x => x.Name);
                    }

                    if (column.Data == "Active")
                    {
                        vehicles = column.SortDirection.ToString() == "Ascendant" ?
                                    vehicles.OrderBy(x => x.Active) :
                                     vehicles.OrderByDescending(x => x.Active);
                    }

                    if (column.Data == "VehicleType")
                    {
                        vehicles = column.SortDirection.ToString() == "Ascendant" ?
                                    vehicles.OrderBy(x => x.VehicleType.Name) :
                                     vehicles.OrderByDescending(x => x.VehicleType.Name);
                    }
                }
                orderByString = "Ordered";
            }

            if (orderByString == string.Empty)
            {
                vehicles = vehicles.OrderBy(x =>
                                         x.Name);
            }
            #endregion Sorting

            // Paging
            if (requestModel.Length != -1)
            {
                vehicles = vehicles.Skip(requestModel.Start).Take(requestModel.Length);
            }

            var result = vehicles.
                         ToList()
                        .Select(vehicle => new
                        {
                            Code = vehicle.Code,
                            Name = vehicle.Name,
                            Description = vehicle.Description,
                            Active = vehicle.Active ? "Yes" : "No",
                            VehicleType = vehicle.VehicleType.Name,
                            Actions = GetActions(vehicle.Id),
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
        public ActionResult Save(NewVehicleVm vm)
        {
            try
            {
                if (vm.Id == null)
                {
                    Vehicle vehicle = new Vehicle()
                    {
                        Id = Guid.NewGuid(),
                        Code = vm.Code,
                        Name = vm.Name,
                        VehicleTypeId = Guid.Parse(vm.VehicleTypeId),
                        Description = vm.Description,
                        Active = vm.Active,
                        CreatedAt = DateTime.Now,
                        CreatedBy = LoggedInUser?.UserName
                    };
                    Db.Vehicles.Add(vehicle);
                }
                else
                {
                    var vehcile = Db.Vehicles.FirstOrDefault(x => x.Id == vm.Id);
                    vehcile.Code = vm.Code;
                    vehcile.Name = vm.Name;
                    vehcile.VehicleTypeId = Guid.Parse(vm.VehicleTypeId);
                    vehcile.Description = vm.Description;
                    vehcile.Active = vm.Active;
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
                var vehicle = Db.Vehicles.FirstOrDefault(x => x.Id == id);
                var assignments = Db.Assignments.Where(x => x.VehicleId == vehicle.Id);

                if (assignments.Count() > 0)
                {
                    return Json(new JsonResponse() { IsSucess = false, ErrorMessage = "Unable to process as there are assignments against this record" }, JsonRequestBehavior.AllowGet);
                }
                Db.Vehicles.Remove(vehicle);
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
                var vehicle = Db.Vehicles.Include(x => x.VehicleType).FirstOrDefault(x => x.Id == id);
                var model = new NewVehicleVm()
                {
                    Id = vehicle.Id,
                    Code = vehicle.Code,
                    Name = vehicle.Name,
                    Description = vehicle.Description,
                    Active = vehicle.Active,
                    VehicleTypeId = vehicle.VehicleType.Id.ToString(),
                };
                TempData["VehicleTypes"] = new SelectList(VehicleTypesList, "Value", "Text");
                return PartialView("~/Views/Vehicles/Add.cshtml", model);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}