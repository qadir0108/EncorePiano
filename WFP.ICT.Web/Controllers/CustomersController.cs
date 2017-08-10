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
    public class CustomersController : BaseController
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
            TempData["AddressStates"] = new SelectList(States, "Value", "Text");
            TempData["ClientTypes"] = new SelectList(ClientTypeList, "Value", "Text");
            return View();
        }

        public ActionResult InitializeClients([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            IEnumerable<Client> Clients = db.Clients.Include(x => x.Addresses);

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
                                         p.Addresses.Address1.Contains(value) ||
                                         p.AccountCode.Contains(value) ||
                                         p.CreatedAt.ToString("yyyy-MM-dd").Contains(value) ||
                                         p.Comment.Contains(value) ||
                                         ((CustomerTypeEnum)(p.CustomerType)).ToString().ToLower().Contains(value.ToLower())
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
                                    Clients.OrderBy(x => x.CustomerType) :
                                    Clients.OrderByDescending(x => x.CustomerType);
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
                                    Clients.OrderBy(x => x.Addresses.Address1) :
                                     Clients.OrderByDescending(x => x.Addresses.Address1);
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
                            Type = ((CustomerTypeEnum)(client.CustomerType)).ToString(),
                            Name = client.Name,
                            Created = client.CreatedAt.ToString("yyyy-MM-dd"),
                            Phone = client.PhoneNumber,
                            Account = client.AccountCode ,
                            Comment = client.Comment,
                            Address = client.Addresses.Address1 + "," + client.Addresses.City + "," + client.Addresses.State + ".",
                            Actions = GetActions(client.Id),
                        });

            return Json(new DataTablesResponse
            (requestModel.Draw, result, filteredCount, totalCount),
                        JsonRequestBehavior.AllowGet);
        }

        public string GetActions(Guid? id)
        {
            return "";
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
                client.CustomerType = NewAddressVm.Client.Type;
                client.PhoneNumber = NewAddressVm.Client.PhoneNumber;
                client.EmailAddress = NewAddressVm.Client.Email;
                client.CreatedAt = System.DateTime.Now;
                client.CreatedBy = LoggedInUser?.UserName;

                db.Clients.Add(client);
                db.SaveChanges();

                address.Id = addressId;
                address.CustomerId = clientId;
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

                db.Addresses.Add(address);
                db.SaveChanges();

                return Json(new JsonResponse() { IsSucess = true }, JsonRequestBehavior.AllowGet);

            }

            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        



    }
}