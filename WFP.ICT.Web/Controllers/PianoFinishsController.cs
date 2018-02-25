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
    public class PianoFinishsController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Initialize([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            IEnumerable<PianoFinish> pianoFinishs = Db.PianoFinish;

            var totalCount = pianoFinishs.Count();

            #region Filtering
            // Apply filters for searching
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                pianoFinishs = pianoFinishs.AsEnumerable().
                                          Where(p =>
                                          p.Code.Contains(value) ||
                                          p.Name.Contains(value)
                                         );
            }

            var filteredCount = pianoFinishs.Count();

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
                        pianoFinishs = column.SortDirection.ToString() == "Ascendant" ?
                                    pianoFinishs.OrderBy(x => x.Code) :
                                    pianoFinishs.OrderByDescending(x => x.Code);
                    }

                    if (column.Data == "Name")
                    {
                        pianoFinishs = column.SortDirection.ToString() == "Ascendant" ?
                                    pianoFinishs.OrderBy(x => x.Name) :
                                    pianoFinishs.OrderByDescending(x => x.Name);
                    }
                }
                orderByString = "Ordered";
            }

            if (orderByString == string.Empty)
            {
                pianoFinishs = pianoFinishs.OrderBy(x =>
                                         x.Name);
            }
            #endregion Sorting

            // Paging
            if (requestModel.Length != -1)
            {
                pianoFinishs = pianoFinishs.Skip(requestModel.Start).Take(requestModel.Length);
            }

            var result = pianoFinishs.
                         ToList()
                        .Select(location => new
                        {
                            Code = location.Code,
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
        public ActionResult Save(NewPianoMakeVm vm)
        {
            try
            {
                if (vm.Id == null)
                {
                    PianoFinish pianoFinish = new PianoFinish()
                    {
                        Id = Guid.NewGuid(),
                        Code = vm.Code,
                        Name = vm.Name,
                        CreatedAt = DateTime.Now,
                        CreatedBy = LoggedInUser?.UserName
                    };
                    Db.PianoFinish.Add(pianoFinish);
                }
                else
                {
                    var pianoFinish = Db.PianoFinish.FirstOrDefault(x => x.Id == vm.Id);
                    pianoFinish.Code = vm.Code;
                    pianoFinish.Name = vm.Name;
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
                var pianoFinish = Db.PianoFinish.FirstOrDefault(x => x.Id == id);
                var pianos = Db.Pianos.Where(x => x.PianoMakeId == pianoFinish.Id);

                if (pianos.Count() > 0)
                {
                    return Json(new JsonResponse() { IsSucess = false, ErrorMessage = "Unable to process as there are pianos against this record" }, JsonRequestBehavior.AllowGet);
                }

                Db.PianoFinish.Remove(pianoFinish);
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
                var pianoFinish = Db.PianoFinish.FirstOrDefault(x => x.Id == id);
                var model = new NewPianoMakeVm()
                {
                    Id = pianoFinish.Id,
                    Code = pianoFinish.Code,
                    Name = pianoFinish.Name,
                };
                return PartialView("~/Views/PianoFinishs/Add.cshtml", model);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}