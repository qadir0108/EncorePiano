using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Mail;
using System.Threading.Tasks;
using WFP.ICT.Data.Entities;
using WFP.ICT.Web.Helpers;
using WFP.ICT.Web.Models;

namespace WFP.ICT.Web.Async
{
    public class EmailHelper
    {
        private const string Footer = @"<br/><br/>Thanks<br/>
                                        <b>Encore Piano Moving</b><br/>http://www.movemypiano.com";

        public static async Task SendOrderEmailToClient(OrderVm orderVm)
        {
            string subject = string.Format("Order # {0} has been received.", orderVm.OrderNumber);

            string body = string.Format(@"<br/><p>Dear {0}</p><br/>
                                       We have recevied your order.<br/><br/>
                                        <table border=""2"">
                                        <tr><th>Order #:</th><td>{1}</td></tr>
                                        <tr><th>Pickup Address:</th><td>{2}</td></tr>
                                        <tr><th>Delivery Address:</th><td>{3}</td></tr>
                                        </table></p> <p>We will soon contact you and update you about your order.</p> {4}"
                                      , orderVm.CallerName, orderVm.OrderNumber
                                      , orderVm.PickupAddressString, orderVm.DeliveryAddressString
                                      , Footer);
            await SendEmailAsync(orderVm.CallerEmail, subject, body);
        }

        public static async Task SendQuoteEmailToClient(OrderVm orderVm)
        {
            string subject = string.Format("Move Order Quote # {0}", orderVm.OrderNumber);

            string body = string.Format(@"<br/><p>Dear {0}</p><br/>
                                       Your move order quote is below.<br/><br/>
                                        <table border=""2"">
                                        <tr><th>Quote #:</th><td>{1}</td></tr>
                                        <tr><th>Pickup Address:</th><td>{2}</td></tr>
                                        <tr><th>Delivery Address:</th><td>{3}</td></tr>
                                        </table></p> <p>We will soon contact you and update you about your order.</p> {4}"
                                      , orderVm.CallerName, orderVm.OrderNumber
                                      , orderVm.PickupAddressString, orderVm.DeliveryAddressString
                                      , Footer);
            await SendEmailAsync(orderVm.CallerEmail, subject, body);
        }

        public static void SendErrorEmail(string to, Exception ex, string currentController, string currentAction)
        {
            if (ConfigurationManager.AppSettings["IsLive"] == "true")
            {
                string subject = "EncoreLive: An error has been occured.";
                string body = string.Format(@"<p>EncoreLive: An error has been occured.</p>
                            <p>Controller : {0}</p><p>Action : {1}</p>
                            <p>Error Details : {2}</p>", currentController, currentAction, ex.GetExceptionMessage());
                SendEmail(to, subject, body);
            }
        }

        public static async Task SendEmailAsync(string to, string subject, string body, string ccEmails = "", List < string> attachments = null)
        {
            using (var smtpClient = new SmtpClient())
            {
                MailMessage msg = new MailMessage();
                msg.To.Add(new MailAddress(to));
                if (!string.IsNullOrEmpty(ccEmails))
                {
                    foreach (var ccEmail in ccEmails.Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                    {
                        msg.CC.Add(ccEmail);
                    }
                }
                    
                msg.Subject = subject;
                msg.IsBodyHtml = true;
                msg.Body = body;
                if (attachments != null)
                    foreach (var attachment in attachments)
                    {
                        msg.Attachments.Add(new System.Net.Mail.Attachment(attachment));
                    }
                await smtpClient.SendMailAsync(msg);
            }
        }

        public static void SendEmail(string to, string subject, string body, string ccEmails = "", List<string> attachments = null)
        {
            using (var smtpClient = new SmtpClient())
            {
                MailMessage msg = new MailMessage();
                msg.To.Add(new MailAddress(to));
                if (!string.IsNullOrEmpty(ccEmails))
                {
                    foreach (var ccEmail in ccEmails.Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                    {
                        msg.CC.Add(ccEmail);
                    }
                }

                msg.Subject = subject;
                msg.IsBodyHtml = true;
                msg.Body = body;
                if (attachments != null)
                    foreach (var attachment in attachments)
                    {
                        msg.Attachments.Add(new System.Net.Mail.Attachment(attachment));
                    }
                smtpClient.Send(msg);
            }
        }
    }
}