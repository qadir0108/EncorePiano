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
            if (WareHouse == string.Empty && Client == string.Empty)
            {
                return Json(new DataTablesResponse
                         (requestModel.Draw, Enumerable.Empty<Piano>(), 0, 0),
                         JsonRequestBehavior.AllowGet);
            }

            IQueryable<Piano> Pianos = db.Pianos;
            if (WareHouse != string.Empty)
            {
                Guid id = Guid.Parse(WareHouse);
                Pianos = Pianos.
                         Where(x => x.WarehouseId == id);
            }
            if (Client != string.Empty)
            {
                Guid id = Guid.Parse(Client);
                Pianos = Pianos.
                        Where(x => x.ClientId == id);
            }
            Pianos = Pianos.
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
                                          Where(p => p.PianoType.Type.Contains(value) ||
                                                p.PianoMake.Name.Contains(value) ||
                                                p.SerialNumber.Contains(value) ||
                                                p.Model.Contains(value) ||
                                                p.Client.Name.Contains(value) ||
                                                PianoSizeConversion.GetFeetInches(p.PianoSize.Width).Contains(value)
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
                    if (column.Data == "Size")
                    {

                        Pianos = column.SortDirection.ToString() == "Ascendant" ?
                                    Pianos.OrderBy(x => x.PianoSize.Width) :
                                    Pianos.OrderByDescending(x => x.PianoSize.Width);
                    }
                    if (column.Data == "Make")
                    {

                        Pianos = column.SortDirection.ToString() == "Ascendant" ?
                                    Pianos.OrderBy(x => x.PianoMake.Name) :
                                    Pianos.OrderByDescending(x => x.PianoMake.Name);
                    }
                    if (column.Data == "Model")
                    {

                        Pianos = column.SortDirection.ToString() == "Ascendant" ?
                                    Pianos.OrderBy(x => x.Model) :
                                    Pianos.OrderByDescending(x => x.Model);
                    }
                    if (column.Data == "IsBench")
                    {

                        Pianos = column.SortDirection.ToString() == "Ascendant" ?
                                    Pianos.OrderBy(x => x.IsBench) :
                                    Pianos.OrderByDescending(x => x.IsBench);
                    }
                    if (column.Data == "IsPlayer")
                    {

                        Pianos = column.SortDirection.ToString() == "Ascendant" ?
                                    Pianos.OrderBy(x => x.IsPlayer) :
                                    Pianos.OrderByDescending(x => x.IsPlayer);
                    }
                    if (column.Data == "IsBoxed")
                    {

                        Pianos = column.SortDirection.ToString() == "Ascendant" ?
                                    Pianos.OrderBy(x => x.IsBoxed) :
                                    Pianos.OrderByDescending(x => x.IsBoxed);
                    }
                    if (column.Data == "Name")
                    {

                        Pianos = column.SortDirection.ToString() == "Ascendant" ?
                                    Pianos.OrderBy(x => x.Client.Name) :
                                    Pianos.OrderByDescending(x => x.Client.Name);
                    }
                    if (column.Data == "SerialNumber")
                    {

                        Pianos = column.SortDirection.ToString() == "Ascendant" ?
                                    Pianos.OrderBy(x => x.SerialNumber) :
                                    Pianos.OrderByDescending(x => x.SerialNumber);
                    }


                }
                orderByString = "Ordered";
            }

            if (orderByString == string.Empty)
            {
                Pianos = Pianos.OrderBy((x => x.PianoType.Type));
            }


            #endregion Sorting

            // Paging
            if(requestModel.Length != -1)
            {
                Pianos = Pianos.Skip(requestModel.Start).Take(requestModel.Length);
            }
    

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

        public ActionResult Batch()
        {

            TempData["Warehouses"] = new SelectList(WarehousesList, "Value", "Text");

            return View();
        }

        public ActionResult PrintLabels(List<BatchLoadVm> data)
        {

            return View();
        }
        public ActionResult InitializeBatchLoadGrid([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel,
                                         string FileName )
        {
            if (FileName == string.Empty)
            {
                return Json(new DataTablesResponse
                         (requestModel.Draw, Enumerable.Empty<Piano>(), 0, 0),
                         JsonRequestBehavior.AllowGet);
            }

            IEnumerable<BatchLoadVm> BatchGrid = ReadExcelSheet(FileName);

            var totalCount = BatchGrid.Count();

            #region Filtering
            // Apply filters for searching
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                BatchGrid = BatchGrid.Where(p =>    p.Bench.Contains(value) ||
                                                    p.Box.Contains(value) ||
                                                    p.Player.Contains(value) ||
                                                    p.Model.Contains(value) ||
                                                    p.Owner.Contains(value) ||
                                                    p.Serial.Contains(value) ||
                                                    p.Size.Contains(value) ||
                                                    p.Type.Contains(value) ||
                                                    p.Make.Contains(value));
            }

            var filteredCount = BatchGrid.Count();

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

                        BatchGrid = column.SortDirection.ToString() == "Ascendant" ?
                                    BatchGrid.OrderBy(x => x.Type) :
                                    BatchGrid.OrderByDescending(x => x.Type);
                    }
                    if (column.Data == "Size")
                    {

                        BatchGrid = column.SortDirection.ToString() == "Ascendant" ?
                                    BatchGrid.OrderBy(x => x.Size) :
                                    BatchGrid.OrderByDescending(x => x.Size);
                    }


                }
                orderByString = "Ordered";
            }

            if (orderByString == string.Empty)
            {
                BatchGrid = BatchGrid.OrderBy((x => x.Type));
            }

            #endregion Sorting

            // Paging
            if(requestModel.Length !=  1)
            {
                BatchGrid = BatchGrid.Skip(requestModel.Start).Take(requestModel.Length);
            }
          

            var result = BatchGrid
                        .Select(x => new
                        {

                            Type = x.Type,
                            Size = x.Size,
                            Make = x.Make,
                            Model = x.Model,
                            Serial = x.Serial,
                            Bench = x.Bench,
                            Player = x.Player,
                            Box = x.Box,
                            Owner = x.Owner,
                            Action = "None"
                        });

            return Json(new DataTablesResponse
                        (requestModel.Draw, result, filteredCount, totalCount),
                        JsonRequestBehavior.AllowGet);
        }


     
        private List<BatchLoadVm> ReadExcelSheet(string fname)
        {
            List<BatchLoadVm> BatchGridData = new List<BatchLoadVm>();

            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(Path.Combine(UploadPath,fname), false))
            {
                Sheet sheet = doc.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
                Worksheet worksheet = (doc.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;
                IEnumerable<Row> rows = worksheet.GetFirstChild<SheetData>().Descendants<Row>();

                foreach (Row row in rows)
                {  
                    if (row.RowIndex.Value == 1)
                    {

                    }
                    else
                    {
                        BatchLoadVm obj = new BatchLoadVm();

                        var j = 0;
                        foreach (Cell cell in row.Descendants<Cell>())
                        {
                         
                            if (j == 0)
                            {
                                obj.Type =  GetCellValue(doc, cell);
                            }
                            else if (j == 1)
                            {
                                obj.Size = GetCellValue(doc, cell);
                            }
                            else if (j == 2)
                            {
                                obj.Make = GetCellValue(doc, cell);
                            }
                            else if (j == 3)
                            {
                                obj.Model = GetCellValue(doc, cell);
                            }
                            else if (j == 4)
                            {
                                obj.Serial = GetCellValue(doc, cell);
                            }
                            else if (j == 5)
                            {
                                obj.Bench = GetCellValue(doc, cell);
                            }
                            else if (j == 6)
                            {
                                obj.Player = GetCellValue(doc, cell);
                            }
                            else if (j == 7)
                            {
                                obj.Box = GetCellValue(doc, cell);
                            }
                            else if (j == 8)
                            {
                                obj.Owner = GetCellValue(doc, cell);
                            }
                            j++;
                        }
                        BatchGridData.Add(obj);
                    }
                  
                }

            }
            return BatchGridData;
        }
        private string GetCellValue(SpreadsheetDocument doc, Cell cell)
        {
            string value = cell.CellValue.InnerText;
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
            }
            return value;
        }

    }

}