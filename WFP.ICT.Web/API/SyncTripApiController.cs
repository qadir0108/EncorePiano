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

namespace WFP.ICT.Web.Controllers
{
    public class SyncTripApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/sync/trip/start")]
        public JsonResult<JsonResponse> SyncStart([FromBody] SyncStartModel model)
        {
            try
            {
                IsTokenValid(RequestTypeEnum.SyncTripStart);

                var assignment = db.Assignments
                .Include(x => x.Proofs)
                .Include(x => x.Order)
                .Include(x => x.Order.Legs)
                .Include(x => x.Statuses)
                .FirstOrDefault(x => x.Id.ToString() == model.Id);

                if (assignment == null)
                    throw new Exception("Assignment Not Found");

                if (!assignment.Statuses.Any(x => x.Status == (int)TripStatusEnum.Started))
                {
                    db.TripStatuses.Add(new TripStatus()
                    {
                        Id = Guid.NewGuid(),
                        AssignmentId = assignment.Id,
                        CreatedAt = DateTime.Now,
                        Status = (int)TripStatusEnum.Started,
                        Comments = "Started",
                        StatusTime = DateTime.Now,
                        StatusBy = StringConstants.System,
                    });
                }

                assignment.StartTime = DateTime.ParseExact(model.departureTime, StringConstants.TimeStampFormatApp, CultureInfo.InvariantCulture);
                assignment.EstimatedTime = DateTime.ParseExact(model.estimatedTime, StringConstants.TimeStampFormatApp, CultureInfo.InvariantCulture);
                db.SaveChanges();

                return Json(new JsonResponse() { IsSucess = true, IsTokenValid = true });
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message });
            }
        }

        [HttpPost]
        [Route("api/sync/trip/status")]
        public JsonResult<JsonResponse> SyncStatus([FromBody] SyncStartModel model)
        {
            try
            {
                IsTokenValid(RequestTypeEnum.SyncTripStart);

                var assignment = db.Assignments
                .Include(x => x.Proofs)
                .Include(x => x.Statuses)
                .FirstOrDefault(x => x.Id.ToString() == model.Id);

                if (assignment == null)
                    throw new Exception("Assignment Not Found");

                var tripStatus = (TripStatusEnum)System.Enum.Parse(typeof(TripStatusEnum), model.tripStatus);

                if (!assignment.Statuses.Any(x => x.Status == (int)tripStatus))
                {
                    db.TripStatuses.Add(new TripStatus()
                    {
                        Id = Guid.NewGuid(),
                        AssignmentId = assignment.Id,
                        CreatedAt = DateTime.Now,
                        Status = (int)tripStatus,
                        Comments = "",
                        StatusTime = DateTime.Now,
                        StatusBy = StringConstants.System,
                    });
                }

                assignment.StartTime = DateTime.ParseExact(model.departureTime, StringConstants.TimeStampFormatApp, CultureInfo.InvariantCulture);
                assignment.EstimatedTime = DateTime.ParseExact(model.estimatedTime, StringConstants.TimeStampFormatApp, CultureInfo.InvariantCulture);
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
