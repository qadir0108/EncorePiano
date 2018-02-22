using System;
using System.Web.Http;
using System.Web.Http.Results;
using WFP.ICT.Web.API;
using WFP.ICT.Web.Models;
using System.Web;

namespace WFP.ICT.Web.Controllers
{
    public class LogApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/log")]
        public JsonResult<JsonResponse> SendLog([FromBody] LogModel model)
        {
            try
            {
                string fileName = $"{DateTime.Now.ToString(StringConstants.TimeStampFormatApp)}.txt";
                var directoryPath = "~/APILogs/";
                var filePath = HttpContext.Current.Server.MapPath(directoryPath + fileName);
                System.IO.File.WriteAllText(filePath, model.log);
                return Json(new JsonResponse() { IsSucess = true});
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message });
            }
        }
    }
}
