using System;
using System.Linq;
using System.Linq.Dynamic;
using System.Data.Entity;
using System.Web.Mvc;
using WFP.ICT.Data.Entities;
using WFP.ICT.Enum;
using DataTables.Mvc;
using WFP.ICT.Web.Models;
using System.Data;
using System.Collections.Generic;
using System.Text;
using WFP.ICT.Web.Helpers;

namespace WFP.ICT.Web.Controllers
{
    [Authorize]
    [AjaxAuthorize]
    public class InvoicesController : BaseController
    {
        public ActionResult Index()
        {
            TempData["PaymentTypes"] = new SelectList(PaymentTypeList, "Value", "Text");
            TempData["OrderNumbers"] = new SelectList(OrderNumbersList, "Value", "Text");
            TempData["InvoiceNumbers"] = new SelectList(InvoiceNumbersList, "Value", "Text");
            return View();
        }

        public ActionResult InitializePayments([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            IEnumerable<ClientPayment> payments = Db.Payments.Include(x => x.Client);

            var totalCount = payments.Count();

            #region Filtering
            // Apply filters for searching
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                payments = payments.AsEnumerable().
                                          Where(p => p.Amount.ToString().Contains(value) ||
                                         p.TransactionNumber.Contains(value) ||
                                         p.PaymentDate.ToString("yyyy-MM-dd").Contains(value) ||
                                         p.CreatedAt.ToString("yyyy-MM-dd").Contains(value)
                                         );
            }

            var filteredCount = payments.Count();

            #endregion Filtering

            #region Sorting
            // Sorting
            var sortedColumns = requestModel.Columns.GetSortedColumns();
            var orderByString = String.Empty;

            //if (sortedColumns.Count() > 0)
            //{
            //    foreach (var column in sortedColumns)
            //    {
            //        if (column.Data == "Type")
            //        {

            //            payments = column.SortDirection.ToString() == "Ascendant" ?
            //                        payments.OrderBy(x => x.CustomerType) :
            //                        payments.OrderByDescending(x => x.CustomerType);
            //        }

            //        if (column.Data == "Name")
            //        {

            //            payments = column.SortDirection.ToString() == "Ascendant" ?
            //                        payments.OrderBy(x => x.Name) :
            //                        payments.OrderByDescending(x => x.Name);
            //        }

            //        if (column.Data == "Created")
            //        {

            //            payments = column.SortDirection.ToString() == "Ascendant" ?
            //                        payments.OrderBy(x => x.CreatedAt) :
            //                         payments.OrderByDescending(x => x.CreatedAt);
            //        }

            //        if (column.Data == "Phone")
            //        {

            //            payments = column.SortDirection.ToString() == "Ascendant" ?
            //                        payments.OrderBy(x => x.PhoneNumber) :
            //                         payments.OrderByDescending(x => x.PhoneNumber);
            //        }

            //        if (column.Data == "Account")
            //        {

            //            payments = column.SortDirection.ToString() == "Ascendant" ?
            //                        payments.OrderBy(x => x.AccountCode) :
            //                         payments.OrderByDescending(x => x.AccountCode);
            //        }

            //        if (column.Data == "Comment")
            //        {

            //            payments = column.SortDirection.ToString() == "Ascendant" ?
            //                        payments.OrderBy(x => x.Comment) :
            //                         payments.OrderByDescending(x => x.Comment);
            //        }
            //        if (column.Data == "Address")
            //        {

            //            payments = column.SortDirection.ToString() == "Ascendant" ?
            //                        payments.OrderBy(x => x.Addresses.Address1) :
            //                         payments.OrderByDescending(x => x.Addresses.Address1);
            //        }

            //    }
            //    orderByString = "Ordered";
            //}

            if (orderByString == string.Empty)
            {
                payments = payments.OrderByDescending(x =>
                                         x.PaymentDate);
            }
            #endregion Sorting

            // Paging
            if (requestModel.Length != -1)
            {
                payments = payments.Skip(requestModel.Start).Take(requestModel.Length);
            }

            var result = payments.
                         ToList()
                        .Select(payment => new
                        {
                            PaymentType = ((PaymentTypeEnum)(payment.PaymentType)).ToString(),
                            PaymentDate = payment.PaymentDate.ToString("yyyy-MM-dd hh:mm:ss tt"),
                            TransactionNumber = payment.TransactionNumber,
                            ClientAccountCode = payment.Client?.AccountCode ,
                            ClientName = payment.Client?.Name,
                            Actions = GetActions(payment.Id),
                        });

            return Json(new DataTablesResponse
            (requestModel.Draw, result, filteredCount, totalCount),
                        JsonRequestBehavior.AllowGet);
        }

