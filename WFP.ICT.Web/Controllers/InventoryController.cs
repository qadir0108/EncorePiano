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

namespace WFP.ICT.Web.Controllers
{
    public class InventoryController : BaseController
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
        // GET: Inventory
        public ActionResult Index()
        {
            TempData["Customers"] = new SelectList(CustomersList, "Value", "Text");

            TempData["Warehouses"] = new SelectList(WarehousesList, "Value", "Text");

            return View();
        }

        public ActionResult InitializePianoInventory([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel,
                                             string WareHouse, string Client)
        {
            if(WareHouse == string.Empty && Client == string.Empty)
            {
                return Json(new DataTablesResponse
                         (requestModel.Draw, Enumerable.Empty<Piano>(), 0, 0),
                         JsonRequestBehavior.AllowGet);
            }

            IQueryable<Piano> Pianos =  db.Pianos;
            if(WareHouse != string.Empty)
            {
                Guid id = Guid.Parse(WareHouse);
                Pianos = Pianos.
                         Where(x => x.WarehouseId == id);
            }
            if(Client != string.Empty)
            {
                Guid id = Guid.Parse(Client);
                Pianos = Pianos.
                        Where(x => x.ClientId == id );
            }
            Pianos =    Pianos.
                        Include(x => x.PianoMake).
                        Include(x => x.PianoSize).
                        Include(x => x.PianoType).
                        Include(x => x.Client);

            var totalCount = Pianos.Count();

            #region Filtering
            // Apply filters for searching
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                Pianos = Pianos.AsEnumerable().
                                          Where(p => p.PianoType.Type.Contains(value)
                                         ).AsQueryable();
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
                                    Pianos.OrderBy(x => x.PianoType.Type) :
                                    Pianos.OrderByDescending(x => x.PianoType.Type);
                    }

                    orderByString = "Ordered";
                }

            }
                Pianos = Pianos.OrderBy(x => orderByString == string.Empty ?
                                       x.PianoType.Type :
                                        orderByString);

                #endregion Sorting

                // Paging
                Pianos = Pianos.Skip(requestModel.Start).Take(requestModel.Length);

                var result = Pianos.
                             ToList()
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
                                Name = x.Client == null ? "Not Data" : x.Client.Name
                            });

                return Json(new DataTablesResponse
                            (requestModel.Draw, result, filteredCount, totalCount),
                            JsonRequestBehavior.AllowGet);
            }

        }
    
}