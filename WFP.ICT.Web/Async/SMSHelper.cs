using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Twilio;
using Twilio.AspNet.Common;
using Twilio.Clients;
using Twilio.Http;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using WFP.ICT.Web.Models;

namespace WFP.ICT.Web.Async
{
    public class SMSHelper
    {
        public static void Send(string to, string message)
        {
            string accountSID = "AC5e13872eb6539292701e5cd26104f29d";
            string authToken = "0318f1e7f66bca3f069a208dca6d061b";

            TwilioClient.Init(accountSID, authToken);

            //var call = CallResource.Create(
            //    new PhoneNumber("+11234567890"),
            //    from: new PhoneNumber("+10987654321"),
            //    url: new Uri("https://my.twiml.here")
            //);
            //Console.WriteLine(call.Sid);

            var msg = MessageResource.Create(
                from: new PhoneNumber("+13219856111"),
                to: new PhoneNumber(to),
                body: message
            );
            Console.WriteLine(msg.Sid);
        }

        public static void Send(OrderVm orderVm)
        {
            string caller = orderVm.CallerFirstName + " " + orderVm.CallerLastName;
            string message = string.Format("Dear {0}, We have received your Move Order #({1}). It will be completed soon.Encore Piano Moving.", caller, orderVm.OrderNumber);
            string callerNumber = orderVm.CallerPhoneNumber;
            if(!string.IsNullOrEmpty(callerNumber)) 
                Send(callerNumber,message);
        }
    }
}