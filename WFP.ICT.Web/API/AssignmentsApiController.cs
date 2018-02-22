using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using Nelibur.ObjectMapper;
using WFP.ICT.Enum;
using WFP.ICT.Enums;
using WFP.ICT.Web.API;
using WFP.ICT.Web.API.Models;
using WFP.ICT.Web.Models;
using WFP.ICT.Data.Entities;

namespace WFP.ICT.Web.Controllers
{
    public class AssignmentsApiController : BaseApiController
    {
        // POST: api/assignment
        [HttpGet]
        [Route("api/assignments")]
        public JsonResult<JsonResponse> GetAssignments()
        {
            try
            {
                IsTokenValid(RequestTypeEnum.GetAssignments);
                return Json(new JsonResponse() { IsSucess = true, IsTokenValid = true, Result = Load() });
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message });
            }
        }

        // POST: api/consignment
        //[HttpPost]
        [Route("api/assignment")]
        public JsonResult<JsonResponse> GetAssignment()
        {
            try
            {
                IsTokenValid(RequestTypeEnum.GetAssignment);

                var requestParams = Request.GetQueryNameValuePairs().ToList();
                var valueParam = requestParams.FirstOrDefault(x => x.Key.Equals(RequestParamsEnum.Id.ToString()));

                if (valueParam.Equals(default(KeyValuePair<string, string>)))
                    throw new Exception("Id is missing.");

                return Json(new JsonResponse() { IsSucess = true, IsTokenValid = true, Result = Load(valueParam.Value) });
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message });
            }
        }

        private List<AssignmentPianoResponse> Load(string id = "")
        {
            List<AssignmentPianoResponse> responses = new List<AssignmentPianoResponse>();
            var pianoTypes = db.PianoTypes.ToList();
            var pianoSizes = db.PianoSize.ToList();
            var pianoFinishs = db.PianoFinish.ToList();
            var pianoMakes = db.PianoMake.ToList();

            var assignments = db.Assignments
                .Include(x => x.Leg) // will be loaded next
                .Include(x => x.Leg.FromLocation)
                .Include(x => x.Leg.ToLocation)
                .Include(x => x.Order)
                .Include(x => x.Drivers)
                .Include(x => x.Vehicle)
                .Include(x => x.Route);

            if (!string.IsNullOrEmpty(id))
                assignments = assignments.Where(x => x.Id.ToString() == id);

            List<AssignmentResponse> assignmentResponses = new List<AssignmentResponse>();
            List<Guid> orderIds = new List<Guid>();

            foreach (var assignment in assignments.ToList())
            {
                var order = db.Orders
                    .Include(x => x.Client)
                    .Include(x => x.Pianos)
                    .Include(x => x.PickupAddress)
                    .Include(x => x.DeliveryAddress)
                    .Include(x => x.OrderCharges)
                    .FirstOrDefault(x => x.Id == assignment.OrderId);

                if (order == null)
                    throw new Exception("Order not found. Wrong Assignment.");
                orderIds.Add(order.Id);

                var assignmentResponse = new AssignmentResponse()
                {
                    Id = assignment.Id.ToString(),
                    AssignmentNumber = assignment.AssignmentNumber,
                    VehicleCode = assignment.Vehicle.Code,
                    VehicleName = assignment.Vehicle.Code + " " + assignment.Vehicle.Name,
                    DriverCode = String.Join(",", assignment.Drivers.Select(x => x.Code).ToList()),
                    DriverName = String.Join(",", assignment.Drivers.Select(x => x.Code + " " + x.Name).ToList()),

                    OrderId = order.Id.ToString(),
                    OrderNumber = order.OrderNumber,
                    OrderedAt = order.CreatedAt.ToString(StringConstants.TimeStampFormatApp),
                    OrderType = ((OrderTypeEnum)order.OrderType).ToString(),
                    CallerName = order.CallerFirstName + " " + order.CallerLastName,
                    CallerPhoneNumber = order.CallerPhoneNumber,
                    CallerPhoneNumberAlt = order.CallerAlternatePhone,
                    CallerEmail = order.CallerEmail,

                    PickupDate = order.PickupDate?.ToString(),
                    PickupAddress = TinyMapper.Map<AddressVm>(order.PickupAddress).ToStringWithoutBreak,
                    PickupName = order.PickupAddress.Name,
                    PickupPhoneNumber = order.PickupAddress.PhoneNumber,
                    PickupAlternateContact = order.PickupAddress.AlternateContact,
                    PickupAlternatePhone = order.PickupAddress.AlternatePhone,
                    PickupNumberStairs = order.PickupAddress.NumberStairs,
                    PickupNumberTurns = order.PickupAddress.NumberTurns,
                    PickupInstructions = order.PickUpNotes,

                    DeliveryDate = order.DeliveryDate?.ToString(),
                    DeliveryAddress = TinyMapper.Map<AddressVm>(order.DeliveryAddress).ToStringWithoutBreak,
                    DeliveryName = order.DeliveryAddress.Name,
                    DeliveryPhoneNumber = order.DeliveryAddress.PhoneNumber,
                    DeliveryAlternateContact = order.DeliveryAddress.AlternateContact,
                    DeliveryAlternatePhone = order.DeliveryAddress.AlternatePhone,
                    DeliveryNumberStairs = order.DeliveryAddress.NumberStairs,
                    DeliveryNumberTurns = order.DeliveryAddress.NumberTurns,
                    DeliveryInstructions = order.DeliveryNotes,

                    CustomerCode = order.ClientId.HasValue ? order.Client.AccountCode : "",
                    CustomerName = order.ClientId.HasValue ? order.Client.Name : "",
                    PaymentOption = ((PaymentOptionEnum)order.PaymentOption).ToString(),
                    PaymentAmount = order.PaymentOption == 0 ? order.CodAmount.ToString() : (order.PaymentOption == 1 ? order.OfficeStaff : order.OnlinePayment),

                    NumberOfItems = order.Pianos.Count,
                    Route = assignment.Route.OrderBy(x => x.Order)
                          .Select(x => new RouteResponse()
                          {
                              ConsignmentId = assignment.Id.ToString(),
                              Lat = x.Lat,
                              Lng = x.Lng,
                              Order = x.Order
                          }).ToArray()
                };
                assignmentResponse.LegDate = assignment.Leg.LegDate.ToString();
                assignmentResponse.LegFromLocation = assignment.Leg.FromLocation.Name;
                assignmentResponse.LegToLocation = assignment.Leg.ToLocation.Name;
                assignmentResponses.Add(assignmentResponse);
            }

            // Pianos
            var pianoResponses = db.Pianos
                          .Include(x => x.Order)
                          .Where(x => orderIds.Contains(x.OrderId.Value)).OrderBy(x => x.CreatedAt)
                          .ToList()
                          .Select(x => new PianoResponse()
                          {
                              Id = x.Id.ToString(),
                              OrderId = x.OrderId.ToString(),
                              Category = System.Enum.GetName(typeof(PianoCategoryTypeEnum), x.PianoCategoryType),
                              Type = x.PianoTypeId.HasValue ? pianoTypes.FirstOrDefault(y => y.Id == x.PianoTypeId).Type : "",
                              Size = x.PianoSizeId.HasValue ? pianoSizes.FirstOrDefault(y => y.Id == x.PianoSizeId).Width.ToString() : "",
                              Make = x.PianoMakeId.HasValue ? pianoMakes.FirstOrDefault(y => y.Id == x.PianoMakeId).Name : "",
                              Model = x.Model,
                              Finish = x.PianoFinishId.HasValue ? pianoFinishs.FirstOrDefault(y => y.Id == x.PianoFinishId).Name : "",
                              SerialNumber = x.SerialNumber,
                              IsBench = x.IsBench ? 1 : 0,
                              IsPlayer = x.IsPlayer ? 1 : 0,
                              IsBoxed = x.IsBoxed ? 1 : 0,
                          }).ToArray();

            responses.Add(new AssignmentPianoResponse()
            {
                Assignments = assignmentResponses.ToArray(),
                Pianos = pianoResponses
            });
            return responses;
        }
        
    }
}
