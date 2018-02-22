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
using Stripe;

namespace WFP.ICT.Web.Controllers
{
    public class SyncPaymentApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/payment/process")]
        public JsonResult<JsonResponse> Process([FromBody] PaymentModel model)
        {
            try
            {
                IsTokenValid(RequestTypeEnum.PaymentProcess);

                var assignment = db.Assignments
                    .Include(x => x.Order)
                    .FirstOrDefault(x => x.Id.ToString() == model.assignmentId);

                if (assignment == null)
                    throw new Exception("Assignment Not Found");

                if (assignment.Order == null)
                    throw new Exception("Assignment Order Not Found");

                var order = assignment.Order;
                string PaymentAmountString = order.PaymentOption == 0 ? order.CodAmount.ToString() : (order.PaymentOption == 1 ? order.OfficeStaff : order.OnlinePayment);
                int PaymentAmount = 0;
                Int32.TryParse(PaymentAmountString, out PaymentAmount);
                PaymentAmount = PaymentAmount * 100;

                StripeConfiguration.SetApiKey("sk_test_b9puC5Zu17mi3iFgdhfJnsou");

                // Token is created using Checkout or Elements!
                // Get the payment token submitted by the form:
                var token = model.token; // Using ASP.NET MVC

                // Charge the user's card:
                var charges = new StripeChargeService();
                var charge = charges.Create(new StripeChargeCreateOptions
                {
                    Amount = PaymentAmount,
                    Currency = "usd",
                    SourceTokenOrExistingSourceId = token,
                    Description = assignment.AssignmentNumber,
                    StatementDescriptor = "Encore Piano Moving"
                });

                if (!"succeeded".Equals(charge.Status))
                    throw new Exception("There is error while processing payment at Stripe.");

                var ifAlreadyPaid = db.Payments.Any(x => x.OrderId == assignment.OrderId);
                if (!ifAlreadyPaid)
                {
                    db.Payments.Add(new ClientPayment()
                    {
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTime.Now,

                        OrderId = assignment.OrderId,
                        ClientId = order.ClientId,

                        PaymentType = (int)PaymentTypeEnum.CreditCard,
                        Amount = PaymentAmount,
                        TransactionNumber = charge.Id,
                        PaymentDate = charge.Created,
                    });
                    db.SaveChanges();
                }

                return Json(new JsonResponse() { IsSucess = true, IsTokenValid = true });
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message });
            }
        }

    }
}
