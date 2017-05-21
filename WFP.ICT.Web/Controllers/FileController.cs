using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WFP.ICT.Web.Models;

namespace WFP.ICT.Web.Controllers
{
    public class FileController : BaseController
    {

        [HttpPost]
        public async Task<JsonResult> UploadFile()
        {
            try
            {
                string amazonFileKey = "";
                foreach (string selectedFile in Request.Files)
                {
                    var fileContent = Request.Files[selectedFile];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        var stream = fileContent.InputStream;
                        string filePath = Path.Combine(UploadPath, fileContent.FileName);
                        using (var fileStream = System.IO.File.Create(filePath))
                        {
                            stream.CopyTo(fileStream);
                        }

                        amazonFileKey = string.Format("{0:yyyyMMdd_HHmmss}_{1}", DateTime.Now, fileContent.FileName);

                        // Delete local
                        //string fullPath = Path.Combine(UploadPath, fileContent.FileName);
                        //if (System.IO.File.Exists(fullPath))
                        //    System.IO.File.Delete(fullPath);
                    }
                }

                return Json(new JsonResponse() { IsSucess = true, Result = amazonFileKey });
            }
            catch (Exception ex)
            {
                //Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = "Upload failed " + ex.Message });
            }
        }

        [HttpGet]
        public virtual ActionResult DownloadFile(string file)
        {
            string filePath = Path.Combine(UploadPath, Server.UrlEncode(file));
            //if (!System.IO.File.Exists(filePath))
            // return null;

            return File(filePath, "text/csv", file);
        }

        [HttpPost]
        public virtual JsonResult DeleteFile(string file)
        {
            try
            {
                //string fullPath = Path.Combine(UploadPath, Server.UrlEncode(file));
                //if (!System.IO.File.Exists(fullPath))
                //    throw new Exception("File does not exists.");
                //System.IO.File.Delete(fullPath);

                return Json(new JsonResponse() {IsSucess = true});
            }
            catch (Exception ex)
            {
                //Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return Json(new JsonResponse() {IsSucess = false, ErrorMessage = "File Delete failed. " + ex.Message });
            }
        }
    }
}