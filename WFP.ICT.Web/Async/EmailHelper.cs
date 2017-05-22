using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Mail;
using System.Threading.Tasks;
using WFP.ICT.Web.Helpers;

namespace WFP.ICT.Web.Async
{
    public class EmailHelper
    {
        private const string Footer = @"<br/><br/>Thanks<br/>
                                        <b>ADS Data Direct</b><br/>http://www.adsdatadirect.com";

        public static void SendErrorEmail(string to, Exception ex, string currentController, string currentAction)
        {
            if (ConfigurationManager.AppSettings["IsLive"] == "true")
            {
                string subject = "ADSLive: An error has been occured.";
                string body = string.Format(@"<p>ADSLive: An error has been occured.</p>
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