        public string GetActions(Guid? id)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"<a href='#' class='btnEdit' data-tooltip='tooltip' data-id='{0}' title='Edit Payment'>
                             <span class='glyphicon glyphicon-pencil'>
                             </span>
                          </a>
                           <a class='btnDelete' data-id={0} href='#' data-tooltip='tooltip' title='Delete Payment'>
                            <span class='glyphicon glyphicon-trash'></span>
                            </a>", id);
            return sb.ToString();
        }

        [HttpPost]
        public ActionResult Save(NewAddressVm NewAddressVm)
        {
            try
            {
                Client client = new Client();
                Address address = new Address();
                Guid addressId = Guid.NewGuid();
                Guid clientId = Guid.NewGuid();

                client.Id = clientId;
                
                client.Name = NewAddressVm.Client.Name;
                client.AccountCode = NewAddressVm.Client.AccountCode;
                client.Comment = NewAddressVm.Client.Notes;
                client.ClientType = NewAddressVm.Client.Type;
                client.PhoneNumber = NewAddressVm.Client.PhoneNumber;
                client.EmailAddress = NewAddressVm.Client.Email;
                client.CreatedAt = DateTime.Now;
                client.CreatedBy = LoggedInUser?.UserName;

                Db.Clients.Add(client);
                Db.SaveChanges();

                address.Id = addressId;
                address.ClientId = clientId;
                address.PhoneNumber = NewAddressVm.PhoneNumber;
                address.Address1 = NewAddressVm.Address;
                address.State = NewAddressVm.State;
                address.City = NewAddressVm.City;
                address.PostCode = NewAddressVm.PostCode;
                address.Name = NewAddressVm.Name;
                address.CreatedAt = System.DateTime.Now;
                address.CreatedBy = LoggedInUser?.UserName;
                address.Lat = NewAddressVm.lat;
                address.Lng = NewAddressVm.lng;

                Db.Addresses.Add(address);
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
                if (Db.Orders.Count(x => x.ClientId == id) > 0 ||
                    Db.Payments.Count(x => x.ClientId == id) > 0 ||
                    Db.Invoices.Count(x => x.ClientId == id) > 0 )
                {
                    return Json(new JsonResponse() { IsSucess = false, ErrorMessage = "Unable to process as there are orders, payments, invoices against this client" }, JsonRequestBehavior.AllowGet);
                }

                var client = Db.Clients.Include(x => x.Address).FirstOrDefault(x => x.Id == id);
                Db.Addresses.Remove(client.Address);
                Db.SaveChanges();

                Db.Clients.Remove(client);
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
                var client = Db.Clients.Include(x => x.Address ).FirstOrDefault(x => x.Id == id);
                TempData["AddressStates"] = new SelectList(States, "Value", "Text");
                var model = new NewClientVm()
                {
                    Name = client.Name,
                    AccountCode = client.AccountCode,
                    Notes = client.Comment,
                    Id = client.Id,
                    Type = client.ClientType,
                    PhoneNumber = client.PhoneNumber,
                    Email = client.EmailAddress,
                };
                
                //client.Id = addressId;
                //client.CustomerId = clientId;
                //client.PhoneNumber = NewAddressVm.PhoneNumber;
                //client.Address1 = NewAddressVm.Address;
                //client.State = NewAddressVm.State;
                //client.City = NewAddressVm.City;
                //client.PostCode = NewAddressVm.PostCode;
                //client.Name = NewAddressVm.Name;
                //client.CreatedAt = System.DateTime.Now;
                //client.CreatedBy = LoggedInUser?.UserName;
                //client.Lat = NewAddressVm.lat;
                //client.Lng = NewAddressVm.lng;

                return PartialView("~/Views/Clients/Add.cshtml", model);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}