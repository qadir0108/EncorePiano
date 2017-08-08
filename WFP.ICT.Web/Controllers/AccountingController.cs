using System;
using System.Linq;
using System.Linq.Dynamic;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using WFP.ICT.Data.Entities;
using WFP.ICT.Enum;
using DataTables.Mvc;
using WFP.ICT.Common;
using System.IO;
using WFP.ICT.Web.Models;
using System.Data;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections.Generic;
using WFP.ICT.Web.Async;
using System.Text;

namespace WFP.ICT.Web.Controllers
{
    public class AccountingController : BaseController
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

            DateTime StartDateParse = DateTime.Parse(StartDate);
            DateTime EndDateParse = DateTime.Parse(EndDate);

            IEnumerable<PianoOrder> Orders = db.PianoOrders
                                            .Include(y => y.Pianos.Select(z => z.PianoMake))
                                            .Include(y => y.Pianos.Select(z => z.PianoType))
                                            .Include(y => y.Pianos.Select(z => z.PianoSize))
                                             .Include(y => y.Pianos.Select(z => z.Client))
                                            .AsEnumerable()
                                            .Where(x => x.CustomerId.ToString() == Client &&
                                             x.CreatedAt.Date >= StartDateParse.Date &&
                                             x.CreatedAt <= EndDateParse.Date);

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
                Pianos = Pianos.Where(p => p.PianoType.Type.Contains(value) ||
                                                p.PianoMake.Name.Contains(value) ||
                                                p.SerialNumber.Contains(value) ||
                                                p.Model.Contains(value) ||
                                                p.Client.Name.Contains(value) ||
                                                PianoSizeConversion.GetFeetInches(p.PianoSize.Width).Contains(value)
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
                                    Pianos.OrderBy(x => x.PianoType.Type).ToList() :
                                    Pianos.OrderByDescending(x => x.PianoType.Type).ToList();
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
                return File(new byte[0], "application/octet-stream", "PickupTickets.pdf");
            }


            DateTime StartDateParse = DateTime.Parse(StartDate);
            DateTime EndDateParse = DateTime.Parse(EndDate);

            Client client = db.Clients.Include(x => x.Addresses).FirstOrDefault(x => x.Id.ToString() == ClientId);

            IEnumerable<PianoOrder> Orders = db.PianoOrders
                                .Include(y => y.Pianos.Select(z => z.PianoMake))
                                .Include(y => y.Pianos.Select(z => z.PianoType))
                                .Include(y => y.Pianos.Select(z => z.PianoSize))
                                 .Include(y => y.Pianos.Select(z => z.Client))
                                 .Include(y => y.OrderCharges)
                                .AsEnumerable()
                                .Where(x => x.CustomerId.ToString() == ClientId &&
                                 x.CreatedAt.Date >= StartDateParse.Date &&
                                 x.CreatedAt <= EndDateParse.Date);
            List<Piano> Pianos = new List<Piano>();

            foreach (var item in Orders)
            {
                List<Piano> PianosNew = item.Pianos.ToList();
                Pianos.AddRange(PianosNew);
            }

            int invoiceNumber = db.CustomerInvoices.Count() + 3000 ;

            string html = GeneratePdf(Orders,client,Pianos , invoiceNumber);

            JsonResponse Path = HtmlToPdf(html, invoiceNumber.ToString());

            string pathValue = Path.Result.ToString();

            if (!System.IO.File.Exists(pathValue))
                return File(new byte[0], "application/octet-stream", "Error.pdf");

            return File(pathValue, "application/pdf", "Invoice.pdf");


