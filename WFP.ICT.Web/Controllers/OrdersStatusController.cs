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

namespace WFP.ICT.Web.Controllers
{
    //[Authorize]
    public class OrdersStatusController : BaseController
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

        public ActionResult Index()
        {
           
            return View();
        }

        public ActionResult InitiliazeOrders([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            IQueryable<PianoPOD> PianoPOD = db.PianoAssignments.
                                            Select(x => x.PianoPod)
                                            .Include(x => x.PianoAssignment).
                                            Include(x => x.Pictures).
                                            Include(x => x.PianoAssignment.PianoOrder);

            var totalCount = PianoPOD.Count();

            #region Filtering
            // Apply filters for searching
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                PianoPOD = PianoPOD.AsEnumerable().
                                          Where(p => p.PianoAssignment.PianoOrder.OrderNumber.Contains(value) ||
                                          p.PianoAssignment.AssignmentNumber.Contains(value) ||
                                          p.Notes.Contains(value) ||
                                          ((PODStatusEnum)(p.PodStatus)).ToString().ToLower().Contains(value.ToLower()) ||
                                          p.ReceivedBy.Contains(value) ||
                                          p.ReceivingTime.Value.ToString("yyyy-MM-dd HH:mm:ss").Contains(value)
                                         ).AsQueryable();
            }

            var filteredCount = PianoPOD.Count();

            #endregion Filtering

            #region Sorting
            // Sorting
            var sortedColumns = requestModel.Columns.GetSortedColumns();
            var orderByString = String.Empty;

            if(sortedColumns.Count() > 0)
            {
                foreach (var column in sortedColumns)
                {
                    if (column.Data == "OrderNumber")
                    {

                        PianoPOD = column.SortDirection.ToString() == "Ascendant" ?
                                    PianoPOD.OrderBy(x => x.PianoAssignment.PianoOrder.OrderNumber) :
                                    PianoPOD.OrderByDescending(x => x.PianoAssignment.PianoOrder.OrderNumber) ;
                    }

                    if (column.Data == "ConsignmentNumber")
                    {

                        PianoPOD = column.SortDirection.ToString() == "Ascendant" ?
                                    PianoPOD.OrderBy(x => x.PianoAssignment.AssignmentNumber) :
                                    PianoPOD.OrderByDescending(x => x.PianoAssignment.AssignmentNumber) ;
                    }

                    if (column.Data == "Status")
                    {

                        PianoPOD = column.SortDirection.ToString() == "Ascendant" ? 
                                    PianoPOD.OrderBy(x => x.PodStatus) :
                                     PianoPOD.OrderByDescending(x => x.PodStatus);
                    }

                    if (column.Data == "Recieved")
                    {

                        PianoPOD = column.SortDirection.ToString() == "Ascendant" ? 
                                    PianoPOD.OrderBy(x => x.ReceivedBy) :
                                     PianoPOD.OrderByDescending(x => x.ReceivedBy);
                    }

                    if (column.Data == "RecievingDate")
                    {

                        PianoPOD = column.SortDirection.ToString() == "Ascendant" ?
                                    PianoPOD.OrderBy(x => x.ReceivingTime) :
                                     PianoPOD.OrderByDescending(x => x.ReceivingTime);
                    }
                    if (column.Data == "Notes")
                    {

                        PianoPOD = column.SortDirection.ToString() == "Ascendant" ? 
                                    PianoPOD.OrderBy(x => x.Notes) :
                                     PianoPOD.OrderByDescending(x => x.Notes);
                    }
                }
                orderByString = "Ordered";
            }


            PianoPOD = PianoPOD.OrderBy(x => orderByString == string.Empty ? 
                                        x.PianoAssignment.PianoOrder.OrderNumber :
                                        orderByString);

            #endregion Sorting

            // Paging
            PianoPOD = PianoPOD.Skip(requestModel.Start).Take(requestModel.Length);

            var result = PianoPOD.
                         ToList()
                        .Select(pod => new
            {

                OrderNumber = pod.PianoAssignment.PianoOrder.OrderNumber,
                ConsignmentNumber = pod.PianoAssignment.AssignmentNumber,
                Status = ((PODStatusEnum)(pod.PodStatus)).ToString(),
                Recieved = pod.ReceivedBy,
                Signature = GetSignatureImage(pod.Signature),
                RecievingDate = pod.ReceivingTime.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                Notes = pod.Notes,
                Print = "Print",
                Pictures = GetPianoImage(pod.Pictures)
            });

            return Json(new DataTablesResponse
            (requestModel.Draw, result, filteredCount, totalCount),
                        JsonRequestBehavior.AllowGet);
        }

        public string GetSignatureImage(string path)
        {
            StringBuilder builder = new StringBuilder();
            if(path != string.Empty)
            {
                builder.AppendFormat("<a href='../Images/{0}' data-lightbox='signature'>", path);
                builder.AppendFormat("<image class='grid-image' src='../Images/{0}'/>", path);
                builder.Append("</a>");
            }
            else
            {
                builder.Append("Not available");
            }
            return builder.ToString();
        }
        public string GetPianoImage(ICollection<PianoPicture> Pictures)
        {
            StringBuilder builder = new StringBuilder();
            if (Pictures.Count > 0)
            {
                Pictures.ToList().ForEach(x =>
                {
                    builder.AppendFormat("<a href='../Images/{0}' data-lightbox='piano-images'>", x.Picture);
                    builder.AppendFormat("<image  class='grid-image' src='../Images/{0}'/>", x.Picture);
                    builder.Append("</a>");
                });
            }
            else
            {
                builder.Append("No image available");
            }
            return builder.ToString();
        }

    }
}