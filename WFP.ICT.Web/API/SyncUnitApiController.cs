using System;
using System.Linq;
using System.Data.Entity;
using System.Web.Http;
using System.Web.Http.Results;
using WFP.ICT.Data.Entities;
using WFP.ICT.Enum;
using WFP.ICT.Enums;
using WFP.ICT.Web.API;
using WFP.ICT.Web.Helpers;
using WFP.ICT.Web.Models;
using System.Globalization;
using WFP.ICT.Web.Async;
using Hangfire;

namespace WFP.ICT.Web.Controllers
{
    public class SyncUnitApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/sync/unit/load")]
        public JsonResult<JsonResponse> SyncLoad([FromBody] SyncLoadModel model)
        {
            try
            {
                IsTokenValid(RequestTypeEnum.SyncLoad);

                var assignment = db.Assignments
                    .Include(x => x.Order)
                    .Include(x => x.Order.Pianos)
                    .FirstOrDefault(x => x.Id.ToString() == model.assignmentId);

                if (assignment == null)
                    throw new Exception("Assignment Not Found");

                var order = db.Orders
                    .Include(x => x.Pianos)
                    .FirstOrDefault(x => x.Id == assignment.OrderId);

                if (order == null)
                    throw new Exception("Assignment Order Not Found");

                var piano = order.Pianos.FirstOrDefault(x => x.Id.ToString() == model.Id);

                if (piano == null)
                    throw new Exception("Unit Not Found");

                string pickerSignature, pickerSignatureFilePath;
                try
                {
                    pickerSignature = $"{piano.Id.ToString()}_pickup.png";
                    pickerSignatureFilePath = $"{SignPath}{pickerSignature}";
                    ImageUtility.Base64ToImage(model.pickerSignature).Save(pickerSignatureFilePath);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error saving sign " + ex.Message);
                }

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
                        StatusBy = StringConstants.EncorePianoApp,
                    });
                }

                // Just save once
                var proof = assignment.Proofs.FirstOrDefault(x => x.AssignmentId == assignment.Id && x.PianoId == piano.Id && x.ProofType == (int)ProofTypeEnum.Pickup);
                Guid id = Guid.NewGuid();
                if (proof == null)
                {
                    db.Proofs.Add(new Proof()
                    {
                        Id = id,
                        CreatedAt = DateTime.Now,
                        ProofType = (int)ProofTypeEnum.Pickup,
                        AssignmentId = assignment.Id,
                        PianoId = piano.Id,
                        LoadTime = DateTime.ParseExact(model.loadingTimeStamp, StringConstants.TimeStampFormatApp, CultureInfo.InvariantCulture),
                        PickerName = model.pickerName,
                        PickerSignature = pickerSignature,
                        IsMainUnitLoaded = model.isMainUnitLoaded == 1,
                        AdditionalBench1Status = model.additionalBench1Status + 1, // Unknown
                        AdditionalBench2Status = model.additionalBench2Status + 1, // Unknown
                        AdditionalCasterCupsStatus = model.additionalCasterCupsStatus + 1,
                        AdditionalCoverStatus = model.additionalCoverStatus + 1,
                        AdditionalLampStatus = model.additionalLampStatus + 1,
                        AdditionalOwnersManualStatus = model.additionalOwnersManualStatus + 1,
                        AdditionalMisc1Status = model.AdditionalMisc1Status + 1,
                        AdditionalMisc2Status = model.AdditionalMisc2Status + 1,
                        AdditionalMisc3Status = model.AdditionalMisc3Status + 1
                    });
                }
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

                var assignment = db.Assignments
                    .Include(x => x.Proofs)
                    .Include(x => x.Order)
                    .Include(x => x.Order.Pianos)
                    .FirstOrDefault(x => x.Id.ToString() == model.assignmentId);

                if (assignment == null)
                    throw new Exception("Assignment Not Found");

                var order = db.Orders
                    .Include(x => x.Pianos)
                    .FirstOrDefault(x => x.Id == assignment.OrderId);

                if (order == null)
                    throw new Exception("Assignment Order Not Found");

                var piano = order.Pianos.FirstOrDefault(x => x.Id.ToString() == model.Id);

                if (piano == null)
                    throw new Exception("Unit Not Found");

                string receiverSignature, receiverSignatureFilePath;
                try
                {
                    receiverSignature = $"{piano.Id.ToString()}_delivery.png";
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
                var proof = assignment.Proofs.FirstOrDefault(x => x.AssignmentId == assignment.Id && x.PianoId == piano.Id && x.ProofType == (int)ProofTypeEnum.Delivery);
                Guid id = Guid.NewGuid();
                if (proof == null)
                {
                    db.Proofs.Add(new Proof()
                    {
                        Id = id,
                        CreatedAt = DateTime.Now,
                        ProofType = (int)ProofTypeEnum.Delivery,

                        ReceivedBy = model.receiverName,
                        Signature = receiverSignature,
                        ReceivingTime = DateTime.ParseExact(model.deliveredAt, StringConstants.TimeStampFormatApp, CultureInfo.InvariantCulture),
                        Notes = StringConstants.EncorePianoApp,

                        Bench1UnloadStatus = model.bench1Unloaded == 1,
                        Bench2UnloadStatus = model.bench2Unloaded == 1,
                        CasterCupsUnloadStatus = model.casterCupsUnloaded == 1,
                        CoverUnloadStatus = model.coverUnloaded == 1,
                        LampUnloadStatus = model.lampUnloaded == 1,
                        OwnersManualUnloadStatus = model.ownersManualUnloaded == 1,
                        Misc1UnloadStatus = model.misc1Unloaded == 1,
                        Misc2UnloadStatus = model.misc2Unloaded == 1,
                        Misc3UnloadStatus = model.misc3Unloaded == 1,
                        AssignmentId = assignment.Id,
                        PianoId = piano.Id,
                    });
                }
                db.SaveChanges();

                if(!string.IsNullOrEmpty(order.DeliveryForm))
                    BackgroundJob.Enqueue(() => PODFormsGenerater.GeneratePODForm(db, UploadsFormsPath, DownloadsFormsPath, SignPath, id) ); 

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
                    //.Include(x => x.Order)
                    //.Include(x => x.Order.Assignments)
                    //.Include(x => x.Order.Assignments.Select(y => y.Proofs))
                    .FirstOrDefault(x => x.Id.ToString() == model.unitId);

                if (piano == null)
                    throw new Exception("Unit Not Found");

                PictureTypeEnum pictureType = (PictureTypeEnum)System.Enum.Parse(typeof(PictureTypeEnum), model.takeLocation);

                var pod = db.Proofs.FirstOrDefault(x => x.AssignmentId.ToString() == model.assignmentId && x.PianoId == piano.Id && x.ProofType == (int)pictureType);

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
                        PianoId = piano.Id,
                        ProofId = pod.Id,
                        CreatedAt = DateTime.Now,
                        PictureType = (int)pictureType,
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
