using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using FirebaseNet.Messaging;

namespace WFP.ICT.Web.FCM
{
    public enum MessageTypeEnum
    {
        Message,
        Consignment
    }

    public class FCMUitlity
    {
        public static void SendConsignment(string to, string id)
        {
            FCMClient client = new FCMClient(ConfigurationSettings.AppSettings["FCMServerKey"]);

            var message = new Message()
            {
                To = to,
                Notification = new AndroidNotification()
                {
                    Body = "A new pickup order is arrived!",
                    Title = "Encore Piano App",
                    Icon = "ic_launcher"
                },
                Data = new Dictionary<string, string>
                {
                    { "MessageType", MessageTypeEnum.Consignment.ToString()},
                    { "Id", id }
                }
            };

            var request = client.SendMessageAsync(message);
            var result = request.Result;
        }

        public static void SendMessage(Guid RecepientID, Guid MessageID)
        {
            // Get Vehicle By User
            // Get GCMRegistrationId from RecepientID
            //Transfocus.PODDelivery.BLL.Vehicles Vehicles = new Transfocus.PODDelivery.BLL.Vehicles();
            
        }
    }
}