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
    public class consignmentsController : baseapiController
    {
        // POST: api/consignment
        [HttpGet]
        [Route("api/consignments")]
        public JsonResult<JsonResponse> GetConsignments()
        {
            try
            {
                IsTokenValid(RequestTypeEnum.GetConsignments);
                return Json(new JsonResponse() { IsSucess = true, IsTokenValid = true, Result = Load() });
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message });
            }
        }

        // POST: api/consignment
        //[HttpPost]
        [Route("api/consignment")]
        public JsonResult<JsonResponse> GetConsignment()
        {
            try
            {
                IsTokenValid(RequestTypeEnum.GetConsignment);

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

        private List<ConsignmentResponse> Load(string id = "")
        {
            List<ConsignmentResponse> responses = new List<ConsignmentResponse>();

            var consignments = db.PianoConsignments
                .Include(x => x.Driver)
                .Include(x => x.Vehicle)
                .Include(x => x.WarehouseStart)
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
                var warehouse = db.Warehouses.Include(x => x.Address).FirstOrDefault(x => x.Id == consignment.WarehouseStartId);
                var consignmentResponse = new ConsignmentResponse()
                {
                    Id = consignment.Id.ToString(),
                    ConsignmentNumber = consignment.ConsignmentNumber,
                    StartWarehouseName = warehouse.Code + " " + warehouse.Name,
                    StartWarehouseAddress = TinyMapper.Map<AddressVm>(warehouse.Address).AddressToStringWithoutBreak,
                    VehicleCode = consignment.Vehicle.Code,
                    VehicleName = consignment.Vehicle.Code + " " + consignment.Vehicle.Name,
                    DriverCode = consignment.Driver.Code,
                    DriverName = consignment.Driver.Code + " " + consignment.Driver.Name,

                    OrderId = order.Id.ToString(),
                    OrderNumber = order.OrderNumber,
                    OrderedAt = order.CreatedAt.ToString(StringConstants.TimeStampFormatApp),
                    OrderType = ((OrderTypeEnum)order.OrderType).ToString(),
                    CallerName = order.CallerFirstName + " " + order.CallerLastName,
                    CallerPhoneNumber = order.CallerPhoneNumber,
                    SpecialInstructions = order.Notes,
                    PickupAddress = TinyMapper.Map<AddressVm>(order.PickupAddress).AddressToStringWithoutBreak,
                    DeliveryAddress = TinyMapper.Map<AddressVm>(order.DeliveryAddress).AddressToStringWithoutBreak,
                    NumberOfItems = order.Pianos.Count,
                    Pianos = order.Pianos.OrderBy(x => x.CreatedAt)
                        .Select(x => new PianoResponse()
                        {
                            Id = x.Id.ToString(),
                            ConsignmentId = consignment.Id.ToString(),
                            Type = pianoTypes.FirstOrDefault(y => y.Id == x.PianoTypeId).Type,
                            Name = x.Name,
                            Color = x.Color,
                            Model = x.Model,
                            Make = x.Make,
                            SerialNumber = x.SerialNumber,
                            IsBench = x.IsBench ? 1 : 0,
                            IsBoxed = x.IsBoxed ? 1 : 0,
                            IsStairs = x.IsPlayer ? 1 : 0
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
