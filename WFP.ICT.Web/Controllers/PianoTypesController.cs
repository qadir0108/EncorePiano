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
    //[Authorize]
    //[AjaxAuthorize]
    //[AuthorizeRole(Roles = SecurityConstants.RoleAdmin)]
    public class PianoTypesController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Initialize([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            IEnumerable<PianoType> pianoTypes = Db.PianoTypes;

            var totalCount = pianoTypes.Count();

            #region Filtering
            // Apply filters for searching
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                pianoTypes = pianoTypes.AsEnumerable().
                                          Where(p =>
                                          p.Code.Contains(value) ||
                                          p.Type.Contains(value)
                                         );
            }

            var filteredCount = pianoTypes.Count();

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
                        pianoTypes = column.SortDirection.ToString() == "Ascendant" ?
                                    pianoTypes.OrderBy(x => x.Code) :
                                    pianoTypes.OrderByDescending(x => x.Code);
                    }

                    if (column.Data == "Type")
                    {
                        pianoTypes = column.SortDirection.ToString() == "Ascendant" ?
                                    pianoTypes.OrderBy(x => x.Type) :
                                    pianoTypes.OrderByDescending(x => x.Type);
                    }
                }
                orderByString = "Ordered";
            }

            if (orderByString == string.Empty)
            {
                pianoTypes = pianoTypes.OrderBy(x =>
                                         x.Type);
            }
            #endregion Sorting

            // Paging
            if (requestModel.Length != -1)
            {
                pianoTypes = pianoTypes.Skip(requestModel.Start).Take(requestModel.Length);
            }

            var result = pianoTypes.
                         ToList()
                        .Select(location => new
                        {
                            Code = location.Code,
                            Type = location.Type,
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
        public ActionResult Save(NewPianoTypeVm vm)
        {
            try
            {
                if (vm.Id == null)
                {
                    PianoType pianoType = new PianoType()
                    {
                        Id = Guid.NewGuid(),
                        Code = vm.Code,
                        Type = vm.Type,
                        CreatedAt = DateTime.Now,
                        CreatedBy = LoggedInUser?.UserName
                    };
                    Db.PianoTypes.Add(pianoType);
                }
                else
                {
                    var pianoType = Db.PianoTypes.FirstOrDefault(x => x.Id == vm.Id);
                    pianoType.Code = vm.Code;
                    pianoType.Type = vm.Type;
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
                var pianoType = Db.PianoTypes.FirstOrDefault(x => x.Id == id);
                var pianos = Db.Pianos.Where(x => x.PianoTypeId == pianoType.Id );

                if (pianos.Count() > 0)
                {
                    return Json(new JsonResponse() { IsSucess = false, ErrorMessage = "Unable to process as there are pianos against this record" }, JsonRequestBehavior.AllowGet);
                }

                var pianoSizes = Db.PianoSize.Where(x => x.PianoTypeId == pianoType.Id);
                Db.PianoSize.RemoveRange(pianoSizes);
                Db.SaveChanges();

                Db.PianoTypes.Remove(pianoType);
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
                var pianoType = Db.PianoTypes.FirstOrDefault(x => x.Id == id);
                var model = new NewPianoTypeVm()
                {
                    Id = pianoType.Id,
                    Code = pianoType.Code,
                    Type = pianoType.Type,
                };
                return PartialView("~/Views/PianoTypes/Add.cshtml", model);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}