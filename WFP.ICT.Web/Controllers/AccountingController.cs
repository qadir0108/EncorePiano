using System;
using System.Linq;
using System.Linq.Dynamic;
using System.Data.Entity;
using System.Web.Mvc;
using WFP.ICT.Data.Entities;
using WFP.ICT.Enum;
using DataTables.Mvc;
using WFP.ICT.Common;
using WFP.ICT.Web.Models;
using System.Data;
using System.Collections.Generic;
using WFP.ICT.Web.Async;
using WFP.ICT.Web.Helpers;

namespace WFP.ICT.Web.Controllers
{
    [Authorize]
    [AjaxAuthorize]
    public class AccountingController : BaseController
    {
        // GET: Accounting
        public ActionResult Invoice()
        {
            TempData["Clients"] = new SelectList(CustomersList, "Value", "Text");

            return View(new InvoiceViewModel());
        }

        public ActionResult InitializeInvoiceGrid([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel,
                                         string Client, string StartDate, string EndDate)
        {
            if (Client == string.Empty || StartDate == null || EndDate == null)
            {
                return Json(new DataTablesResponse
                             (requestModel.Draw,
                              Enumerable.Empty<Piano>(),
                              0, 0),
                              JsonRequestBehavior.AllowGet);
            }

            IEnumerable<Order> Orders = GetOrders(Client, StartDate, EndDate);

            List<Piano> Pianos = new List<Piano>();

            foreach (var item in Orders)
            {
                List<Piano> PianosNew = item.Pianos.ToList();
                Pianos.AddRange(PianosNew);
            }
            var totalCount = Pianos.Count();

            #region Filtering
            // Apply filters for searching
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                Pianos = Pianos.Where(p => p.PianoType != null ? p.PianoType.Type.Contains(value) : p.SerialNumber.Contains(value) ||
                                               p.PianoMake != null ? p.PianoMake.Name.Contains(value) : p.SerialNumber.Contains(value)  ||
                                                p.SerialNumber.Contains(value) ||
                                                p.Model.Contains(value) ||
                                                p.PianoMake != null ? p.Client.Name.Contains(value) : p.SerialNumber.Contains(value) ||
                                                 p.PianoSize != null ? PianoSizeConversion.GetFeetInches(p.PianoSize.Width).Contains(value) : p.SerialNumber.Contains(value) 
                                         ).ToList();
            }
            var filteredCount = Pianos.Count();

            #endregion Filtering

            #region Sorting
            // Sorting
            var sortedColumns = requestModel.Columns.GetSortedColumns();
            var orderByString = String.Empty;

            if (sortedColumns.Count() > 0)
            {
                foreach (var column in sortedColumns)
                {
                    if (column.Data == "Type")
                    {

                        Pianos = column.SortDirection.ToString() == "Ascendant" ?
                                    Pianos.OrderBy(x => x.PianoType == null ? x.SerialNumber : x.PianoType.Type).ToList() :
                                    Pianos.OrderByDescending(x => x.PianoType == null ? x.SerialNumber : x.PianoType.Type).ToList();
                    }
                    if (column.Data == "Size")
                    {

                        Pianos = column.SortDirection.ToString() == "Ascendant" ?
                                    Pianos.OrderBy(x => x.PianoSize.Width).ToList() :
                                    Pianos.OrderByDescending(x => x.PianoSize.Width).ToList();
                    }
                    if (column.Data == "Make")
                    {

                        Pianos = column.SortDirection.ToString() == "Ascendant" ?
                                    Pianos.OrderBy(x => x.PianoMake.Name).ToList() :
                                    Pianos.OrderByDescending(x => x.PianoMake.Name).ToList();
                    }
                    if (column.Data == "Model")
                    {

                        Pianos = column.SortDirection.ToString() == "Ascendant" ?
                                    Pianos.OrderBy(x => x.Model).ToList() :
                                    Pianos.OrderByDescending(x => x.Model).ToList();
                    }
                    if (column.Data == "IsBench")
                    {

                        Pianos = column.SortDirection.ToString() == "Ascendant" ?
                                    Pianos.OrderBy(x => x.IsBench).ToList() :
                                    Pianos.OrderByDescending(x => x.IsBench).ToList();
                    }
                    if (column.Data == "IsPlayer")
                    {

                        Pianos = column.SortDirection.ToString() == "Ascendant" ?
                                    Pianos.OrderBy(x => x.IsPlayer).ToList() :
                                    Pianos.OrderByDescending(x => x.IsPlayer).ToList();
                    }
                    if (column.Data == "IsBoxed")
                    {

                        Pianos = column.SortDirection.ToString() == "Ascendant" ?
                                    Pianos.OrderBy(x => x.IsBoxed).ToList() :
                                    Pianos.OrderByDescending(x => x.IsBoxed).ToList();
                    }
                    if (column.Data == "Name")
                    {

                        Pianos = column.SortDirection.ToString() == "Ascendant" ?
                                    Pianos.OrderBy(x => x.Client.Name).ToList() :
                                    Pianos.OrderByDescending(x => x.Client.Name).ToList();
                    }
                    if (column.Data == "SerialNumber")
                    {

                        Pianos = column.SortDirection.ToString() == "Ascendant" ?
                                    Pianos.OrderBy(x => x.SerialNumber).ToList() :
                                    Pianos.OrderByDescending(x => x.SerialNumber).ToList();
                    }


                }
                orderByString = "Ordered";
            }

            if (orderByString == string.Empty)
            {
                Pianos = Pianos.OrderBy((x => x.Id)).ToList();
            }


            #endregion Sorting

            // Paging
            if (requestModel.Length != -1)
            {
                Pianos = Pianos.Skip(requestModel.Start).Take(requestModel.Length).ToList();
            }


            var result = Pianos
                        .Select(x => new
                        {

                            Type = x.PianoType == null ? "No Data" : x.PianoType.Type,
                            Size = x.PianoSize == null ? "No Data" : PianoSizeConversion.GetFeetInches(x.PianoSize.Width),
                            Make = x.PianoMake == null ? "No Data" : x.PianoMake.Name,
                            Model = x.Model,
                            SerialNumber = x.SerialNumber,
                            IsBench = x.IsBench ? "Yes" : "No",
                            IsPlayer = x.IsPlayer ? "Yes" : "No",
                            IsBoxed = x.IsBoxed ? "Yes" : "No",
                            Name = x.Client == null ? "No Data" : x.Client.Name
                        });

            return Json(new DataTablesResponse
                        (requestModel.Draw, result, totalCount, totalCount),
                        JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadInvoice(string ClientId, string StartDate, string EndDate)
        {
            if (ClientId == string.Empty || StartDate == null || EndDate == null)
            {
                return File(new byte[0], "application/octet-stream", "Error.pdf");
            }

            Client client = Db.Clients.Include(x => x.Address).FirstOrDefault(x => x.Id.ToString() == ClientId);

            IEnumerable<Order> Orders = GetOrders(ClientId, StartDate, EndDate);
            
            List<Piano> Pianos = new List<Piano>();

            foreach (var item in Orders)
            {
                List<Piano> PianosNew = item.Pianos.ToList();
                Pianos.AddRange(PianosNew);
            }

            int invoiceNumber = Db.Invoices.Count() + 3000 ;
            long totalAmount = 0;
            string html = InvoiceHtmlHelper.GenerateClientInvoiceHtml(Orders,client,Pianos , invoiceNumber, out totalAmount);

            JsonResponse Path = HtmlToPdf(html, invoiceNumber.ToString());

            SaveInvoice(ClientId, StartDate, EndDate, invoiceNumber, totalAmount, InvoiceStatusEnum.Generated);

            string pathValue = Path.Result.ToString();
            if (!System.IO.File.Exists(pathValue))
                return File(new byte[0], "application/octet-stream", "Error.pdf");
            
            return File(pathValue, "application/pdf", "Invoice.pdf");

        }

        private void SaveInvoice(string ClientId, string StartDate, string EndDate, int InvoiceNumber, long totalAmount, InvoiceStatusEnum status)
        {
            DateTime StartDateParse = DateTime.Parse(StartDate);
            DateTime EndDateParse = DateTime.Parse(EndDate);

            var ifExists = Db.Invoices.Any(x => x.ClientId.ToString() == ClientId && x.StartDate == StartDateParse && x.EndDate == EndDateParse);
            if (ifExists) return;

            var invoice = new ClientInvoice()
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,

                ClientId = Guid.Parse(ClientId),
                StartDate = StartDateParse,
                EndDate = EndDateParse,
                GeneratedAt = DateTime.Now,

                InvoiceNumber = InvoiceNumber.ToString(),
                Amount = totalAmount,
                Status = (int)status,
                Notes = "System generated invoice",
            };

            if (status == InvoiceStatusEnum.Sent)
                invoice.SentOn = DateTime.Now;
            
            Db.Invoices.Add(invoice);
            Db.SaveChanges();
        }

        public ActionResult EmailInvoice(string ClientId, string StartDate, string EndDate)
        {
            try
            {
                if (ClientId == string.Empty || StartDate == null || EndDate == null)
                {
                    return Json(new { IsSucess = false }, JsonRequestBehavior.AllowGet);
                }
              

                Client client = Db.Clients.Include(x => x.Address).FirstOrDefault(x => x.Id.ToString() == ClientId);

                IEnumerable<Order> Orders = GetOrders( ClientId, StartDate, EndDate);
                List<Piano> Pianos = new List<Piano>();

                foreach (var item in Orders)
                {
                    List<Piano> PianosNew = item.Pianos.ToList();
                    Pianos.AddRange(PianosNew);
                }

                int invoiceNumber = Db.Invoices.Count() + 3000;
                long totalAmount = 0;
                string html = InvoiceHtmlHelper.GenerateClientInvoiceHtml(Orders, client, Pianos, invoiceNumber, out totalAmount);

                JsonResponse Path = HtmlToPdf(html, invoiceNumber.ToString());

                string pathValue = Path.Result.ToString();

                string subject = "Client Invoice";
                string body = string.Format(@"<p>Please find the attached invoice for client {0} </p>",
                                client.Name);
                List<string> attachment = new List<string>();

                attachment.Add(pathValue);

                EmailHelper.SendEmail(client.EmailAddress, subject, body, null, attachment);

                SaveInvoice(ClientId, StartDate, EndDate, invoiceNumber, totalAmount, InvoiceStatusEnum.Sent);

                return Json(new { IsSucess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse{ErrorMessage = ex.ToString(), IsSucess = false }, JsonRequestBehavior.AllowGet);
                throw;
            }

        }

        public JsonResponse HtmlToPdf(string html, string InvoiceNumber)
        {
            try
            {
                var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
                var pdfBytes = htmlToPdf.GeneratePdf(html);

                string Path = Server.MapPath("~/Downloads/ClientInvoices");
                if (!System.IO.Directory.Exists(Path)) System.IO.Directory.CreateDirectory(Path);
                string filePath = System.IO.Path.Combine(Path, InvoiceNumber+".pdf");

                using (var fileStream = System.IO.File.Create(filePath))
                {
                    fileStream.Write(pdfBytes, 0, pdfBytes.Length);
                }
                return new JsonResponse() { IsSucess = true, Result = filePath };
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        public IEnumerable<Order> GetOrders(string ClientId, string StartDate, string EndDate)
        {
            DateTime StartDateParse = DateTime.Parse(StartDate);
            DateTime EndDateParse = DateTime.Parse(EndDate);

            return Db.Orders
                   .Include(y => y.Pianos.Select(z => z.PianoMake))
                   .Include(y => y.Pianos.Select(z => z.PianoType))
                   .Include(y => y.Pianos.Select(z => z.PianoSize))
                    .Include(y => y.Pianos.Select(z => z.Client))
                    .Include(y => y.OrderCharges)
                   .AsEnumerable()
                   .Where(x => x.ClientId.ToString() == ClientId &&
                    x.CreatedAt.Date >= StartDateParse.Date &&
                    x.CreatedAt <= EndDateParse.Date);

            
        }

    }
}