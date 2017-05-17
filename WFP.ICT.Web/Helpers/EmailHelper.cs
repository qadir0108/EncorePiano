using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Mail;
using System.Threading.Tasks;
using WFP.ICT.Data.Entities;

namespace WFP.ICT.Web.Helpers
{
    public class EmailHelper
    {
        //private const string Footer = @"<br/><br/>Thanks<br/>
        //                                <b>ADS Data Direct</b><br/>http://www.adsdatadirect.com";

        //public static async Task SendOrderEmailToClient(WFPUser loggedInUser, Campaign campaign)
        //{
        //    string subject = string.Format("Order # {0}, Campaign Date {1}, QTY {2} , Date Submitted {3}",
        //                            campaign.OrderNumber, campaign.BroadcastDate.Value.ToString("d"), campaign.Quantity, campaign.CreatedAt.ToString("d"));

        //    string body = string.Format(@"<br/><p>Dear {0}</p><br/>
        //                               We have recevied your order.<br/><br/>
        //                                <table border=""2"">
        //                                <tr><th>Order #:</th><td>{1}</td></tr>
        //                                <tr><th>Status:</th><td>{2}</td></tr>
        //                                <tr><th>Username:</th><td>{3}</td></tr>
        //                                <tr><th>User Email:</th><td>{4}</td></tr>
        //                                <tr><th>Campaign Name:</th><td>{5}</td></tr>
        //                                <tr><th>Broadcast Date:</th><td>{6}</td></tr>
        //                                <tr><th>Rebroadcast:</th><td>{7}</td></tr>
        //                                <tr><th>Rebroadcast Date:</th><td>{8}</td></tr>
        //                                <tr><th>From Line:</th><td>{9}</td></tr>
        //                                <tr><th>Subject Line:</th><td>{10}</td></tr>
        //                                <tr><th>Personalization:</th><td>{11}</td></tr>
        //                                <tr><th>Matchback:</th><td>{12}</td></tr>
        //                                <tr><th>Suppression:</th><td>{13}</td></tr>
        //                                <tr><th>White Label:</th><td>{14}</td></tr>
        //                                <tr><th>HTML File:</th><td>{15}</td></tr>
        //                                <tr><th>Test Seed:</th><td>{16}</td></tr>  
        //                                <tr><th>Live Seed:</th><td>{17}</td></tr>   
        //                                <tr><th>Special Instructions:</th><td>{18}</td></tr>                                     
        //                                <tr><th>Start Time:</th><td>{19}</td></tr>
        //                                <tr><th>IP:</th><td>{20}</td></tr>
        //                                <tr><th>Browser:</th><td>{21}</td></tr>
        //                                <tr><th>OS:</th><td>{22}</td></tr>
        //                                <tr><th>Referrer:</th><td>{23}</td></tr>
        //                                </table></p> <p>We will soon contact you and update you about your order.</p> {24}"
        //                              , loggedInUser.UserName.ToCapitalLetterString().ToCapitalized(), campaign.OrderNumber, (CampaignStatusEnum)campaign.Status
        //                              , loggedInUser.UserName, loggedInUser.Email, campaign.CampaignName, campaign.BroadcastDate.Value.ToString("d")
        //                              , campaign.ReBroadCast ? "Yes" : "No", campaign.ReBroadcastDate.HasValue ? campaign.ReBroadcastDate.ToString() : "", campaign.FromLine, campaign.SubjectLine, campaign.IsPersonalization ? "Yes" : "No"
        //                              , campaign.IsMatchback ? "Yes" : "No", campaign.IsSuppression ? "Yes" : "No" 
        //                              , campaign.WhiteLabel, campaign.HtmlImageFiles, campaign.TestSeedList, campaign.FinalSeedList, campaign.SpecialInstructions 
        //                              , campaign.CreatedAt.AddMinutes(2).AddSeconds(30).ToString()
        //                              , campaign.IP, campaign.Browser, campaign.OS, campaign.Referrer
        //                              , Footer);
        //    await SendEmailAsync(loggedInUser.Email, subject, body);
        //}

        //public static async Task SendApprovedToVendor(Vendor vendor, Campaign campaign)
        //{
        //    string newOld = !campaign.RebroadId.HasValue ? "New" : "RDP";
        //    string orderNumber = campaign.OrderNumber;
        //    string deployDate = campaign.Approved.DeployDate.Value.ToString("d");
        //    string deployTime = campaign.Approved.DeployDate.Value.ToString("hh:mm");
        //    string quantity = campaign.Approved.Quantity.ToString();

        //    string subject = string.Format("{0} Order {1}, Order # {2}",
        //                            newOld, campaign.CampaignName, campaign.OrderNumber);

        //    string body = string.Format(@"<br/><p>Dear {0}</p><br/>
        //                               Please find below Order details<br/><br/>
        //                                <table border=""2"">
        //                                <tr><th>Reference #:</th><td>{1}</td></tr>
        //                                <tr><th>Order #:</th><td>{2}</td></tr>
        //                                <tr><th>Campaign Name:</th><td>{3}</td></tr>
        //                                <tr><th>Rebroadcast:</th><td>{4}</td></tr>                                        
        //                                <tr><th>Broadcast Date:</th><td>{5}</td></tr>
        //                                <tr><th>From Line:</th><td>{6}</td></tr>
        //                                <tr><th>Subject Line:</th><td>{7}</td></tr>
        //                                <tr><th>Opt Out:</th><td>{8}</td></tr>
        //                                <tr><th>White Label:</th><td>{9}</td></tr>                                        
        //                                <tr><th>Personalization:</th><td>{10}</td></tr>
        //                                <tr><th>Creative URL:</th><td>{11}</td></tr>
        //                                <tr><th>Quantity:</th><td>{12}</td></tr>
        //                                <tr><th>Geo (s):</th><td>{13}</td></tr>
        //                                <tr><th>Demographics:</th><td>{14}</td></tr>
        //                                <tr><tr><th>Zip URL:</th><td>{15}</td></tr>
        //                                <tr><th>Special Instructions:</th><td>{16}</td></tr>
        //                                <tr><th>Deploy Date:</th><td>{17}</td></tr>
        //                                <tr><th>Deploy Time:</th><td>{18}</td></tr>
        //                                <tr><th>ReportSite Link:</th><td>{19}</td></tr>
        //                                <tr><th>Link Breakout:</th><td>{20}</td></tr>
        //                                </table></p> <p></p> {21}"
        //                              , vendor.Name, campaign.Approved.ReferenceNumber, orderNumber, campaign.Approved.CampaignName
        //                              , campaign.Approved.ReBroadCast ? "Yes" : "No", campaign.Approved.DeployDate.Value.ToString("d")
        //                              , campaign.Approved.FromLine, campaign.Approved.SubjectLine, campaign.OptOut, campaign.Approved.WhiteLabel
        //                              , campaign.IsPersonalization ? "Yes" : "No", campaign.Approved.CreativeURL, quantity
        //                              , campaign.Approved.GeoDetails, campaign.Approved.Demographics, campaign.Approved.ZipURL
        //                              , campaign.Approved.SpecialInstructions, deployDate, deployTime
        //                              , campaign.Approved.ReportSiteLink, campaign.Approved.LinkBreakout
        //                              , Footer);
        //    await SendEmailAsync(vendor.Email, subject, body, vendor.CCEmails);
        //}

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

        public static async Task SendEmailAsync(string to, string subject, string body, string ccEmails = "", List<string> attachments = null)
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