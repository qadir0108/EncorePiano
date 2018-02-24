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
    public class LocationsController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Initialize([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            IEnumerable<Location> locations = Db.Locations;

            var totalCount = locations.Count();

            #region Filtering
            // Apply filters for searching
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                locations = locations.AsEnumerable().
                                          Where(p => p.Name.Contains(value)
                                         );
            }

            var filteredCount = locations.Count();

            #endregion Filtering

            #region Sorting
            // Sorting
            var sortedColumns = requestModel.Columns.GetSortedColumns();
            var orderByString = String.Empty;

            if (sortedColumns.Count() > 0)
            {
                foreach (var column in sortedColumns)
                {
                    if (column.Data == "Name")
                    {

                        locations = column.SortDirection.ToString() == "Ascendant" ?
                                    locations.OrderBy(x => x.Name) :
                                    locations.OrderByDescending(x => x.Name);
                    }
                }
                orderByString = "Ordered";
            }

            if (orderByString == string.Empty)
            {
                locations = locations.OrderBy(x =>
                                         x.Name);
            }
            #endregion Sorting

            // Paging
            if (requestModel.Length != -1)
            {
                locations = locations.Skip(requestModel.Start).Take(requestModel.Length);
            }

            var result = locations.
                         ToList()
                        .Select(location => new
                        {
                            Name = location.Name,
                            Created = location.CreatedAt.ToString("yyyy-MM-dd"),
                            Actions = GetActions(location.Id),
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
        public ActionResult Save(NewLocationVm vm)
        {
            try
            {
                if (vm.Id == null)
                {
                    Location location = new Location()
                    {
                        Id = Guid.NewGuid(),
                        Name = vm.Name,
                        CreatedAt = DateTime.Now,
                        CreatedBy = LoggedInUser?.UserName
                    };
                    Db.Locations.Add(location);
                }
                else
                {
                    var location = Db.Locations.FirstOrDefault(x => x.Id == vm.Id);
                    location.Name = vm.Name;
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
                var location = Db.Locations.FirstOrDefault(x => x.Id == id);
                var ass = Db.Legs.Where(x => x.FromLocationId == location.Id || x.ToLocationId == location.Id);

                if (ass.Count() > 0)
                {
                    return Json(new JsonResponse() { IsSucess = false, ErrorMessage = "Unable to process as there are assignments against this record" }, JsonRequestBehavior.AllowGet);
                }

                Db.Locations.Remove(location);
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
                var location = Db.Locations.FirstOrDefault(x => x.Id == id);
                var model = new NewLocationVm()
                {
                    Id = location.Id,
                    Name = location.Name,
                };
                return PartialView("~/Views/Locations/Add.cshtml", model);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}