          ;
        }

        public ActionResult EmailInvoice(string ClientId, string StartDate, string EndDate)
        {
            try
            {
                if (ClientId == string.Empty || StartDate == null || EndDate == null)
                {
                    return Json(new { IsSucess = false }, JsonRequestBehavior.AllowGet);
                }
                DateTime StartDateParse = DateTime.Parse(StartDate);
                DateTime EndDateParse = DateTime.Parse(EndDate);


                Client client = db.Clients.Include(x => x.Addresses).FirstOrDefault(x => x.Id.ToString() == ClientId);

                IEnumerable<PianoOrder> Orders = db.PianoOrders
                                    .Include(y => y.Pianos.Select(z => z.PianoMake))
                                    .Include(y => y.Pianos.Select(z => z.PianoType))
                                    .Include(y => y.Pianos.Select(z => z.PianoSize))
                                     .Include(y => y.Pianos.Select(z => z.Client))
                                     .Include(y => y.OrderCharges)
                                    .AsEnumerable()
                                    .Where(x => x.CustomerId.ToString() == ClientId &&
                                     x.CreatedAt.Date >= StartDateParse.Date &&
                                     x.CreatedAt <= EndDateParse.Date);
                List<Piano> Pianos = new List<Piano>();

                foreach (var item in Orders)
                {
                    List<Piano> PianosNew = item.Pianos.ToList();
                    Pianos.AddRange(PianosNew);
                }

                int invoiceNumber = db.CustomerInvoices.Count() + 3000;

                string html = GeneratePdf(Orders, client, Pianos, invoiceNumber);

                JsonResponse Path = HtmlToPdf(html, invoiceNumber.ToString());

                string pathValue = Path.Result.ToString();

                string subject = "Client Invoice";
                string body = string.Format(@"<p>Please find the attached invoice for client {0} </p>",
                                client.Name);
                List<string> attachment = new List<string>();

                attachment.Add(pathValue);

                EmailHelper.SendEmail(client.EmailAddress, subject, body, null, attachment);

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

                string Path = Server.MapPath("~/Uploads/ClientInvoices");
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

        public static string GeneratePdf(IEnumerable<PianoOrder> Orders ,Client client, List<Piano> Pianos ,int invoiceNumber)
        {
            StringBuilder html = new StringBuilder();


            html.Append(@"<!DOCTYPE html><html><head><style>
                                    body{border:solid 1px #efefef;}
                                    table {color:#2C3E50;
                                    table-layout:fixed; font-family: arial,text-
                                    align:left; sans-serif; padding: 5px; border-
                                    collapse:collapse; width:100%; border-spacing:0px }
                                                                       
                                 	table td{padding:3px;}

                                    table.striped tr:nth-child(odd) {background-color: #f0f0f0 } 
                                    table.striped tr:nth-child(even) {background-color: #f7f7f7}
                                    table tr.header-row{background-color:#8cb1d6  !important; color :#ffffff;} 
                                    table.bordered {border: 1px solid #dddddd;}
                                    table.bordered td{border: 1px solid #dddddd;}
									table td.right{ text-align:right;}
                                    table span.bold{font-weight:600;} 
                                    table span.blue-head{font-weight:600;
                                    color:#5E738B;
                                    font-size:28px;
                                    } 
                                     table span.under{text-decoration:underline;}
                                     table span.bill{font-size:20px;}
                                    table span.heading-cmp{
                                    font-weight:600; 
                                    font-size:28px;}
                                    </style>
                                    </head>
                                    <body>");

            html.AppendFormat(@"<table><tr>
                                    <td colspan='3'><span class='heading-cmp'>{0} </span> <br /><span class='bold'>{1} <br />{2}  <br />{3} <br /><span/></td>
                                     <td colspan='1'>
                                    <span class='blue-head under'>Invoice</span> <br />
                                    <span class='bold'>Date : {4} <br />
                                    Invoice # {5} <br />
                                    Customer Id : {6}</span><br />  </td>
                                    </tr>   ", client.Name,"State"/*client.Addresses.Address1*/,"City"/*client.Addresses.City*/,"State"/*client.Addresses.State*/, DateTime.Now.ToString("yyyy-MM-dd"), invoiceNumber,
                                    client.AccountCode);

            html.Append(@"</table>");

            html.AppendFormat(@"<table><tr>
                                    <td><span class='bold under bill'>Bill To</span><br />
                                    {0}<br />
                                    {1}<br />
                                    {2}<br />
                                    {3}<br />
                                    </td>
                                    </tr>", DateTime.Now.ToString("yyyy-MM-dd"), "2211", client.AccountCode,
                        client.Name, client.CompanyLogo);
            html.AppendFormat(@"</table>");
            html.AppendFormat(@"<table class='striped'><tr class='header-row'>
                                    <td style ='width:70%'><span class=bold'>Description</span><br />
                                   
                                    </td>
                                    <td style ='width:15%'><span class='bold'>Taxed</span><br />
                                   
                                    </td>
                                    <td style ='width:15%'><span class='bold'>Amount</span><br />
                                   
                                    </td>
                                    </tr>");

            long totalAmount = 0;

            foreach (var item in Orders)
            {
                var amount = item.OrderCharges.Select(x => x.Amount).Sum();

                totalAmount += amount;

                html.AppendFormat(@" <tr>
                                    <td><span class='bold'>Units </span>{0}
                                      
                                        <span class='bold'>Order Number </span>{2} </ br>
                                        <span class='bold'>Date Ordered </span>{1}
                                    </td>
                                    <td><span class='bold'>{3}</span>
                                   
                                    </td>
                                    <td><span class='bold'>{4}</span>
                                   
                                    </td>
                                    </tr>   ", item.Pianos.Count , item.CreatedAt , item.OrderNumber,
                                                "No amount","$ " + amount);
            }
            html.AppendFormat(@"</table>");

            html.AppendFormat(@"<table style='width: 30%; margin-left:70%;'><tr>
                                    <td><span class='bold'>Subtotal</span><br />
                                    </td>
                                    <td><span class='bold'>{0}</span><br />
                                    </td>
                                    </tr>
                                    <tr>
                                    <td><span class='bold'>Taxable</span><br />
                                    </td>
                                    <td><span class='bold'>{1}</span><br />
                                    </td>
                                    </tr>
                                    <tr>
                                    <td><span class='bold'>Tax Rate</span><br />
                                    </td>
                                    <td><span class='bold'>{2}</span><br />
                                    </td>
                                    </tr>
                                    <tr>
                                    <td><span class='bold'>Tax due</span><br />
                                    </td>
                                    <td><span class='bold'>{3}</span><br />
                                    </td>
                                    </tr>
                                    <tr>
                                    <td><span class='bold'>Other</span><br />
                                    </td>
                                    <td><span class='bold'>{4}</span><br />
                                    </td>
                                    </tr>
                                    <tr>
                                    <td style='border-top:double black;'><span class='bold'>Total</span><br />
                                    </td>
                                    <td style='border-top: double black'><span class='bold'>{5}</span><br />
                                    </td>
                                    </tr>
                                    <tr style='padding-top:10px;'>
                                    <td colspan='2'>Make all check payable to Encore Piano Limited<br />
                                    </td>
                                    </table>", totalAmount, 0, 0, 0, 0, totalAmount);

            html.AppendFormat(@"</table>");

            html.AppendFormat(@"<table style='width: 70%; margin-top:15px;' class='bordered striped'><tr class='header-row'>
                                    <td><span class='bold'>Other Notes</span><br />
                                    </tr>
                                    <tr>
                                    <td>1 - Total payment due in 30 days <br />
                                    </td></tr>
                                    <tr>
                                    <td>2 - Please include the invoice number on your check <br />
                                    </td></tr>");

            html.AppendFormat(@"</table>");

        

            html.AppendFormat(@"<table><tr>
                                    <td style='text-align:center;'><span class='bold'>Thank you for your business!</span><br />
                                    </td></tr>");

            html.AppendFormat(@"</table>");


            html.Append(@"</body></html>");

            return html.ToString();

        }
    
    }
}