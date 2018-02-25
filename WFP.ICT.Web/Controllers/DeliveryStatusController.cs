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
using WFP.ICT.Enums;
using WFP.ICT.Web.Helpers;

namespace WFP.ICT.Web.Controllers
{
    [Authorize]
    [AjaxAuthorize]
    public class DeliveryStatusController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult InitiliazeOrders([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            IQueryable<Proof> Proofs = Db.Proofs
                                            .Include(x => x.Assignment)
                                            .Include(x => x.Assignment.Order)
                                            .Include(x => x.Pictures)
                                            .Include(x => x.Piano)
                                            .Include(x => x.Piano.Statuses)
                                            .Where(x => x.ProofType == (int)ProofTypeEnum.Delivery)
                                            ;

            var totalCount = Proofs.Count();

            #region Filtering
            // Apply filters for searching
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                Proofs = Proofs.AsEnumerable().
                                          Where(p => p.Assignment.Order.OrderNumber.Contains(value) ||
                                          p.Assignment.AssignmentNumber.Contains(value) ||
                                          p.Notes.Contains(value) ||
                                          p.ReceivedBy.Contains(value) ||
                                          p.ReceivingTime.Value.ToString("yyyy-MM-dd HH:mm:ss").Contains(value) ||
                                        (( (PianoStatusEnum)p.Piano.Statuses.OrderByDescending(x => x.CreatedAt).First().Status).ToString().ToLower().Equals(value.ToLower()))
                                         ).AsQueryable();
            }

            var filteredCount = Proofs.Count();

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

                        Proofs = column.SortDirection.ToString() == "Ascendant" ?
                                    Proofs.OrderBy(x => x.Assignment.Order.OrderNumber) :
                                    Proofs.OrderByDescending(x => x.Assignment.Order.OrderNumber) ;
                    }

                    if (column.Data == "ConsignmentNumber")
                    {

                        Proofs = column.SortDirection.ToString() == "Ascendant" ?
                                    Proofs.OrderBy(x => x.Assignment.AssignmentNumber) :
                                    Proofs.OrderByDescending(x => x.Assignment.AssignmentNumber) ;
                    }

                    if (column.Data == "Status")
                    {

                        Proofs = column.SortDirection.ToString() == "Ascendant" ? 
                                    Proofs.OrderBy(x => x.Piano.Statuses) :
                                     Proofs.OrderByDescending(x => x.Piano.Statuses);
                    }

                    if (column.Data == "Recieved")
                    {

                        Proofs = column.SortDirection.ToString() == "Ascendant" ? 
                                    Proofs.OrderBy(x => x.ReceivedBy) :
                                     Proofs.OrderByDescending(x => x.ReceivedBy);
                    }

                    if (column.Data == "RecievingDate")
                    {

                        Proofs = column.SortDirection.ToString() == "Ascendant" ?
                                    Proofs.OrderBy(x => x.ReceivingTime) :
                                     Proofs.OrderByDescending(x => x.ReceivingTime);
                    }
                    if (column.Data == "Notes")
                    {

                        Proofs = column.SortDirection.ToString() == "Ascendant" ? 
                                    Proofs.OrderBy(x => x.Notes) :
                                     Proofs.OrderByDescending(x => x.Notes);
                    }
                }
                orderByString = "Ordered";
            }

            if (orderByString == string.Empty) {
                Proofs = Proofs.OrderBy( x=> 
                                    x.Assignment.Order.OrderNumber);
            }



            #endregion Sorting

            // Paging
            if (requestModel.Length != -1)
            {
                Proofs = Proofs.Skip(requestModel.Start).Take(requestModel.Length);
            }

            var list = Proofs.ToList();
            var result = list.Where(pod => pod != null)
                .Select(pod => new
                {
                    Id = pod.Id,
                    OrderNumber = pod.Assignment.Order.OrderNumber,
                    ConsignmentNumber = pod.Assignment.AssignmentNumber,
                    Status = ((PianoStatusEnum)(pod.Piano.Statuses.OrderByDescending(y => y.CreatedAt).FirstOrDefault().Status)).ToString(),
                    Recieved = pod.ReceivedBy,
                    Signature = GetSignatureImage(pod.Signature),
                    RecievingDate = pod.ReceivingTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                    Notes = pod.Notes,
                    Form = pod.Assignment.Order.DeliveryForm,
                    Pictures = GetPianoImage(pod.Pictures),

                    Bench1UnloadStatus = pod.Bench1UnloadStatus ? "Yes" : "No",
                    Bench2UnloadStatus = pod.Bench2UnloadStatus ? "Yes" : "No",
                    CasterCupsUnloadStatus = pod.CasterCupsUnloadStatus ? "Yes" : "No",
                    CoverUnloadStatus = pod.CoverUnloadStatus ? "Yes" : "No",
                    LampUnloadStatus = pod.LampUnloadStatus ? "Yes" : "No",
                    OwnersManualUnloadStatus = pod.OwnersManualUnloadStatus ? "Yes" : "No",
                    Misc1UnloadStatus = pod.Misc1UnloadStatus ? "Yes" : "No",
                    Misc2UnloadStatus = pod.Misc2UnloadStatus ? "Yes" : "No",
                    Misc3UnloadStatus = pod.Misc3UnloadStatus ? "Yes" : "No"
                });

            return Json(new DataTablesResponse
            (requestModel.Draw, result, filteredCount, totalCount),
                        JsonRequestBehavior.AllowGet);
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
            Proof pod = Db.Proofs
                                            .Include(x => x.Assignment)
                                            .Include(x => x.Assignment.Order)
                                            .Include(x => x.Pictures)
                                            .Include(x => x.Piano)
                                            .Include(x => x.Piano.Statuses)
                                            .FirstOrDefault(x => x.Id == id)
                                            ;

            string templateFormsPath = Path.Combine(UploadsFormsPath, pod.Assignment.Order.DeliveryForm);
            string podFileName = "POD" + pod.Assignment.Order.OrderNumber + ".pdf";
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