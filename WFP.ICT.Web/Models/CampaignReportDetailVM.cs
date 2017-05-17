using System.Collections.Generic;
using System.Web.Mvc;

namespace WFP.ICT.Web.Models
{
    public class CampaignReportDetailVM : CampaignReportVM
    {
        public string IONumber { get; set; }
        public string Link { get; set; }
        public string QuantityDetail { get; set; }

    }
}