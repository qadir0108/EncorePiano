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
        public ActionResult Add()
        {
            TempData["AddressStates"] = new SelectList(States, "Value", "Text");
            TempData["ClietTypes"] = new SelectList(ClientTypeList, "Value", "Text");
            
            return View(new NewAddressVm());
        }

        [HttpPost]
        public ActionResult Save(NewAddressVm address)
        {
            try
            {

                return Json(new JsonResponse() { IsSucess = true }, JsonRequestBehavior.AllowGet);

            }

            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        



    }
}