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
    public class PianoChargesController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Initialize([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            IEnumerable<PianoCharges> pianoCharges = Db.PianoCharges;

            var totalCount = pianoCharges.Count();

            #region Filtering
            // Apply filters for searching
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                pianoCharges = pianoCharges.AsEnumerable().
                                          Where(p =>
                                          p.Code.ToString().Contains(value) ||
                                          p.Details.Contains(value)
                                         );
            }

            var filteredCount = pianoCharges.Count();

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
                        pianoCharges = column.SortDirection.ToString() == "Ascendant" ?
                                    pianoCharges.OrderBy(x => x.Code) :
                                    pianoCharges.OrderByDescending(x => x.Code);
                    }

                    if (column.Data == "Details")
                    {
                        pianoCharges = column.SortDirection.ToString() == "Ascendant" ?
                                    pianoCharges.OrderBy(x => x.Details) :
                                    pianoCharges.OrderByDescending(x => x.Details);
                    }

                    if (column.Data == "Amount")
                    {
                        pianoCharges = column.SortDirection.ToString() == "Ascendant" ?
                                    pianoCharges.OrderBy(x => x.Amount) :
                                    pianoCharges.OrderByDescending(x => x.Amount);
                    }
                }
                orderByString = "Ordered";
            }

            if (orderByString == string.Empty)
            {
                pianoCharges = pianoCharges.OrderBy(x =>
                                         x.Code);
            }
            #endregion Sorting

            // Paging
            if (requestModel.Length != -1)
            {
                pianoCharges = pianoCharges.Skip(requestModel.Start).Take(requestModel.Length);
            }

            var result = pianoCharges.
                         ToList()
                        .Select(location => new
                        {
                            Code = location.Code,
                            Details = location.Details,
                            Amount = location.Amount,
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
        public ActionResult Save(NewPianoChargesVm vm)
        {
            try
            {
                if (vm.Id == null)
                {
                    PianoCharges pianoCharge = new PianoCharges()
                    {
                        Id = Guid.NewGuid(),
                        Code = vm.Code,
                        Details = vm.Details,
                        Amount = vm.Amount,
                        CreatedAt = DateTime.Now,
                        CreatedBy = LoggedInUser?.UserName
                    };
                    Db.PianoCharges.Add(pianoCharge);
                }
                else
                {
                    var pianoCharge = Db.PianoCharges.FirstOrDefault(x => x.Id == vm.Id);
                    pianoCharge.Code = vm.Code;
                    pianoCharge.Details = vm.Details;
                    pianoCharge.Amount = vm.Amount;
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
                var pianoCharge = Db.PianoCharges.FirstOrDefault(x => x.Id == id);
                var pianoCharges = Db.OrderCharges.Where(x => x.PianoChargesId == pianoCharge.Id);

                if (pianoCharges.Count() > 0)
                {
                    return Json(new JsonResponse() { IsSucess = false, ErrorMessage = "Unable to process as there are pianos against this record" }, JsonRequestBehavior.AllowGet);
                }

                Db.PianoCharges.Remove(pianoCharge);
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
                var pianoCharge = Db.PianoCharges.FirstOrDefault(x => x.Id == id);
                var model = new NewPianoChargesVm()
                {
                    Id = pianoCharge.Id,
                    Code = pianoCharge.Code,
                    Details = pianoCharge.Details,
                    Amount = pianoCharge.Amount
                };
                return PartialView("~/Views/PianoCharges/Add.cshtml", model);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}