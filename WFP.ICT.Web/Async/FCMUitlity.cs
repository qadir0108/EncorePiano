using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using FirebaseNet.Messaging;
using WFP.ICT.Data.Entities;

namespace WFP.ICT.Web.FCM
{
    public enum MessageTypeEnum
    {
        Message,
        Consignment
    }

    public class FCMUitlity
    {
        public static void SendConsignment(string driverToken, string consignmentId, string consignmentNumber)
        {
            FCMClient client = new FCMClient(ConfigurationSettings.AppSettings["FCMServerKey"]);
            
            var message = new Message()
            {
                To = driverToken,
                Notification = new AndroidNotification()
                {
                    Body = "A new pickup order is arrived!",
                    Title = "Encore Piano App",
                    Icon = "ic_launcher"
                },
                Data = new Dictionary<string, string>
                {
                    { "MessageType", MessageTypeEnum.Consignment.ToString()},
                    { "Id", consignmentId }
                }
            };

          //  var request = client.SendMessageAsync(message);
           // var result = request.Result;
        }

        public static void SendMessage(Guid RecepientID, Guid MessageID)
        {
            // Get Vehicle By User
            // Get GCMRegistrationId from RecepientID
            //Transfocus.PODDelivery.BLL.Vehicles Vehicles = new Transfocus.PODDelivery.BLL.Vehicles();
            
        }
    }
}