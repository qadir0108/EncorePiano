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

namespace WFP.ICT.Web.Controllers
{
    public class AssignmentsController : BaseApiController
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

        private List<AssignmentResponse> Load(string id = "")
        {
            List<AssignmentResponse> responses = new List<AssignmentResponse>();

            var consignments = db.PianoAssignments
                .Include(x => x.Drivers)
                .Include(x => x.Vehicle)
                .Include(x => x.Route);

            if (!string.IsNullOrEmpty(id))
                consignments = consignments.Where(x => x.Id.ToString() == id);

            var consignmentsList = consignments.ToList();

            foreach (var consignment in consignmentsList)
            {

                var order = db.PianoOrders
                    .Include(x => x.Customer)
                    .Include(x => x.Pianos)
                    .Include(x => x.PickupAddress)
                    .Include(x => x.DeliveryAddress)
                    .Include(x => x.OrderCharges)
                    .FirstOrDefault(x => x.Id == consignment.PianoOrderId);
                if (order == null)
                    throw new Exception("Order not found. Wrong consignment.");

                var pianoTypes = db.PianoTypes.ToList();
                var pianoSizes = db.PianoSize.ToList();
                var pianoFinishs = db.PianoFinish.ToList();
                var pianoMakes = db.PianoMake.ToList();
                var consignmentResponse = new AssignmentResponse()
                {
                    Id = consignment.Id.ToString(),
                    ConsignmentNumber = consignment.AssignmentNumber,
                    VehicleCode = consignment.Vehicle.Code,
                    VehicleName = consignment.Vehicle.Code + " " + consignment.Vehicle.Name,
                    DriverCode = String.Join(",", consignment.Drivers.Select(x => x.Code).ToList()),
                    DriverName = String.Join(",", consignment.Drivers.Select(x => x.Code + " " + x.Name).ToList()),

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
                    PickupPhoneNumber = order.PickupAddress.PhoneNumber,
                    PickupAlternateContact = order.PickupAddress.AlternateContact,
                    PickupAlternatePhone = order.PickupAddress.AlternatePhone,
                    PickupNumberStairs = order.PickupAddress.NumberStairs,
                    PickupNumberTurns = order.PickupAddress.NumberTurns,
                    PickupInstructions = order.PickUpNotes,

                    DeliveryDate = order.DeliveryDate?.ToString(),
                    DeliveryAddress = TinyMapper.Map<AddressVm>(order.DeliveryAddress).ToStringWithoutBreak,
                    DeliveryPhoneNumber = order.DeliveryAddress.PhoneNumber,
                    DeliveryAlternateContact = order.DeliveryAddress.AlternateContact,
                    DeliveryAlternatePhone = order.DeliveryAddress.AlternatePhone,
                    DeliveryNumberStairs = order.DeliveryAddress.NumberStairs,
                    DeliveryNumberTurns = order.DeliveryAddress.NumberTurns,
                    DeliveryInstructions = order.DeliveryNotes,

                    CustomerCode = order.CustomerId.HasValue ? order.Customer.AccountCode : "",
                    CustomerName = order.CustomerId.HasValue ? order.Customer.Name : "",

                    NumberOfItems = order.Pianos.Count,
                    Pianos = order.Pianos.OrderBy(x => x.CreatedAt)
                        .Select(x => new PianoResponse()
                        {
                            Id = x.Id.ToString(),
                            ConsignmentId = consignment.Id.ToString(),
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
                        }).ToArray(),
                    Route = consignment.Route.OrderBy(x => x.Order)
                        .Select(x => new RouteResponse()
                        {
                            ConsignmentId = consignment.Id.ToString(),
                            Lat = x.Lat,
                            Lng = x.Lng,
                            Order = x.Order
                        }).ToArray()
                };
                responses.Add(consignmentResponse);
            }
            return responses;
        }
    }
}
