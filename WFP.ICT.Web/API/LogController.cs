using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using Microsoft.Ajax.Utilities;
using Nelibur.ObjectMapper;
using Newtonsoft.Json;
using WFP.ICT.Data.Entities;
using WFP.ICT.Enum;
using WFP.ICT.Enums;
using WFP.ICT.Web.API;
using WFP.ICT.Web.API.Models;
using WFP.ICT.Web.Helpers;
using WFP.ICT.Web.Models;
using System.Globalization;
using System.Web;

namespace WFP.ICT.Web.Controllers
{
    public class logController : BaseApiController
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
