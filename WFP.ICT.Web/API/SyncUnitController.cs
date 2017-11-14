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
using WFP.ICT.Web.Async;
using Hangfire;

namespace WFP.ICT.Web.Controllers
{
    public class SyncUnitController : BaseApiController
    {
        [HttpPost]
        [Route("api/sync/unit/load")]
        public JsonResult<JsonResponse> SyncLoad([FromBody] SyncLoadModel model)
        {
            try
            {
                IsTokenValid(RequestTypeEnum.SyncLoad);

                var assignment = db.PianoAssignments
                    .Include(x => x.PianoOrder)
                    .Include(x => x.PianoOrder.Pianos)
                    .FirstOrDefault(x => x.Id.ToString() == model.assignmentId);

                if (assignment == null)
                    throw new Exception("Assignment Not Found");

                var order = db.PianoOrders
                    .Include(x => x.Pianos)
                    .FirstOrDefault(x => x.Id == assignment.PianoOrderId);

                if (order == null)
                    throw new Exception("Assignment Order Not Found");

                var piano = order.Pianos.FirstOrDefault(x => x.Id.ToString() == model.Id);

                if (piano == null)
                    throw new Exception("Unit Not Found");
               
                piano = db.Pianos.Include(x => x.Statuses).FirstOrDefault(x => x.Id.ToString() == model.Id);

                // Add statues
                if(!piano.Statuses.Any(x => x.Status == (int)PianoStatusEnum.Picked))
                {
                    db.PianoStatuses.Add(new PianoStatus()
                    {
                        Id = Guid.NewGuid(),
                        PianoId = piano.Id,
                        CreatedAt = DateTime.Now,
                        Status = (int)PianoStatusEnum.Picked,
                        Comments = "Picked",
                        StatusTime = DateTime.Now,
                        StatusBy = StringConstants.System,
                    });
                }

                piano.IsMainUnitLoaded = model.isMainUnitLoaded == 1;
                piano.AdditionalBenchesStatus = model.additionalBenchesStatus + 1; // Unknown
                piano.AdditionalCasterCupsStatus = model.additionalCasterCupsStatus + 1;
                piano.AdditionalCoverStatus = model.additionalCoverStatus + 1;
                piano.AdditionalLampStatus = model.additionalLampStatus + 1;
                piano.AdditionalOwnersManualStatus = model.additionalOwnersManualStatus + 1;
                piano.LoadTimeStamp = DateTime.ParseExact(model.loadingTimeStamp, StringConstants.TimeStampFormatApp, CultureInfo.InvariantCulture);
                db.SaveChanges();

                return Json(new JsonResponse() { IsSucess = true, IsTokenValid = true});
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message });
            }
        }

        [HttpPost]
        [Route("api/sync/unit/deliver")]
        public JsonResult<JsonResponse> SyncDeliver([FromBody] SyncDeliverModel model)
        {
            try
            {
                IsTokenValid(RequestTypeEnum.SyncDeliver);

                var assignment = db.PianoAssignments
                    .Include(x => x.PODs)
                    .Include(x => x.PianoOrder)
                    .Include(x => x.PianoOrder.Pianos)
                    .FirstOrDefault(x => x.Id.ToString() == model.assignmentId);

                if (assignment == null)
                    throw new Exception("Assignment Not Found");

                var order = db.PianoOrders
                    .Include(x => x.Pianos)
                    .FirstOrDefault(x => x.Id == assignment.PianoOrderId);

                if (order == null)
                    throw new Exception("Assignment Order Not Found");

                var piano = order.Pianos.FirstOrDefault(x => x.Id.ToString() == model.Id);

                if (piano == null)
                    throw new Exception("Unit Not Found");

                string receiverSignature, receiverSignatureFilePath;
                try
                {
                    receiverSignature = $"{piano.Id.ToString()}.png";
                    receiverSignatureFilePath = $"{SignPath}{receiverSignature}";
                    ImageUtility.Base64ToImage(model.receiverSignature).Save(receiverSignatureFilePath);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error saving sign " + ex.Message);
                }                

                piano = db.Pianos.Include(x => x.Statuses).FirstOrDefault(x => x.Id.ToString() == model.Id);
                var pianoStatus = (PianoStatusEnum)System.Enum.Parse(typeof(PianoStatusEnum), model.pianoStatus);
                
                // Add statues
                if (!piano.Statuses.Any(x => x.Status == (int)pianoStatus))
                {
                    db.PianoStatuses.Add(new PianoStatus()
                    {
                        Id = Guid.NewGuid(),
                        PianoId = piano.Id,
                        CreatedAt = DateTime.Now,
                        Status = (int)pianoStatus,
                        Comments = "Delivered",
                        StatusTime = DateTime.Now,
                        StatusBy = StringConstants.System,
                    });
                }
                
                // Just save once
                var pod = assignment.PODs.FirstOrDefault(x => x.PianoId == piano.Id);
                Guid podId = Guid.NewGuid();
                if (pod == null)
                {
                    db.PianoPODs.Add(new PianoPOD()
                    {
                        Id = podId,
                        CreatedAt = DateTime.Now,

                        ReceivedBy = model.receiverName,
                        Signature = receiverSignature,
                        ReceivingTime = DateTime.ParseExact(model.deliveredAt, StringConstants.TimeStampFormatApp, CultureInfo.InvariantCulture),
                        Notes = StringConstants.System,

                        BenchesUnloadStatus = model.benchesUnloaded == 1,
                        CasterCupsUnloadStatus = model.casterCupsUnloaded == 1,
                        CoverUnloadStatus = model.coverUnloaded == 1,
                        LampUnloadStatus = model.lampUnloaded == 1,
                        OwnersManualUnloadStatus = model.ownersManualUnloaded == 1,

                        PianoAssignmentId = assignment.Id,
                        PianoId = piano.Id,
                    });
                }
                db.SaveChanges();

                if(!string.IsNullOrEmpty(order.DeliveryForm))
                    BackgroundJob.Enqueue(() => PODFormsGenerater.GeneratePODForm(db, UploadsFormsPath, DownloadsFormsPath, SignPath, podId) ); 

                return Json(new JsonResponse() { IsSucess = true, IsTokenValid = true });
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message });
            }
        }

        [HttpPost]
        [Route("api/sync/unit/image")]
        public JsonResult<JsonResponse> SyncImage([FromBody] SyncImageModel model)
        {
            try
            {
                IsTokenValid(RequestTypeEnum.SyncImage);

                var piano = db.Pianos
                    .Include(x => x.Pictures)
                    .Include(x => x.Order)
                    .Include(x => x.Order.PianoAssignment)
                    .Include(x => x.Order.PianoAssignment.PODs)
                    .FirstOrDefault(x => x.Id.ToString() == model.unitId);

                if (piano == null)
                    throw new Exception("Unit Not Found");

                var pod = piano.Order.PianoAssignment.PODs.FirstOrDefault(x => x.PianoId == piano.Id);

                if (pod == null)
                    throw new Exception("POD Not Found");

                string imageFileName, imageFilePath;
                try
                {
                    imageFileName = $"{model.unitId}\\{model.Id}.jpg";
                    imageFilePath = $"{PianoImagesPath}{imageFileName}";
                    string directoryPath = $"{PianoImagesPath}{model.unitId}";
                    if (!System.IO.Directory.Exists(directoryPath)) System.IO.Directory.CreateDirectory(directoryPath);

                    ImageUtility.Base64ToImage(model.image).Save(imageFilePath);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error saving image " + ex.Message);
                }

                if (!piano.Pictures.Any(x => x.Id.ToString() == model.Id))
                {
                    db.PianoPictures.Add(new PianoPicture()
                    {
                        Id = Guid.NewGuid(),
                        PianoPodId = pod.Id,
                        CreatedAt = DateTime.Now,
                        PictureType = (int)PictureTypeEnum.Piano,
                        Picture = imageFileName,
                    });
                }
                db.SaveChanges();

                return Json(new JsonResponse() { IsSucess = true, IsTokenValid = true });
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message });
            }
        }
    }
}
