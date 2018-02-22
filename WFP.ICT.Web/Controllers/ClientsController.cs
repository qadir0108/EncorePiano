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

namespace WFP.ICT.Web.Controllers
{
    public class ClientsController : BaseController
    {
        public ActionResult Index()
        {
            TempData["AddressStates"] = new SelectList(States, "Value", "Text");
            TempData["ClientTypes"] = new SelectList(ClientTypeList, "Value", "Text");
            return View();
        }

        public ActionResult Initialize([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            IEnumerable<Client> Clients = Db.Clients.Include(x => x.Address);

            var totalCount = Clients.Count();

            #region Filtering
            // Apply filters for searching
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                Clients = Clients.AsEnumerable().
                                          Where(p => p.Name.Contains(value) ||
                                         p.PhoneNumber.Contains(value) ||
                                         p.EmailAddress.Contains(value) ||
                                         p.Address.Address1.Contains(value) ||
                                         p.AccountCode.Contains(value) ||
                                         p.CreatedAt.ToString("yyyy-MM-dd").Contains(value) ||
                                         p.Comment.Contains(value) ||
                                         ((CustomerTypeEnum)(p.ClientType)).ToString().ToLower().Contains(value.ToLower())
                                         );
            }

            var filteredCount = Clients.Count();

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

                        Clients = column.SortDirection.ToString() == "Ascendant" ?
                                    Clients.OrderBy(x => x.ClientType) :
                                    Clients.OrderByDescending(x => x.ClientType);
                    }

                    if (column.Data == "Name")
                    {

                        Clients = column.SortDirection.ToString() == "Ascendant" ?
                                    Clients.OrderBy(x => x.Name) :
                                    Clients.OrderByDescending(x => x.Name);
                    }

                    if (column.Data == "Created")
                    {

                        Clients = column.SortDirection.ToString() == "Ascendant" ?
                                    Clients.OrderBy(x => x.CreatedAt) :
                                     Clients.OrderByDescending(x => x.CreatedAt);
                    }

                    if (column.Data == "Phone")
                    {

                        Clients = column.SortDirection.ToString() == "Ascendant" ?
                                    Clients.OrderBy(x => x.PhoneNumber) :
                                     Clients.OrderByDescending(x => x.PhoneNumber);
                    }

                    if (column.Data == "Account")
                    {

                        Clients = column.SortDirection.ToString() == "Ascendant" ?
                                    Clients.OrderBy(x => x.AccountCode) :
                                     Clients.OrderByDescending(x => x.AccountCode);
                    }

                    if (column.Data == "Comment")
                    {

                        Clients = column.SortDirection.ToString() == "Ascendant" ?
                                    Clients.OrderBy(x => x.Comment) :
                                     Clients.OrderByDescending(x => x.Comment);
                    }
                    if (column.Data == "Address")
                    {

                        Clients = column.SortDirection.ToString() == "Ascendant" ?
                                    Clients.OrderBy(x => x.Address.Address1) :
                                     Clients.OrderByDescending(x => x.Address.Address1);
                    }

                }
                orderByString = "Ordered";
            }

            if (orderByString == string.Empty)
            {
                Clients = Clients.OrderBy(x =>
                                         x.Name);
            }
            #endregion Sorting

            // Paging
            if (requestModel.Length != -1)
            {
                Clients = Clients.Skip(requestModel.Start).Take(requestModel.Length);
            }

            var result = Clients.
                         ToList()
                        .Select(client => new
                        {
                            Type = ((CustomerTypeEnum)(client.ClientType)).ToString(),
                            Name = client.Name,
                            Created = client.CreatedAt.ToString("yyyy-MM-dd"),
                            Phone = client.PhoneNumber,
                            Account = client.AccountCode ,
                            Comment = client.Comment,
                            Address = client.Address?.Address1 + "," + client.Address?.City + "," + client.Address?.State + ".",
                            Actions = GetActions(client.Id),
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