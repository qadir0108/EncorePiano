using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WFP.ICT.Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            //string enc = WFP.ICT.Web.Security.AppSecurity.Encrypt("Kamran Qadir");
            //string dec = WFP.ICT.Web.Security.AppSecurity.Decrypt("/H56mheuKXagOAIIe0ho2A==");
            return View();
        }
    }
}