using System.Collections.Generic;
using System.Web.Mvc;

namespace WFP.ICT.Web.Models
{
    public class CampaignReportVM
    {
        public string Id { get; set; }
        public string OrderNumber { get; set; }
        public string CampaignName { get; set; }
        public string WhiteLabel { get; set; }
        public string Quantity { get; set; }
        public string Opened { get; set; }
        public string Clicked { get; set; }
        public string Status { get; set; }

        public string IONumber { get; set; }
        public string OrderDate { get; set; }
        public string StartDate { get; set; }
        public string EmailsSent { get; set; }
        public string OpenedPercentage { get; set; }
        public string ClickedPercentage { get; set; }
        public string CTRPercentage { get; set; }
        
        public List<CampaignReportDetailVM> PerLink { get; set; }
    }
}