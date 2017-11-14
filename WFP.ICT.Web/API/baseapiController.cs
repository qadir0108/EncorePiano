using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using WFP.ICT.Data.Entities;
using WFP.ICT.Enum;
using WFP.ICT.Web.Helpers;
using WFP.ICT.Web.Models;

namespace WFP.ICT.Web.API
{
    public class BaseApiController : ApiController
    {
        protected WFPICTContext db = new WFPICTContext();

        readonly string _imagesPath = $"~/Images/";
        protected string ImagesPath
        {
            get
            {
                return HttpContext.Current.Server.MapPath(_imagesPath);
            }
        }

        readonly string _signPath = $"~/Images/Sign/";
        protected string SignPath
        {
            get
            {
                return HttpContext.Current.Server.MapPath(_signPath);
            }
        }

        readonly string _pianoImagesPath = $"~/Images/Piano/";
        protected string PianoImagesPath
        {
            get
            {
                return HttpContext.Current.Server.MapPath(_pianoImagesPath);
            }
        }

        public string UploadsFormsPath
        {
            get
            {
                return HttpContext.Current.Server.MapPath("~/Uploads/Forms/");
            }
        }

        public string DownloadsFormsPath
        {
            get
            {
                return HttpContext.Current.Server.MapPath("~/Downloads/Forms/");
            }
        }

        protected DriverLogin IsTokenValid(RequestTypeEnum requestType)
        {
            var requestParams = Request.GetQueryNameValuePairs().ToList();
            if (requestParams.Count(x => x.Key.Equals("AuthToken")) == 0)
                throw new Exception("Authentication token is required.");

            string authToken = requestParams.FirstOrDefault(x => x.Key.Equals("AuthToken")).Value;
            var driverLogin = db.DriverLogins.FirstOrDefault(x => x.Token == authToken);
            if (driverLogin == null)
                throw new Exception("Authentication token is invalid.");
            
            db.APIRequestLogs.Add(new APIRequestLog()
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                RequestType = (int)requestType,
            });
            db.SaveChanges();

            return driverLogin;
        }
    }
}
