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
            return View(new InvoiceViewModel());
        }
    }
}