using System.Web;
using System.Web.Mvc;

namespace WFP.ICT.Web.Controllers
{
    public class ErrorController : BaseController
    {
        //
        // GET: /Error/
        public ActionResult Index()
        {
            return RedirectToAction("GenericError", new HandleErrorInfo(new HttpException(403, "Dont allow access the pages"), "ErrorController", "Index"));
        }

        public ViewResult GenericError(HandleErrorInfo exception)
        {
            return View("Error", exception);
        }

        public ViewResult NotFound(HandleErrorInfo exception)
        {
            ViewBag.Title = "404. Not Found";
            return View("Error", new HandleErrorInfo(new HttpException(404, "404. Not Found"), "ErrorController", "Index"));
        }

        public ViewResult NotAuthorized()
        {
            ViewBag.Title = "You are not authorized";
            return View("Error");
        }
    }
}