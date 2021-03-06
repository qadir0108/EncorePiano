﻿using System;
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
                string FileName = "";
                foreach (string selectedFile in Request.Files)
                {
                    var fileContent = Request.Files[selectedFile];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        FileName = string.Format("{0:yyyyMMdd_HHmmss}_{1}", DateTime.Now, fileContent.FileName);
                        var stream = fileContent.InputStream;
                        string filePath = Path.Combine(UploadsPath, FileName);
                        using (var fileStream = System.IO.File.Create(filePath))
                        {
                            stream.CopyTo(fileStream);
                        }

                      

                    }
                }

                return Json(new JsonResponse() { IsSucess = true, Result = FileName });
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
            string filePath = Path.Combine(UploadsPath, Server.UrlEncode(file));
            if (!System.IO.File.Exists(filePath))
                return null;

            return File(filePath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file);
        }

        [HttpPost]
        public virtual JsonResult DeleteFile(string file)
        {
            try
            {
                string fullPath = Path.Combine(UploadsPath, Server.UrlEncode(file));
                if (!System.IO.File.Exists(fullPath))
                    throw new Exception("File does not exists.");
                System.IO.File.Delete(fullPath);

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