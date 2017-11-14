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
using System.IO;
using WFP.ICT.Web.Async;
using ADSDataDirect.Web.Helpers;

namespace WFP.ICT.Web.Controllers
{
    //[Authorize]
    public class DeliveryStatusController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult InitiliazeOrders([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            IQueryable<PianoPOD> PianoPOD = Db.PianoPODs
                                            .Include(x => x.PianoAssignment)
                                            .Include(x => x.PianoAssignment.PianoOrder)
                                            .Include(x => x.Pictures)
                                            .Include(x => x.Piano)
                                            .Include(x => x.Piano.Statuses)
                                            ;

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
                                          p.ReceivedBy.Contains(value) ||
                                          p.ReceivingTime.Value.ToString("yyyy-MM-dd HH:mm:ss").Contains(value) ||
                                        (( (PianoStatusEnum)p.Piano.Statuses.OrderByDescending(x => x.CreatedAt).First().Status).ToString().ToLower().Equals(value.ToLower()))
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
                                    PianoPOD.OrderBy(x => x.Piano.Statuses) :
                                     PianoPOD.OrderByDescending(x => x.Piano.Statuses);
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

            if (orderByString == string.Empty) {
                PianoPOD = PianoPOD.OrderBy( x=> 
                                    x.PianoAssignment.PianoOrder.OrderNumber);
            }



            #endregion Sorting

            // Paging
            if (requestModel.Length != -1)
            {
                PianoPOD = PianoPOD.Skip(requestModel.Start).Take(requestModel.Length);
            }

            var list = PianoPOD.ToList();
            var result = list.Where(pod => pod != null)
                .Select(pod => new
                {
                    Id = pod.Id,
                    OrderNumber = pod.PianoAssignment.PianoOrder.OrderNumber,
                    ConsignmentNumber = pod.PianoAssignment.AssignmentNumber,
                    Status = ((PianoStatusEnum)(pod.Piano.Statuses.OrderByDescending(y => y.CreatedAt).FirstOrDefault().Status)).ToString(),
                    Recieved = pod.ReceivedBy,
                    Signature = GetSignatureImage(pod.Signature),
                    RecievingDate = pod.ReceivingTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                    Notes = pod.Notes,
                    Print = "Print",
                    Pictures = GetPianoImage(pod.Pictures),

                    BenchesUnloadStatus = pod.BenchesUnloadStatus ? "Yes" : "No",
                    CasterCupsUnloadStatus = pod.CasterCupsUnloadStatus ? "Yes" : "No",
                    CoverUnloadStatus = pod.CoverUnloadStatus ? "Yes" : "No",
                    LampUnloadStatus = pod.LampUnloadStatus ? "Yes" : "No",
                    OwnersManualUnloadStatus = pod.OwnersManualUnloadStatus ? "Yes" : "No"
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
                builder.AppendFormat("<a href='../Images/Sign/{0}' data-lightbox='signature'>", path);
                builder.AppendFormat("<image class='grid-image' src='../Images/Sign/{0}'/>", path);
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
                    builder.AppendFormat("<a href='../Images/Piano/{0}' data-lightbox='piano-images'>", x.Picture);
                    builder.AppendFormat("<image  class='grid-image' src='../Images/Piano/{0}'/>", x.Picture);
                    builder.Append("</a>");
                });
            }
            else
            {
                builder.Append("No image available");
            }
            return builder.ToString();
        }

        public ActionResult GeneratePOD(Guid? id)
        {
            PianoPOD pod = Db.PianoPODs
                                            .Include(x => x.PianoAssignment)
                                            .Include(x => x.PianoAssignment.PianoOrder)
                                            .Include(x => x.Pictures)
                                            .Include(x => x.Piano)
                                            .Include(x => x.Piano.Statuses)
                                            .FirstOrDefault(x => x.Id == id)
                                            ;

            string templateFormsPath = Path.Combine(UploadsFormsPath, pod.PianoAssignment.PianoOrder.DeliveryForm);
            string podFileName = "POD" + pod.PianoAssignment.PianoOrder.OrderNumber + ".pdf";
            string podFilePath = Path.Combine(DownloadsFormsPath, pod.Id.ToString() + ".pdf");

            string signature = Path.Combine(SignPath, pod.Signature);
            string signatureResized = Path.Combine(SignPath, "1"+ pod.Signature);

            if(!System.IO.File.Exists(signatureResized))
                ImageResizer.Resize(signature, signatureResized, 150, 150, true);

            PODFormsGenerater.GeneratePODForm(templateFormsPath, podFilePath, pod.ReceivedBy, pod.ReceivingTime?.ToString(), signatureResized);

            return File(podFilePath, " application/octet-stream", podFileName);
        }


    }
}