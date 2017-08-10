using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WFP.ICT.Enum;
using WFP.ICT.Web.API.Models;

namespace WFP.ICT.Web.API
{
    public class filesController : baseapiController
    {
        [AllowAnonymous]
        //[Route("api/files/sign")]
        public async Task<HttpResponseMessage> sign()
        {
            //IsTokenValid(RequestTypeEnum.UploadSign);
            return await Upload(UploadFileType.Signature);
        }

        [AllowAnonymous]
        //[Route("api/files/piano")]
        public async Task<HttpResponseMessage> piano()
        {
            IsTokenValid(RequestTypeEnum.UploadPianoImage);
            return await Upload(UploadFileType.PodImage);
        }

        private async Task<HttpResponseMessage> Upload(UploadFileType fileType)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                var httpRequest = HttpContext.Current.Request;

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                    var postedFile = httpRequest.Files[file];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {

                        int MaxContentLength = 1024 * 1024 * 1; //Size = 1 MB  

                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {

                            var message = string.Format("Please Upload {0} image of type .jpg,.gif,.png.", fileType);

                            dict.Add("error", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else if (postedFile.ContentLength > MaxContentLength)
                        {

                            var message = string.Format("Please Upload a file upto 1 mb.");

                            dict.Add("error", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else
                        {
                            var directoryPath = fileType == UploadFileType.Signature
                                ? "~/Images/Sign/"
                                : "~/Images/Piano/";
                            var filePath = HttpContext.Current.Server.MapPath( directoryPath + postedFile.FileName);
                            postedFile.SaveAs(filePath);
                        }
                    }

                    var message1 = string.Format("Image uploaded successfully.");
                    return Request.CreateErrorResponse(HttpStatusCode.Created, message1); ;
                }
                var res = string.Format("Please Upload a image.");
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
            catch (Exception ex)
            {
                var res = string.Format("There is error while uploading." + ex.Message);
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
        }
    }
}