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
    public class DriversController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Initialize([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            IEnumerable<Driver> drivers = Db.Drivers;

            var totalCount = drivers.Count();

            #region Filtering
            // Apply filters for searching
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                drivers = drivers.AsEnumerable().
                                          Where(p => p.Name.Contains(value) ||
                                         p.Description.Contains(value) ||
                                         p.Code.Contains(value)
                                         );
            }

            var filteredCount = drivers.Count();

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

                        drivers = column.SortDirection.ToString() == "Ascendant" ?
                                    drivers.OrderBy(x => x.Code) :
                                    drivers.OrderByDescending(x => x.Code);
                    }

                    if (column.Data == "Name")
                    {

                        drivers = column.SortDirection.ToString() == "Ascendant" ?
                                    drivers.OrderBy(x => x.Name) :
                                    drivers.OrderByDescending(x => x.Name);
                    }

                    if (column.Data == "Description")
                    {

                        drivers = column.SortDirection.ToString() == "Ascendant" ?
                                    drivers.OrderBy(x => x.Description) :
                                     drivers.OrderByDescending(x => x.Description);
                    }
                }
                orderByString = "Ordered";
            }

            if (orderByString == string.Empty)
            {
                drivers = drivers.OrderBy(x =>
                                         x.Name);
            }
            #endregion Sorting

            // Paging
            if (requestModel.Length != -1)
            {
                drivers = drivers.Skip(requestModel.Start).Take(requestModel.Length);
            }

            var result = drivers.
                         ToList()
                        .Select(driver => new
                        {
                            Code = driver.Code,
                            Name = driver.Name,
                            Password = driver.Password,
                            Created = driver.CreatedAt.ToString("yyyy-MM-dd"),
                            Description = driver.Description,
                            Actions = GetActions(driver.Id),
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
        public ActionResult Save(NewDriverVm vm)
        {
            try
            {
                if (vm.Id == null)
                {
                    Driver driver = new Driver()
                    {
                        Id = Guid.NewGuid(),
                        Code = vm.Code,
                        Name = vm.Name,
                        Description = vm.Description,
                        Password = vm.Password,
                        CreatedAt = DateTime.Now,
                        CreatedBy = LoggedInUser?.UserName
                    };
                    Db.Drivers.Add(driver);
                }
                else
                {
                    var driver = Db.Drivers.FirstOrDefault(x => x.Id == vm.Id);
                    driver.Code = vm.Code;
                    driver.Name = vm.Name;
                    driver.Description = vm.Description;
                    driver.Password = vm.Password;
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
                var driver = Db.Drivers.FirstOrDefault(x => x.Id == id);
                var ass = Db.Assignments.Include(x => x.Drivers).ToList();

                if (ass.Count(x => x.Drivers.Select(y => y.Id).ToList().Contains(driver.Id)) > 0)
                {
                    return Json(new JsonResponse() { IsSucess = false, ErrorMessage = "Unable to process as there are assignments against this record" }, JsonRequestBehavior.AllowGet);
                }

                var logins = Db.DriverLogins.Where(x => x.DriverId == id);
                Db.DriverLogins.RemoveRange(logins);
                Db.SaveChanges();

                Db.Drivers.Remove(driver);
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
                var driver = Db.Drivers.FirstOrDefault(x => x.Id == id);
                var model = new NewDriverVm()
                {
                    Id = driver.Id,
                    Code = driver.Code,
                    Name = driver.Name,
                    Description = driver.Description,
                    Password = driver.Password,
                };
                return PartialView("~/Views/Drivers/Add.cshtml", model);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}