using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WFP.ICT.Data.Entities;

namespace WFP.ICT.Web.Models
{
    public class ProDataResponse
    {
        public string status { get; set; }
        public Reports reports { get; set; }
        public string timestamp { get; set; }
    }

    public class Reports
    {
        public Report[] report { get; set; }
    }

    public class Report
    {
        public string IO { get; set; }
        public string Reportsite_URL { get; set; }
        public string Destination_URL { get; set; }
        public string CampaignStartDate { get; set; }
        public string ClickCount { get; set; }
        public string CampaignName { get; set; }
        public long UniqueCnt { get; set; }
        public long MobileCnt { get; set; }
        public long ImpressionCnt { get; set; }
    }

}